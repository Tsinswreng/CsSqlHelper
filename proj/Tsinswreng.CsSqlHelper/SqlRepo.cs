namespace Tsinswreng.CsSqlHelper;

using System.Data;


using Tsinswreng.CsCore;
using Tsinswreng.CsDictMapper;
using Tsinswreng.CsTools;
using Tsinswreng.CsPage;
using System.Collections;
using System.Diagnostics;
using Str_Any = System.Collections.Generic.Dictionary<str, obj?>;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;

//using T = Bo_Word;
//TODO 拆分ⁿ使更通用化
//TODO 分頁
public partial class SqlRepo<
	TEntity, TId
>
	:IRepo<TEntity, TId>
	where TEntity: class, new()
{




	public async Task<IAsyncEnumerable<TEntity?>> SlctManyInIdsWithDel(
		IDbFnCtx Ctx, IEnumerable<TId> Ids
		,CT Ct
	){
		IList<IParam> Params = [];
		var sqlD = FnSqlDuplicator.Mk((Cnt)=>{
			Params = T.NumParams(Cnt);
			return $"SELECT * FROM {T.Qt(T.DbTblName)} WHERE {T.QtCol(T.CodeIdName)} IN ({str.Join(", ", Params)})" ;
		});
		var bat = SqlCmdMkr.AutoBatch<TId, IAsyncEnumerable<TEntity?>>(
			Ctx, sqlD,
			async(z, Ids, Ct)=>{
				var Args = ArgDict.Mk(T).AddManyT(Params, Ids, T.CodeIdName);
				var RawDicts = z.SqlCmd.Args(Args).AsyE1d(Ct);
				return RawDicts.Select(x=>T.DbDictToEntity(x));
			}
		);
		var R = bat.AddToEnd(Ids, Ct);
		return R.Flat();
	}

	public async Task<IAsyncEnumerable<TEntity?>> SlctManyInIds(
		IDbFnCtx Ctx, IEnumerable<TId> Ids
		,CT Ct
	){
		IList<IParam> Params = [];
		var sqlD = FnSqlDuplicator.Mk((Cnt)=>{
			Params = T.NumParams(Cnt);
			return 
$"""
SELECT * FROM {T.Qt(T.DbTblName)} WHERE {T.QtCol(T.CodeIdName)} IN ({str.Join(", ", Params)})
{T.AndSqlIsNonDel()}
""" ;
		});
		var bat = SqlCmdMkr.AutoBatch<TId, IAsyncEnumerable<TEntity?>>(
			Ctx, sqlD,
			async(z, Ids, Ct)=>{
				var Args = ArgDict.Mk(T).AddManyT(Params, Ids, T.CodeIdName);
				var RawDicts = z.SqlCmd.Args(Args).AsyE1d(Ct);
				return RawDicts.Select(x=>T.DbDictToEntity(x));
			}
		);
		var R = bat.AddToEnd(Ids, Ct);
		return R.Flat();
	}


	public async Task<IAsyncEnumerable<TEntity?>> BatSlctById(
		IDbFnCtx Ctx, IEnumerable<TId> Ids
		,CT Ct
	){
		var PId = T.Prm(T.CodeIdName);
		var Sql = T.SqlSplicer().Select("*").From().Where1()
		.Raw("And "+T.QtCol(T.CodeIdName)+"="+PId);

		var bat = AutoBatch<TId, IAsyncEnumerable<TEntity?>>.Mk(
			Ctx, SqlCmdMkr, Sql,
			async(z, Ids, Ct)=>{
				var Args = ArgDict.Mk(T).AddManyT(PId, Ids, T.CodeIdName);
				var RawDicts = z.SqlCmd.Args(Args).AsyE1d(Ct);
				return RawDicts.Select(x=>T.DbDictToEntity<TEntity>(x));
			}
		);
		var R = bat.AddToEnd(Ids, Ct);
		return R.Flat();
	}

	public async Task<IAsyncEnumerable<TAgg?>> BatSlctAggById<TAgg>(
		IDbFnCtx Ctx, IEnumerable<TId> Ids
		,CT Ct
	)
		where TAgg: class
	{
		var aggReg = TblMgr.GetAgg<TAgg>();
		if(aggReg.RootEntityType != typeof(TEntity)){
			throw new Exception($"Agg root type mismatch. Agg={typeof(TAgg)}, ExpectedRoot={typeof(TEntity)}, RegisteredRoot={aggReg.RootEntityType}");
		}
		if(aggReg.RootIdType != typeof(TId)){
			throw new Exception($"Agg root id type mismatch. Agg={typeof(TAgg)}, ExpectedId={typeof(TId)}, RegisteredId={aggReg.RootIdType}");
		}

		u64 InBatchSize = TblMgr.DbSrcType == EDbSrcType.Sqlite ? 50ul : 500ul;

		async Task<IList<TAgg?>> HandleOneBatch(IList<TId> OrderedBatchIds, CT Ct){
			var rootsAsy = await SlctManyInIdsWithDel(Ctx, OrderedBatchIds, Ct);
			var rootById = new Dictionary<object, TEntity>();
			var rootIdSet = new HashSet<TId>();
			await foreach(var root in rootsAsy.WithCancellation(Ct)){
				if(root is null){
					continue;
				}
				var keyObj = aggReg.FnGetIdFromRootObj(root);
				if(keyObj is null){
					continue;
				}
				if(keyObj is not TId key){
					throw new Exception($"Agg root key type mismatch. Agg={typeof(TAgg)}, Root={typeof(TEntity)}, Key={keyObj.GetType()}, ExpectedKey={typeof(TId)}");
				}
				rootById[key] = root;
				rootIdSet.Add(key);
			}

			var qryCtx = new AggQryCtx();
			if(rootIdSet.Count > 0){
				var rootIds = rootIdSet.ToList();
				var optQry = new OptQry{ InParamCnt = (u64)rootIds.Count };
				foreach(var include in aggReg.Includes){
					var slctByIn = await FnScltAllByColInVals<TId>(Ctx, include.Tbl, include.FKeyCodeCol, optQry, Ct);
					var dbAsy = await slctByIn(rootIds, Ct);
					await foreach(var dbDict in dbAsy.WithCancellation(Ct)){
						var codeDict = include.Tbl.ToCodeDict(dbDict);
						var entity = include.FnNewEntityObj();
						include.Tbl.DictMapper.AssignShallow(include.EntityType, entity, codeDict);
						var keyObj = include.FnFKeyToRootIdObj(entity);
						if(keyObj is null){
							continue;
						}
						if(include.RelKind == EAggRelKind.OneToOne
							&& qryCtx.GetOne(include.EntityType, keyObj) is not null
						){
							throw new Exception($"OneToOne include got duplicate rows. Agg={typeof(TAgg)}, Include={include.EntityType}, Key={keyObj}");
						}
						qryCtx.Add(include.EntityType, keyObj, entity);
					}
				}
			}

			var ans = new List<TAgg?>(OrderedBatchIds.Count);
			foreach(var id in OrderedBatchIds){
				if(!rootById.TryGetValue(id!, out var root)){
					ans.Add(null);
					continue;
				}
				var agg = (TAgg)aggReg.FnAssembleAggObj(root, qryCtx);
				ans.Add(agg);
			}
			return ans;
		}

		async IAsyncEnumerable<TAgg?> Fn(IAsyncEnumerable<TAgg?> Src){
			await foreach(var item in Src.WithCancellation(Ct)){
				yield return item;
			}
		}

		async IAsyncEnumerable<TAgg?> Run(){
			var batchIds = new List<TId>((i32)InBatchSize);
			foreach(var id in Ids){
				batchIds.Add(id);
				if((u64)batchIds.Count < InBatchSize){
					continue;
				}
				var batchAns = await HandleOneBatch(batchIds, Ct);
				foreach(var item in batchAns){
					yield return item;
				}
				batchIds = new List<TId>((i32)InBatchSize);
			}

			if(batchIds.Count > 0){
				var batchAns = await HandleOneBatch(batchIds, Ct);
				foreach(var item in batchAns){
					yield return item;
				}
			}
		}

		return Fn(Run());
	}




	

	public delegate Task<IDictionary<TKey, IList<TPo>>> TFnIncludeEntitysByKeys<TKey, TPo>(
		ITable Tbl, Func<TPo, TKey> FnMemb, IEnumerable<TKey?> Keys, CT Ct
	);

/*
Func<
		ITable, Func<TPo, TKey>, IEnumerable<TKey>
		,CT, Task<IDictionary<TKey, IList<TPo>>>
	>
 */
	protected async Task<TFnIncludeEntitysByKeys<TKey, TPo>> FnIncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,ITable Tbl
		,str CodeCol
		,OptQry? OptQry
		,CT Ct
	)where TPo: new(){
		var fn = await FnScltAllByColInVals<TKey>(Ctx, Tbl, CodeCol, OptQry, Ct);
		return async(Tbl, Memb, Keys, Ct)=>{
			IList<TKey> KeyList = Keys.Where(x=>x is not null).ToList()!;
			var poPage = await fn(KeyList, Ct);
			var dicts = await poPage.ToListAsync(Ct);
			var pos = dicts.Select(x=>Tbl.DbDictToEntity<TPo>(x));
			IDictionary<TKey, IList<TPo>> posByKey = pos.GroupBy(Memb).ToDictionary(g=>g.Key, g=>(IList<TPo>)g.ToList());
			return posByKey;
		};
	}

	public async Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey?> Keys
		,Func<TPo, TKey> FnMemb
		,ITable Tbl
		,CT Ct
	)where TPo: new(){
		//var keyList = Keys.AsOrToList();
		// 自動把OptQry之ParamCnt設成 Keys.Count?
		var fn = await FnIncludeEntitysByKeys<TPo, TKey>(Ctx, Tbl, CodeCol, OptQry, Ct);
		return await fn(Tbl, FnMemb, Keys, Ct);
	}

	public async Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey?> Keys
		,Func<TPo, TKey> FnMemb
		,ITable<TPo> Tbl
		,CT Ct
	)where TPo: new(){
		//var keyList = Keys.AsOrToList();
		// 自動把OptQry之ParamCnt設成 Keys.Count?
		var fn = await FnIncludeEntitysByKeys<TPo, TKey>(Ctx, Tbl, CodeCol, OptQry, Ct);
		return await fn(Tbl, FnMemb, Keys, Ct);
	}

	
}

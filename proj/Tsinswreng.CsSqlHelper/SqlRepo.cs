namespace Tsinswreng.CsSqlHelper;

using System.Data;

using IDbFnCtx = IBaseDbFnCtx;
using Tsinswreng.CsCore;
using Tsinswreng.CsDictMapper;
using Tsinswreng.CsTools;
using Tsinswreng.CsPage;


//using T = Bo_Word;
//TODO 拆分ⁿ使更通用化
//TODO 分頁
public partial class SqlRepo<
	TEntity, TId
>
	:IRepo<TEntity, TId>
	where TEntity: class, new()
{

public ITblMgr TblMgr{get;set;}
public ISqlCmdMkr SqlCmdMkr{get;set;}
public IDictMapperShallow DictMapper{get;set;}

	public SqlRepo(
		ITblMgr TblMgr
		,ISqlCmdMkr SqlCmdMkr
		,IDictMapperShallow DictMapper
	){
		this.DictMapper = DictMapper;
		this.TblMgr = TblMgr;
		this.SqlCmdMkr = SqlCmdMkr;
	}

	public async Task<Func<
		CT
		,Task<u64>
	>> FnCount(
		IDbFnCtx? Ctx
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var NCnt = "Cnt";
		var Sql =
$"SELECT COUNT(*) AS {T.Qt(NCnt)} FROM {T.Qt(T.DbTblName)}";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDispose(Cmd);
		var Fn = async(
			CT Ct
		)=>{
			var CountDict = await Cmd.IterAsyE(Ct).FirstOrDefaultAsync(Ct);
			u64 R = 0;
			if (CountDict != null){
				if(CountDict.TryGetValue(NCnt, out var Cnt)){
					if(Cnt is u64 || Cnt is i64 || Cnt is u32 || Cnt is i32){
						R = Convert.ToUInt64(Cnt);
					}
				}
			}
			return R;
		};
		return Fn;
	}

	public async Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<IDictionary<str, obj?>>>
	>> FnPageAllDict(IDbFnCtx? Ctx, CT Ct){
		var T = TblMgr.GetTbl<TEntity>();
		var Sql = $"""
		SELECT * FROM {T.Qt(T.DbTblName)}
		{T.SqlMkr.ParamLimOfst(out var Lim, out var Ofst)}
		""";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		var CountAll = await FnCount(Ctx, Ct);
		Ctx?.AddToDispose(Cmd);
		var Fn = async(IPageQry Qry, CT Ct)=>{
			var Arg = ArgDict.Mk().AddPageQry(Qry, Lim, Ofst);
			var Ran = Cmd.Args(Arg).IterAsyE(Ct);

			u64 Cnt = 0;
			var HasCnt = false;
			if(Qry.WantTotCnt){
				Cnt = await CountAll(Ct);
				HasCnt = true;
			}

			var R = PageAsyE.Mk(
				Qry, Ran, HasCnt, Cnt
			);
			return R;
		};
		return Fn;
	}

	public async Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<TEntity>>
	>> FnPageAll(IDbFnCtx Ctx, CT Ct){
		var T = TblMgr.GetTbl<TEntity>();
		var PageAllDict = await FnPageAllDict(Ctx, Ct);
		var Fn = async(IPageQry Qry, CT Ct)=>{
			var AllDictPage = await PageAllDict(Qry, Ct);
			if(AllDictPage.DataAsyE != null){
				var EntityData = AllDictPage.DataAsyE.Select(d=>T.DbDictToEntity<TEntity>(d));
				var R = PageAsyE.Mk(Qry, EntityData, AllDictPage.HasTotCnt, AllDictPage.TotCnt);
				return R;
			}else{
				return PageAsyE.Mk<TEntity>(Qry, null, false, 0);
			}
		};
		return Fn;
	}

	//public class _ClsInsrtMany<E,I>(SqlRepo<E,I> z)

//TODO 用多值插入語法、免 for循環中多次查詢
	protected async Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> _FnInsertMany(
		IDbFnCtx? Ctx
		,bool Prepare
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var Clause = T.InsertClause(T.Columns.Keys);
		var Sql =
$"INSERT INTO {T.Qt(T.DbTblName)} {Clause}";
		var Cmd = await SqlCmdMkr.MkCmd(Ctx, Sql, Ct);
		if(Prepare){
			Cmd = await SqlCmdMkr.Prepare(Cmd, Ct);
		}
		Ctx?.AddToDispose(Cmd);
		var Fn = async(
			IEnumerable<TEntity> Entitys
			,CT ct
		)=>{
			var i = 0;
			foreach(var entity in Entitys){
				var CodeDict = DictMapper.ToDictShallowT(entity);
				var DbDict = T.ToDbDict(CodeDict);
				await Cmd.RawArgs(DbDict).IterAsyE(ct).FirstOrDefaultAsync(ct);
				i++;
			}
			return NIL;
		};
		return Fn;
	}



	[Impl]
	public async Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertMany(
		IDbFnCtx? Ctx
		,CT Ct
	){
		return await _FnInsertMany(Ctx, true, Ct);
	}

	public async Task<Func<
		TEntity
		,CT
		,Task<nil>
	>> FnInsertOne(
		IDbFnCtx? Ctx
		,CT Ct
	){
		var InsrtMany = await FnInsertMany(Ctx, Ct);
		return async(Entity, Ct)=>{
			await InsrtMany([Entity], Ct);
			return NIL;
		};
	}


/// <summary>
/// 不預編譯。適用于況芝 在事務中 初建表後即添數據
/// </summary>
/// <param name="Ctx"></param>
/// <param name="ct"></param>
/// <returns></returns>
	public async Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertManyNoPrepare(
		IDbFnCtx? Ctx
		,CT ct
	){
		return await _FnInsertMany(Ctx, false, ct);
	}


	[Impl]
	public async Task<Func<
		TId
		,CT
		,Task<TEntity?>
	>> FnSlctById(
		IDbFnCtx? Ctx
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var Params = T.Prm(0,0);
		var Sql = $"SELECT * FROM {T.Qt(T.DbTblName)} WHERE {T.Fld(T.CodeIdName)} = {Params[0]}" ;
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDisposeAsy(Cmd);
		var Fn = async(
			TId Id
			,CT Ct
		)=>{
			var IdCol = T.Columns[T.CodeIdName];
			var ConvertedId = IdCol.UpperToRaw?.Invoke(Id)??Id;
			var RawDict = await Cmd
				.Args([ConvertedId])
				.IterAsyE(Ct).FirstOrDefaultAsync(Ct)
			;
			if(RawDict == null){
				return null;
			}
			var CodeDict = T.ToCodeDict(RawDict);
			var R = new TEntity();
			DictMapper.AssignShallowT(R, CodeDict);
			return R;
		};
		return Fn;
	}

// 	[Impl]
// 	public async Task<Func<
// 		obj//raw
// 		,CT
// 		,Task<IPage<IDictionary<str, obj?>>>
// 	>> FnPageByOneCol(IDbFnCtx? Ctx, str DbColName, CT Ct){
// var T = TblMgr.GetTbl<TEntity>();
// var Sql =
// """
// """
// 	}

	/// <summary>
	///
	/// </summary>
	/// <param name="Ctx"></param>
	/// <param name="UpperFieldsToUpdate">潙null旹更新全部字段</param>
	/// <param name="Ct"></param>
	/// <returns></returns>
	public async Task<Func<
		TId
		,TEntity
		,CT, Task<nil>
	>> FnUpdByIdOld(
		IDbFnCtx? Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var NId = T.CodeIdName;
		UpperFieldsToUpdate = UpperFieldsToUpdate??T.Columns.Keys;
		var Clause = T.UpdateClause(UpperFieldsToUpdate);
		var FieldsToUpdateMap = UpperFieldsToUpdate.ToHashSet();
		var Sql =
$"UPDATE {T.Qt(T.DbTblName)} SET {Clause} WHERE {T.Fld(NId)} = {T.Prm(NId)}";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDispose(Cmd);
		return async(Id, Entity, Ct)=>{
			var Arg = ArgDict.Mk(T);
			var CodeDict = DictMapper.ToDictShallowT(Entity);
			foreach(var (k,v) in CodeDict){
				if(FieldsToUpdateMap.Contains(k)){
					Arg.AddT(T.Prm(k), v, k);
				}
			}
			await Cmd.Args(Arg).All(Ct);
			return NIL;
		};
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="Ctx"></param>
	/// <param name="UpperFieldsToUpdate">潙null旹更新全部字段</param>
	/// <param name="Ct"></param>
	/// <returns></returns>
	public async Task<Func<
		TEntity
		,CT, Task<nil>
	>> FnUpdOneById(
		IDbFnCtx? Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var NId = T.CodeIdName;
		UpperFieldsToUpdate = UpperFieldsToUpdate??T.Columns.Keys;
		var Clause = T.UpdateClause(UpperFieldsToUpdate);
		var FieldsToUpdateMap = UpperFieldsToUpdate.ToHashSet();
		var Sql =
$"UPDATE {T.Qt(T.DbTblName)} SET {Clause} WHERE {T.Fld(NId)} = {T.Prm(NId)}";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDispose(Cmd);
		return async(Entity, Ct)=>{
			var Arg = ArgDict.Mk(T);
			var CodeDict = DictMapper.ToDictShallowT(Entity);
			foreach(var (k,v) in CodeDict){
				if(FieldsToUpdateMap.Contains(k)){
					Arg.AddT(T.Prm(k), v, k);
				}
			}
			await Cmd.Args(Arg).All(Ct);
			return NIL;
		};
	}



	/// <param name="FieldsToUpdate">潙null旹更新全部字段</param>
	/// TODO 用批量插入sql 代替for循環
	public async Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnUpdManyById(
		IDbFnCtx? Ctx
		,IEnumerable<str>? FieldsToUpdate
		,CT Ct
	){
		var UpdById = await FnUpdOneById(Ctx, FieldsToUpdate, Ct);
		return async(Entitys, Ct)=>{
			foreach(var e in Entitys){
				await UpdById(e, Ct);
			}
			return NIL;
		};
	}


	[Obsolete]
	[Impl]
	public async Task<Func<
		IEnumerable<Id_Dict<TId>>
		,CT
		,Task<nil>
	>> FnUpdManyByIdOld(
		IDbFnCtx? Ctx
		,IEnumerable<str> FieldsToUpdate
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var NId = T.CodeIdName;
		var Clause = T.UpdateClause(FieldsToUpdate);
		var Sql =
$"UPDATE {T.Qt(T.DbTblName)} SET ${Clause} WHERE {T.Fld(NId)} = {T.Prm(NId)}";

		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDispose(Cmd);
		var Fn = async(
			IEnumerable<Id_Dict<TId>> Id_Dicts
			,CT ct
		)=>{
			foreach(var id_dict in Id_Dicts){
				var CodeId = id_dict.Id;
				var CodeDict = id_dict.Dict;
				var DbDict = T.ToDbDict(CodeDict);
				await Cmd.RawArgs(DbDict).IterAsyE(ct).FirstOrDefaultAsync(ct);
			}//~for
			return NIL;
		};
		return Fn;
	}

	// public async Task<I_Answer<nil>> UpdateManyAsy(IEnumerable<T_Entity> EntityList){
	// 	I_Answer<nil> ans = new Answer<nil>();
	// 	IDbContextTransaction tx = null!;
	// 	//DbCtx.Entry
	// 	try{
	// 		tx = await DbCtx.Database.BeginTransactionAsync();

	// 		foreach (var newEntity in EntityList){
	// 			var existingEntity = DbCtx.Set<T_Entity>().Find(newEntity.Id);
	// 			if (existingEntity == null){
	// 				break;
	// 			}
	// 			foreach (var property in DbCtx.Entry(existingEntity).Properties){
	// 				var newValue = newEntity.GetType().GetProperty(property.Metadata.Name)?.GetValue(newEntity);
	// 				if (!Equals(property.CurrentValue, newValue)){
	// 					property.CurrentValue = newValue;
	// 					property.IsModified = true;
	// 				}
	// 			}
	// 		}

	// 		await DbCtx.SaveChangesAsync();
	// 		await tx.CommitAsync();
	// 		return ans.OkWith(Nil);
	// 	}
	// 	catch (Exception e){
	// 		ans.AddErrException(e);
	// 		await tx.RollbackAsync();
	// 	}
	// 	return ans;
	// }


//TODO TEST
	[Impl]
	public async Task<Func<
		IEnumerable<obj?>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys(
		IDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		if(T.SoftDelCol == null){
			throw new Exception("SoftDeleteCol is null");
		}
		var ParamList = T.Prm(1, CountPerBatch);
		var Sql =
$"""
UPDATE {T.Qt(T.DbTblName)}
SET {T.Fld(T.SoftDelCol.CodeColName)} = {T.Prm(0,0)[0]}
WHERE {T.Fld(KeyNameInCode)} IN ({str.Join(",", ParamList)})
AND {T.Fld(KeyNameInCode)} IS NOT NULL
;
""";
		var ValToSet = T.SoftDelCol.FnDelete(null);
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDispose(Cmd);
		var Fn = async(
			IEnumerable<object?> Keys
			,CT Ct
		)=>{
			await using BatchListAsy<object?, nil> BatchList = new(async (x, Ct)=>{
				IList<object?> Args = [ValToSet, ..x];
				Args.FillUpTo(CountPerBatch+1, null);
				await Cmd.Args(Args).IterAsyE(Ct).FirstOrDefaultAsync(Ct);
				return NIL;
			});

			foreach(var key in Keys){
				await BatchList.Add(key, Ct);
			}
			await BatchList.End(Ct);
			return NIL;
		};
		return Fn;
	}

	[Impl]
	public async Task<Func<
		IEnumerable<TKey>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys<TKey>(
		IDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 ParamNum
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var NonGeneric = await FnSoftDelManyByKeys(Ctx, KeyNameInCode, ParamNum, Ct);
		var Fn = async(
			IEnumerable<TKey> Keys
			,CT Ct
		)=>{
			var Args = Keys.Select(K=>T.UpperToRaw(K, KeyNameInCode));
			await NonGeneric(Args, Ct);
			return NIL;
		};
		return Fn;
	}

/// TODO 用Where Id IN (@0, @1, @2...) 㕥減次芝往返
	public async Task<Func<
		TId
		,CT
		,Task<nil>
	>> FnDeleteOneById(
		IDbFnCtx? Ctx
		,CT ct
	){
		var T = TblMgr.GetTbl<TEntity>();
var Sql = $"DELETE FROM {T.DbTblName} WHERE {T.Fld(T.CodeIdName)} = ?";

		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, ct);
		Ctx?.AddToDispose(Cmd);
		async Task<nil> Fn(
			TId Id
			,CT Ct
		) {
			if (Id is not TId id) {
				throw new Exception("Id is not T_Id id");
			}
			var IdCol = T.Columns[T.CodeIdName];
			var ConvertedId = IdCol.UpperToRaw?.Invoke(Id);
			await Cmd.Args([ConvertedId]).IterAsyE(Ct).FirstOrDefaultAsync(Ct);
			return NIL;
		}
		return Fn;
	}


//TODO TEST
[Obsolete("等FnSoftDeleteManyByKeys測試無誤後再照搬")]
	public async Task<Func<
		IEnumerable<object?>
		,CT
		,Task<nil>
	>> FnDeleteManyByKeys(
		IDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 ParamNum
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var Clause = T.NumParamClause(ParamNum-1);
		var Sql =
$"""
DELETE FROM {T.Qt(T.DbTblName)} WHERE {T.Fld(KeyNameInCode)} IN ${Clause}
AND {T.Qt(KeyNameInCode)} IS NOT NULL;
""";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDispose(Cmd);
		var Fn = async(
			IEnumerable<object?> Keys
			,CT Ct
		)=>{

			IList<object?> Args = new List<object?>();
			u64 i = 0, j=0;
			foreach(var key in Keys){
				Args.Add(key);
				if(j == ParamNum - 1){
					await Cmd.Args(Args).IterAsyE(Ct).FirstOrDefaultAsync(Ct);
					Args.Clear();
					j = 0;
				}
			i++;j++;}
			if(j > 0){
				await Cmd.Args(Args).IterAsyE(Ct).FirstOrDefaultAsync(Ct);
			}
			return NIL;
		};
		return Fn;
	}


	public async Task<Func<
		IEnumerable<TKey>
		,CT
		,Task<nil>
	>> FnDeleteManyByKeys<TKey>(
		IDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 ParamNum
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
		var NonGeneric = await FnDeleteManyByKeys(Ctx, KeyNameInCode, ParamNum, Ct);
		var Fn = async(
			IEnumerable<TKey> Keys
			,CT Ct
		)=>{
			var Args = Keys.Select(Id => T.UpperToRaw(Id, KeyNameInCode));
			await NonGeneric(Args, Ct);
			return NIL;
		};
		return Fn;
	}

	[Impl]
	public async Task<Func<
		TId
		,obj?
		,CT
		,Task<nil>
	>> FnUpdOneColById(
		IDbFnCtx? Ctx
		,str Col
		,CT Ct
	){
		var T = TblMgr.GetTbl<TEntity>();
str NId = T.CodeIdName;
var PTarget = T.Prm("__Target");var PId = T.Prm(NId);
var Sql = $"""
UPDATE {T.Qt(T.DbTblName)}
SET {T.Qt(Col)} = {PTarget}
WHERE {T.Fld(NId)} = {PId}
""";
var SqlCmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		Ctx?.AddToDispose(SqlCmd);
		var Fn = async(TId Id, obj? Target, CT Ct)=>{
			var Arg = ArgDict.Mk()
				.AddRaw(PId, T.UpperToRaw(Id, NId))
				.AddRaw(PTarget, Target)
				.ToDict()
			;
			await SqlCmd.RawArgs(Arg).IterAsyE(Ct).FirstOrDefaultAsync(Ct);
			return NIL;
		};
		return Fn;
	}


	// public async Task<Func<
	// 	IEnumerable<T_Entity>
	// 	,i64
	// 	,CT
	// 	,Task<nil>
	// >> Fn_BatchSetUpdateAtAsy(
	// 	CT ct
	// ){
	// 	var Fn = async(
	// 		IEnumerable<T_Entity> Pos
	// 		,i64 Time
	// 		,CT ct
	// 	)=>{
	// 		foreach(var po in Pos){
	// 			if(po is not I_HasId<T_Id> IdPo){
	// 				continue;
	// 			}

	// 		}
	// 		return Nil;
	// 	};
	// 	return Fn;
	// }


// 	public async Task<Func<
// 		IEnumerable<T_Id2>
// 		,CT
// 		,Task<nil>
// 	>> Fn_DeleteManyByIdAsy<T_Id2>(){
// 		var Tbl = TblMgr.GetTable<T_Entity>();
// 		var Cmd = Connection.CreateCommand();
// 		Cmd.CommandText =
// $"DELETE FROM {Tbl.Name} WHERE ${nameof(I_Id<nil>.Id)} IN ?";
// 		var Fn = async(
// 			IEnumerable<T_Id2> Ids
// 			,CT ct
// 		)=>{
// 			// if(Id is not T_Id id){
// 			// 	throw new Exception("Id is not T_Id id");
// 			// }

// 			var IdCol = Tbl.Columns[nameof(I_Id<nil>.Id)];
// 			var ConvertedId = IdCol.ToDbType(Id);
// 			Cmd.Parameters.AddWithValue("", ConvertedId);
// 			using var Reader = await Cmd.ExecuteReaderAsync(ct);
// 			return Nil;
// 		};
// 		return Fn;
// 	}


	// public async Task<T_Ret> TxnAsy<T_Ret>(
	// 	Func<CT, Task<T_Ret>> FnAsy
	// 	,CT ct
	// ){
	// 	using var Tx = await DbCtx.Database.BeginTransactionAsync(ct);
	// 	try{
	// 		var ans = await FnAsy(ct);
	// 		await Tx.CommitAsync(ct);
	// 		return ans;
	// 	}
	// 	catch (System.Exception){
	// 		await Tx.RollbackAsync(ct);
	// 		throw;
	// 	}
	// }


}

namespace Tsinswreng.CsSqlHelper;

using System.Data;
using Tsinswreng.CsTools.Tools;
using Tsinswreng.CsDictMapper.DictMapper;

using IDbFnCtx = IBaseDbFnCtx;
using Tsinswreng.CsCore;

//using T = Bo_Word;
//TODO 拆分ⁿ使更通用化
public class Repo<
	TEntity, TId
>
	:IRepo<TEntity, TId>
	where TEntity: class, new()
	// where TEntity: class, I_Id<TId>, new()
	// where TId : IEquatable<TId>

{

public ITblMgr TblMgr{get;set;}
public ISqlCmdMkr SqlCmdMkr{get;set;}
public IDictMapperShallow DictMapper{get;set;}

	public Repo(
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
		var T = TblMgr.GetTable<TEntity>();
		var NCnt = "Cnt";
		var Sql =
$"SELECT COUNT(*) AS {T.Quote(NCnt)} FROM {T.Quote(T.Name)}";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		var Fn = async(
			CT Ct
		)=>{
			var CountDict = await Cmd.Run(Ct).FirstOrDefaultAsync(Ct);
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


	protected async Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> _FnInsertMany(
		IDbFnCtx? Ctx
		,bool Prepare
		,CT Ct
	){
		var T = TblMgr.GetTable<TEntity>();
		var Clause = T.InsertClause(T.Columns.Keys);
		var Sql =
$"INSERT INTO {T.Quote(T.Name)} {Clause}";
		var Cmd = await SqlCmdMkr.MkCmd(Ctx, Sql, Ct);
		if(Prepare){
			Cmd = await SqlCmdMkr.Prepare(Cmd, Ct);
		}
		var Fn = async(
			IEnumerable<TEntity> Entitys
			,CT ct
		)=>{
			var i = 0;
			foreach(var entity in Entitys){
				var CodeDict = DictMapper.ToDictShallowT(entity);
				var DbDict = T.ToDbDict(CodeDict);
				await Cmd.Args(DbDict).Run(ct).FirstOrDefaultAsync(ct);
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
	>> FnSelectById(
		IDbFnCtx? Ctx
		,CT ct
	){
		var T = TblMgr.GetTable<TEntity>();
		var Params = T.MkUnnamedParam(0,0);
		var Sql = $"SELECT * FROM {T.Quote(T.Name)} WHERE {T.Field(T.CodeColId)} = {Params[0]}" ;
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, ct);

		var Fn = async(
			TId Id
			,CT ct
		)=>{
			if(Id is not TId id){
				throw new Exception("Id is not TId id");
			}
			var IdCol = T.Columns[T.CodeColId];
			var ConvertedId = IdCol.UpperToRaw(Id);
			var RawDict = await Cmd
				.Args([ConvertedId])
				.Run(ct).FirstOrDefaultAsync(ct)
			;
			if(RawDict == null){
				return null;
			}
			var CodeDict = T.ToCodeDict(RawDict);
			var Ans = new TEntity();
			DictMapper.AssignShallowT(Ans, CodeDict);
			return Ans;
		};
		return Fn;
	}

	[Impl]
	public async Task<Func<
		IEnumerable<Id_Dict<TId>>
		,CT
		,Task<nil>
	>> FnUpdateManyById(
		IDbFnCtx? Ctx
		,IDictionary<str, object?> ModelDict //不當有Id
		,CT ct
	){
		var T = TblMgr.GetTable<TEntity>();
		ModelDict = new Dictionary<str, object?>(ModelDict);
		var NId = T.CodeColId;
		ModelDict.Remove(NId);
		var Clause = T.UpdateClause(ModelDict.Keys);

		var Sql =
$"UPDATE {T.Quote(T.Name)} SET ${Clause} WHERE {T.Field(NId)} = {T.Param(NId)}";

		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, ct);
		var Fn = async(
			IEnumerable<Id_Dict<TId>> Id_Dicts
			,CT ct
		)=>{
			foreach(var id_dict in Id_Dicts){
				var CodeId = id_dict.Id;
				var CodeDict = id_dict.Dict;
				var DbDict = T.ToDbDict(CodeDict);
				await Cmd.Args(DbDict).Run(ct).FirstOrDefaultAsync(ct);
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
	public async Task<Func<
		IEnumerable<object?>
		,CT
		,Task<nil>
	>> FnSoftDeleteManyByKeys(
		IDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 InClauseParamNum
		,CT Ct
	){
		var T = TblMgr.GetTable<TEntity>();
		if(T.SoftDeleteCol == null){
			throw new Exception("SoftDeleteCol is null");
		}
		var ParamList = T.MkUnnamedParam(1, InClauseParamNum);
		var Sql =
$"""
UPDATE {T.Quote(T.Name)}
SET {T.Field(T.SoftDeleteCol.CodeColName)} = {T.MkUnnamedParam(0,0)[0]}
WHERE {T.Field(KeyNameInCode)} IN ({str.Join(",", ParamList)})
AND {T.Field(KeyNameInCode)} IS NOT NULL
;
""";
		var ValToSet = T.SoftDeleteCol.FnDelete(null);
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);

		var Fn = async(
			IEnumerable<object?> Keys
			,CT Ct
		)=>{
			await using BatchListAsy<object?, nil> BatchList = new(async (x, Ct)=>{
				IList<object?> Args = [ValToSet, ..x];
				Args = Args.FillUpTo(InClauseParamNum, null);
				await Cmd.Args(Args).Run(Ct).FirstOrDefaultAsync(Ct);
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
		var T = TblMgr.GetTable<TEntity>();
		var NonGeneric = await FnSoftDeleteManyByKeys(Ctx, KeyNameInCode, ParamNum, Ct);
		var Fn = async(
			IEnumerable<TKey> Keys
			,CT Ct
		)=>{
			var Args = Keys.Select(K=>T.ToDbType(KeyNameInCode, K));
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
		var T = TblMgr.GetTable<TEntity>();
var Sql = $"DELETE FROM {T.Name} WHERE {T.Field(T.CodeColId)} = ?";

		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, ct);
		async Task<nil> Fn(
			TId Id
			, CT ct
		) {
			if (Id is not TId id) {
				throw new Exception("Id is not T_Id id");
			}
			var IdCol = T.Columns[T.CodeColId];
			var ConvertedId = IdCol.UpperToRaw(Id);
			await Cmd.Args([ConvertedId]).Run(ct).FirstOrDefaultAsync(ct);
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
		var T = TblMgr.GetTable<TEntity>();
		var Clause = T.NumParamClause(ParamNum-1);
		var Sql =
$"""
DELETE FROM {T.Quote(T.Name)} WHERE {T.Field(KeyNameInCode)} IN ${Clause}
AND {T.Quote(KeyNameInCode)} IS NOT NULL;
""";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
		var Fn = async(
			IEnumerable<object?> Keys
			,CT Ct
		)=>{

			IList<object?> Args = new List<object?>();
			u64 i = 0, j=0;
			foreach(var key in Keys){
				Args.Add(key);
				if(j == ParamNum - 1){
					await Cmd.Args(Args).Run(Ct).FirstOrDefaultAsync(Ct);
					Args.Clear();
					j = 0;
				}
			i++;j++;}
			if(j > 0){
				await Cmd.Args(Args).Run(Ct).FirstOrDefaultAsync(Ct);
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
		var T = TblMgr.GetTable<TEntity>();
		var NonGeneric = await FnDeleteManyByKeys(Ctx, KeyNameInCode, ParamNum, Ct);
		var Fn = async(
			IEnumerable<TKey> Keys
			,CT Ct
		)=>{
			var Args = Keys.Select(Id => T.ToDbType(KeyNameInCode, Id));
			await NonGeneric(Args, Ct);
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

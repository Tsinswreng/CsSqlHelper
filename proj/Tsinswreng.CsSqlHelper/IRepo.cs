//此文件中的API已廢棄
namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsPage;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;


public partial interface IRepo<TEntity, TId>{
	
	


	[Doc(@$"using `Id IN (...)` Clause,
	which would ignore unexisted Id and returned list may be unordered.
	")]
	public Task<IAsyncEnumerable<TEntity?>> SlctManyInIdsWithDel(
		IDbFnCtx Ctx, IAsyncEnumerable<TId> Ids
		,CT Ct
	);

	[Doc(@$"Got Entities are corresponding to the given Ids. if not found, the place will be null.")]
	public Task<IAsyncEnumerable<TEntity?>> BatSlctById(
		IDbFnCtx Ctx, IAsyncEnumerable<TId> Ids
		,CT Ct
	);

	[Doc(@$"Batch select aggregate roots by ids; aggregate metadata should be registered in ITblMgr.AddAgg().")]
	public Task<IAsyncEnumerable<TAgg?>> BatSlctAggById<TAgg>(
		IDbFnCtx Ctx, IAsyncEnumerable<TId> Ids
		,CT Ct
	)
		where TAgg: class;


	

	
	[Doc(@$"
	assume you have MainEntity and AssetEntity, Each MainEntity Has Many AssetEntity,
	when you want to selete multi MainEntity with their respective AssetEntity,
	use this to avoid N+1 Query
	#Params(
		[],
		[logical Forein Key],
		[Options],
		[All Keys. we use `IN` inside to avoid N+1 Query
		null will be filtered off by code before being sent to db],
		[main entity member selector],
		[Table],
		[],
	)
	#Rtn[Dict of Key map to multi OneToMany entitys]
	")]
	public Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey?> Keys
		,Func<TPo, TKey> FnMemb
		,ITable Tbl
		,CT Ct
	)where TPo: new();


	public Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey> Keys
		,Func<TPo, TKey> FnMemb
		,ITable<TPo> Tbl //帶泛型
		,CT Ct
	)where TPo: new();
	
	
	public Task<IRespBatInsert> BatInsert(
		IDbFnCtx Ctx, IAsyncEnumerable<TEntity> Ents, CT Ct
	);
	
	public Task<IRespBatUpd> BatUpdById(
		IDbFnCtx Ctx, IAsyncEnumerable<TEntity> Ents, CT Ct
	);
	
	[Doc(@$"
	#Params(
		[],
		[Dicts, Db Col Map to Raw Value, support;
		dicts with different key structure are allowed],
		[Ids, its count must equal to Dicts count],
		[],
	)
	#Examples([
	```cs
	
	```
	])
	")]
	public Task<IRespBatUpd> BatUpdByDbDict(
		IDbFnCtx Ctx
		,IAsyncEnumerable<TId> Ids
		,IAsyncEnumerable<IStr_Any> Dicts
		,CT Ct
	);
	
	[Doc(@$"
	#Params(
		[],
		[Dicts, Code Col(Entity Field) Map to Upper Value(Entity member), support;
		dicts with different key structure are allowed],
		[Ids, its count must equal to Dicts count],
		[],
	)
	#Examples([
	```cs
	
	```
	])
	")]
	public Task<IRespBatUpd> BatUpdByCodeDict(
		IDbFnCtx Ctx
		,IAsyncEnumerable<TId> Ids
		,IAsyncEnumerable<IStr_Any> Dicts
		,CT Ct
	);
	
	public Task<IBatSoftDel> BatSoftDelById(
		IDbFnCtx Ctx, IAsyncEnumerable<TId> Ids, CT Ct
	);
	
	public Task<IBatHardDel> BatHardDelById(
		IDbFnCtx Ctx, IAsyncEnumerable<TId> Ids, CT Ct
	);

}



public class IRespBatInsert{
	
}

public class RespBatInsert:IRespBatInsert{}


public class IRespBatUpd{
	
}
public class RespUpd:IRespBatUpd{
	
}

public class IBatSoftDel{}

public class BatSoftDel:IBatSoftDel{}

public class IBatHardDel{}

public class BatHardDel:IBatHardDel{}

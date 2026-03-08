namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsPage;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;

[Doc($@"
#Sum[Repository interface for database common operations on entities]
#TParams([Entity type],[Entity ID type])
")]
public partial interface IRepo<TEntity, TId>{
	
	

	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertMany(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		IAsyncEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertAsyE(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		TEntity
		,CT
		,Task<nil>
	>> FnInsertOne(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		CT
		,Task<u64>
	>> FnCount(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		TId
		,CT
		,Task<TEntity?>
	>> FnSlctOneById(
		IDbFnCtx Ctx
		,CT Ct
	);

	[Doc(@$"using `Id IN (...)` Clause,
	which would ignore unexisted Id and returned list may be unordered.
	")]
	public Task<IAsyncEnumerable<TEntity?>> SlctManyInIdsWithDel(
		IDbFnCtx Ctx, IEnumerable<TId> Ids
		,CT Ct
	);

	[Doc(@$"Got Entities are corresponding to the given Ids. if not found, the place will be null.")]
	public Task<IAsyncEnumerable<TEntity?>> BatSlctById(
		IDbFnCtx Ctx, IEnumerable<TId> Ids
		,CT Ct
	);

	[Doc(@$"Batch select aggregate roots by ids; aggregate metadata should be registered in ITblMgr.AddAgg().")]
	public Task<IAsyncEnumerable<TAgg?>> BatSlctAggByIds<TAgg>(
		IDbFnCtx Ctx, IEnumerable<TId> Ids
		,CT Ct
	)
		where TAgg: class;


	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<IDictionary<str, obj?>>>
	>> FnPageAllDict(IDbFnCtx Ctx, CT Ct);


	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<TEntity>>
	>> FnPageAll(IDbFnCtx Ctx, CT Ct);


	[Obsolete]
	public Task<Func<
		TId
		,TEntity
		,CT, Task<nil>
	>> FnUpdByIdOld(
		IDbFnCtx Ctx
		,IEnumerable<str>? FieldsToUpdate
		,CT Ct
	);


	[Obsolete]
	public Task<Func<
		IEnumerable<Id_Dict<TId>>
		,CT
		,Task<nil>
	>> FnUpdManyByIdOld(
		IDbFnCtx Ctx
		,IEnumerable<str> FieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		TEntity
		,CT, Task<nil>
	>> FnUpdOneById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnUpdManyById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		IAsyncEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnAsyEUpdManyById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		IEnumerable<obj?>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys(
		IDbFnCtx Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	);


	public Task<Func<
		IEnumerable<TKey>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys<TKey>(
		IDbFnCtx Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	);


	/// 不預編譯。適用于況芝 在事務中 初建表後即添數據
	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertManyNoPrepare(
		IDbFnCtx Ctx
		,CT ct
	);


	public Task<Func<
		TId
		,obj?
		,CT
		,Task<nil>
	>> FnUpdOneColById(
		IDbFnCtx Ctx
		,str Col
		,CT Ct
	);


	public Task<Func<
		IList<TVal>
		,CT
		,Task<IAsyncEnumerable<IStr_Any>>
	>> FnScltAllByColInVals<TVal>(
		IDbFnCtx Ctx
		,ITable Tbl
		,str CodeCol
		,OptQry? OptQry
		,CT Ct
	);

	
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


	public Task<Func<
		IList<TCol>
		,CT
		,Task<IAsyncEnumerable<TEntity2>>
	>> FnScltAllByColInVals<TEntity2, TCol>(
		IDbFnCtx Ctx
		,ITable Tbl
		,str CodeCol
		,OptQry? OptQry
		,CT Ct
	)
		where TEntity2 : class, new();

}

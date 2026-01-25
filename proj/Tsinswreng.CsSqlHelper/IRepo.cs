namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsPage;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;

[Doc($@"
#Sum[Repository interface for database operations on entities]
#TParams([Entity type],[Entity ID type])
")]
public partial interface IRepo<TEntity, TId>{
	[Doc($@"
#Sum[Create a function to insert multiple entities]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes entities and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertMany(
		IDbFnCtx Ctx
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to insert entities from async enumerable]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes async enumerable of entities and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		IAsyncEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertAsyE(
		IDbFnCtx Ctx
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to insert a single entity]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes entity and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		TEntity
		,CT
		,Task<nil>
	>> FnInsertOne(
		IDbFnCtx Ctx
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to count entities]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes cancellation token, returns task with count]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		CT
		,Task<u64>
	>> FnCount(
		IDbFnCtx Ctx
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to select one entity by ID]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes ID and cancellation token, returns task with entity or null]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		TId
		,CT
		,Task<TEntity?>
	>> FnSlctOneById(
		IDbFnCtx Ctx
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to page all entities as dictionaries]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes page query and cancellation token, returns task with paged dictionary results]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<IDictionary<str, obj?>>>
	>> FnPageAllDict(IDbFnCtx Ctx, CT Ct);

	[Doc($@"
#Sum[Create a function to page all entities]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes page query and cancellation token, returns task with paged entity results]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<TEntity>>
	>> FnPageAll(IDbFnCtx Ctx, CT Ct);

	[Doc($@"
#Sum[Create a function to update entity by ID (old method)]
#Params([Database function context],[Fields to update],[Cancellation token])
#Rtn[Function that takes ID, entity and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
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

	[Doc($@"
#Sum[Create a function to update multiple entities by ID (old method)]
#Params([Database function context],[Fields to update],[Cancellation token])
#Rtn[Function that takes ID-dictionary enumerable and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
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

	[Doc($@"
#Sum[Create a function to update one entity by ID]
#Params([Database function context],[Fields to update],[Cancellation token])
#Rtn[Function that takes entity and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		TEntity
		,CT, Task<nil>
	>> FnUpdOneById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to update multiple entities by ID]
#Params([Database function context],[Fields to update],[Cancellation token])
#Rtn[Function that takes entity enumerable and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnUpdManyById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to update multiple entities by ID from async enumerable]
#Params([Database function context],[Fields to update],[Cancellation token])
#Rtn[Function that takes async enumerable of entities and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
	public Task<Func<
		IAsyncEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnAsyEUpdManyById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);

	[Doc($@"
#Sum[Create a function to soft delete multiple entities by keys]
#Params([Database function context],[Key name in code],[Count per batch],[Cancellation token])
#Rtn[Function that takes key enumerable and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
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

	[Doc($@"
#Sum[Create a function to soft delete multiple entities by typed keys]
#Params([Database function context],[Key name in code],[Count per batch],[Cancellation token])
#Rtn[Function that takes typed key enumerable and cancellation token, returns task]
#TParams([Entity type],[Entity ID type],[Key type])
")]
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

	[Doc($@"
#Sum[Create a function to insert multiple entities without preparation]
#Params([Database function context],[Cancellation token])
#Rtn[Function that takes entities and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
	/// 不預編譯。適用于況芝 在事務中 初建表後即添數據
	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertManyNoPrepare(
		IDbFnCtx Ctx
		,CT ct
	);

	[Doc($@"
#Sum[Create a function to update one column by ID]
#Params([Database function context],[Column name],[Cancellation token])
#Rtn[Function that takes ID, target value and cancellation token, returns task]
#TParams([Entity type],[Entity ID type])
")]
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

	[Doc($@"
#Sum[Create a function to select all entities by column values]
#Params([Database function context],[Table],[Column name],[Query options],[Cancellation token])
#Rtn[Function that takes column value list and cancellation token, returns task with async enumerable of dictionaries]
#TParams([Entity type],[Entity ID type],[Column value type])
")]
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

	[Doc($@"
#Sum[Include related entities by keys]
#Params([Database function context],[Column name],[Query options],[Keys],[Member function],[Table],[Cancellation token])
#Rtn[Task with dictionary mapping keys to entity lists]
#TParams([Entity type],[Entity ID type],[Related entity type],[Key type])
")]
	public Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey> Keys
		,Func<TPo, TKey> FnMemb
		,ITable Tbl
		,CT Ct
	)where TPo: new();

	[Doc($@"
#Sum[Include related entities by keys with typed table]
#Params([Database function context],[Column name],[Query options],[Keys],[Member function],[Typed table],[Cancellation token])
#Rtn[Task with dictionary mapping keys to entity lists]
#TParams([Entity type],[Entity ID type],[Related entity type],[Key type])
")]
	public Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey> Keys
		,Func<TPo, TKey> FnMemb
		,ITable<TPo> Tbl
		,CT Ct
	)where TPo: new();

	[Doc($@"
#Sum[Create a function to select entities by column values with mapping]
#Params([Database function context],[Table],[Column name],[Query options],[Cancellation token])
#Rtn[Function that takes column value list and cancellation token, returns task with async enumerable of mapped entities]
#TParams([Entity type],[Entity ID type],[Result entity type],[Column value type])
")]
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

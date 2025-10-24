namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsPage;
using IDbFnCtx = IBaseDbFnCtx;
public partial interface IRepo<TEntity, TId>{
	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertMany(
		IDbFnCtx? Ctx
		,CT Ct
	);

	public Task<Func<
		TEntity
		,CT
		,Task<nil>
	>> FnInsertOne(
		IDbFnCtx? Ctx
		,CT Ct
	);


	public Task<Func<
		CT
		,Task<u64>
	>> FnCount(
		IDbFnCtx? Ctx
		,CT Ct
	);

	public Task<Func<
		TId
		,CT
		,Task<TEntity?>
	>> FnSlctById(
		IDbFnCtx? Ctx
		,CT Ct
	);


	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<IDictionary<str, obj?>>>
	>> FnPageAllDict(IDbFnCtx? Ctx, CT Ct);

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
		IDbFnCtx? Ctx
		,IEnumerable<str>? FieldsToUpdate
		,CT Ct
	);

	[Obsolete]
	public Task<Func<
		IEnumerable<Id_Dict<TId>>
		,CT
		,Task<nil>
	>> FnUpdManyByIdOld(
		IDbFnCtx? Ctx
		,IEnumerable<str> FieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		TEntity
		,CT, Task<nil>
	>> FnUpdOneById(
		IDbFnCtx? Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);

	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnUpdManyById(
		IDbFnCtx? Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		IEnumerable<obj?>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys(
		IDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	);


	public Task<Func<
		IEnumerable<TKey>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys<TKey>(
		IDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	);

/// <summary>
/// 不預編譯。適用于況芝 在事務中 初建表後即添數據
/// </summary>
/// <param name="Ctx"></param>
/// <param name="ct"></param>
/// <returns></returns>
	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertManyNoPrepare(
		IDbFnCtx? Ctx
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



}

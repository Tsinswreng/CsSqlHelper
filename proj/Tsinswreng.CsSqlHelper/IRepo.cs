namespace Tsinswreng.CsSqlHelper;

using IDbFnCtx = IBaseDbFnCtx;
public interface IRepo<TEntity, TId>{


	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertMany(
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
	>> FnSelectById(
		IDbFnCtx? Ctx
		,CT Ct
	);

	public Task<Func<
		IEnumerable<Id_Dict<TId>>
		,CT
		,Task<nil>
	>> FnUpdateManyById(
		IDbFnCtx? Ctx
		,IDictionary<str, object?> ModelDict //不當有Id
		,CT Ct
	);








}

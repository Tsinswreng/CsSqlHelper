

namespace Tsinswreng.CsSqlHelper.EFCore;

public class EfRepo<TEntity, TId>
	: IRepo<TEntity, TId>
{
	public async Task<Func<CT, Task<ulong>>> FnCount(IBaseDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<Func<IEnumerable<TEntity>, CT, Task<object>>> FnInsertMany(IBaseDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public async Task<Func<TId, CT, Task<TEntity?>>> FnSelectById(IBaseDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public async Task<Func<IEnumerable<Id_Dict<TId>>, CT, Task<object>>> FnUpdateManyById(IBaseDbFnCtx? Ctx, IDictionary<string, object?> ModelDict, CT Ct) {
		throw new NotImplementedException();
	}
}

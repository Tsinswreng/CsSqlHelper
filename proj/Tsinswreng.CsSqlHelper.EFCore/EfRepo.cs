

using Microsoft.EntityFrameworkCore;

namespace Tsinswreng.CsSqlHelper.EFCore;

public  partial class EfRepo<TEntity, TId>
	: IRepo<TEntity, TId>
	where TEntity:class
{
	public DbContext EfDbCtx{get;set;}
	public EfRepo(DbContext EfDbCtx) {
		this.EfDbCtx = EfDbCtx;
	}

	public async Task<Func<CT, Task<ulong>>> FnCount(IBaseDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public async Task<Func<
		IEnumerable<TEntity>, CT, Task<nil>
	>> FnInsertMany(IBaseDbFnCtx? Ctx, CT Ct) {
		var Fn = async(IEnumerable<TEntity> Entitys, CT Ct)=>{
			await EfDbCtx.Set<TEntity>().AddRangeAsync(Entitys, Ct);
			await EfDbCtx.SaveChangesAsync(Ct);
			return NIL;
		};
		return Fn;
	}

	public async Task<Func<TId, CT, Task<TEntity?>>> FnSelectById(IBaseDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public async Task<Func<IEnumerable<Id_Dict<TId>>, CT, Task<object>>> FnUpdateManyById(IBaseDbFnCtx? Ctx, IDictionary<string, object?> ModelDict, CT Ct) {
		throw new NotImplementedException();
	}
}

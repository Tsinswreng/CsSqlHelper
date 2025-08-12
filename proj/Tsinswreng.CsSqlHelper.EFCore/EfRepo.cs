

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

	public async Task<Func<TId, CT, Task<TEntity?>>> FnSlctById(IBaseDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public async Task<Func<IEnumerable<Id_Dict<TId>>, CT, Task<object>>> FnUpdManyById(
		IBaseDbFnCtx? Ctx
		,IEnumerable<str> FieldsToUpdate
		,CT Ct
	){
		throw new NotImplementedException();
	}

	public Task<Func<
		IEnumerable<obj?>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys(
		IBaseDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	){
		throw new NotImplementedException();
	}

	public Task<Func<
		IEnumerable<TKey>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys<TKey>(
		IBaseDbFnCtx? Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	){
		throw new NotImplementedException();
	}

	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertManyNoPrepare(
		IBaseDbFnCtx? Ctx
		,CT ct
	){
		throw new NotImplementedException();
	}

}

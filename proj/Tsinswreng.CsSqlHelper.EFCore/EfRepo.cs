namespace Tsinswreng.CsSqlHelper.EFCore;

using Microsoft.EntityFrameworkCore;
using Tsinswreng.CsPage;
using IDbFnCtx = IBaseDbFnCtx;

public partial class EfRepo<TEntity, TId>
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

	public async Task<Func<TId, CT, Task<TEntity?>>> FnSlctOneById(IBaseDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public async Task<Func<IEnumerable<Id_Dict<TId>>, CT, Task<object>>> FnUpdManyByIdOld(
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

	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<IDictionary<str, obj?>>>
	>> FnPageAllDict(IDbFnCtx? Ctx, CT Ct){
		throw new NotImplementedException();
	}

	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<TEntity>>
	>> FnPageAll(IDbFnCtx Ctx, CT Ct){
		throw new NotImplementedException();
	}

	public Task<Func<TId, object?, CT, Task<object>>> FnUpdOneColById(IDbFnCtx Ctx, string Col, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<Func<TEntity, CT, Task<object>>> FnInsertOne(IDbFnCtx? Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<Func<TId, TEntity, CT, Task<object>>> FnUpdByIdOld(IDbFnCtx? Ctx, IEnumerable<string>? FieldsToUpdate, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<Func<TEntity, CT, Task<object>>> FnUpdOneById(IDbFnCtx? Ctx, IEnumerable<string>? FieldsToUpdate, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<Func<IEnumerable<TEntity>, CT, Task<object>>> FnUpdManyById(IDbFnCtx? Ctx, IEnumerable<string>? FieldsToUpdate, CT Ct) {
		throw new NotImplementedException();
	}
}

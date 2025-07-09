using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
namespace Tsinswreng.CsSqlHelper.EFCore;


public class EfTxn:ITxn{
	public EfTxn(IDbContextTransaction _RawTxn){
		this._RawTxn = _RawTxn;
	}
	public object? RawTxn{get;}
	IDbContextTransaction _RawTxn;
	public async Task<nil> Begin(CT Ct){
		return NIL;
	}
	public async Task<nil> Commit(CT Ct){
		await _RawTxn.CommitAsync();
		return NIL;
	}
	public async Task<nil> Rollback(CT Ct){
		await _RawTxn.RollbackAsync();
		return NIL;
	}
	public void Dispose(){
		_RawTxn.Dispose();
	}


}

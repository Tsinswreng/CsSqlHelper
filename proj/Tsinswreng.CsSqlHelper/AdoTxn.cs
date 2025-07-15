using System.Data;

namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// ADO.NET 事務
/// </summary>
public class AdoTxn:ITxn{
	public AdoTxn(IDbTransaction _RawTxn){
		this._RawTxn = _RawTxn;
	}
	public object? RawTxn{get;}
	IDbTransaction _RawTxn;
	public async Task<nil> Begin(CT Ct){
		return NIL;
	}
	public async Task<nil> Commit(CT Ct){
		_RawTxn.Commit();
		return NIL;
	}
	public async Task<nil> Rollback(CT Ct){
		_RawTxn.Rollback();
		return NIL;
	}
	public void Dispose(){
		_RawTxn.Dispose();
	}
}

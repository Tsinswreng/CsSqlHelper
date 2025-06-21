using System.Data;
using Tsinswreng.CsSqlHelper.Db;

namespace Tsinswreng.CsSqlHelper.Cmd;

public class AdoTxn:ITxn{
	public AdoTxn(IDbTransaction _RawTxn){
		this._RawTxn = _RawTxn;
	}
	public object? RawTxn{get;}
	IDbTransaction _RawTxn;
	public async Task<nil> Begin(CancellationToken Ct){
		return NIL;
	}
	public async Task<nil> Commit(CancellationToken Ct){
		_RawTxn.Commit();
		return NIL;
	}
	public async Task<nil> Rollback(CancellationToken Ct){
		_RawTxn.Rollback();
		return NIL;
	}
	public void Dispose(){
		_RawTxn.Dispose();
	}
}

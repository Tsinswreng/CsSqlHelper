using System.Data;
using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper.Cmd;

public class AdoTxn:ITxn{
	public AdoTxn(IDbTransaction _RawTxn){
		this._RawTxn = _RawTxn;
	}
	public object? RawTxn{get;}
	IDbTransaction _RawTxn;
	public async Task<nil> Begin(CancellationToken Ct){
		return Nil;
	}
	public async Task<nil> Commit(CancellationToken Ct){
		_RawTxn.Commit();
		return Nil;
	}
	public async Task<nil> Rollback(CancellationToken Ct){
		_RawTxn.Rollback();
		return Nil;
	}
	public void Dispose(){
		_RawTxn.Dispose();
	}
}

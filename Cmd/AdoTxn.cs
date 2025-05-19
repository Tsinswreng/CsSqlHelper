using System.Data;
using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper.Cmd;

public class AdoTxn:I_TxnAsy{
	public AdoTxn(IDbTransaction _RawTxn){
		this._RawTxn = _RawTxn;
	}
	public object? RawTxn{get;}
	IDbTransaction _RawTxn;
	public async Task<nil> BeginAsy(CancellationToken Ct){
		return Nil;
	}
	public async Task<nil> CommitAsy(CancellationToken Ct){
		_RawTxn.Commit();
		return Nil;
	}
	public async Task<nil> RollbackAsy(CancellationToken Ct){
		_RawTxn.Rollback();
		return Nil;
	}
	public void Dispose(){
		_RawTxn.Dispose();
	}
}

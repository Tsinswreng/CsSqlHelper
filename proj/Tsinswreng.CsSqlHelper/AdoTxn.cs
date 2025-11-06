using System.Data;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// ADO.NET 事務
/// </summary>
public partial class AdoTxn:ITxn{
	public AdoTxn(IDbTransaction _RawTxn){
		this._RawTxn = _RawTxn;
		this.RawTxn = _RawTxn;
	}

	[Impl]
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

namespace Tsinswreng.CsSqlHelper;

[Doc(@$"Maker of Database Function Context")]
public partial interface IMkrDbFnCtx{
	public IMkrTxn TxnGetter{get;set;}
	[Doc(@$"Make Database Function Context with Transaction attached
	For Sqlite, Do not use for select statement, or it may cause nested transaction error.
	You just need `new {nameof(DbFnCtx)}` for select statement.
	")]
	public async Task<IDbFnCtx> MkTxnDbFnCtx(CT Ct){
		var R = new DbFnCtx();
		R.Txn = await TxnGetter.MkTxn(R, Ct);
		return R;
	}
}

/// <summary>
/// 2026-01-09T23:27:03.740+08:00_W2-5{
/*
泛型潙舊設計、初慮及 自ʹ業務程序集 可自建DbFnCtx子類㕥做擴展
肰泛型大增繁。 如需擴展 則于業務程序集中 潙基ʹIDbFnCtx接口作擴展方法 㕥轉型作己ʹ擴展ʹ子類
不建議用泛型
 */
/// }
/// </summary>
/// <typeparam name="TDbFnCtx"></typeparam>
[Obsolete]
public partial interface IMkrDbFnCtx<TDbFnCtx>
	where TDbFnCtx: IDbFnCtx, new()
{
	public IMkrTxn TxnGetter{get;set;}
	public async Task<TDbFnCtx> MkTxnDbFnCtxAsy(CT Ct){
		var R = new TDbFnCtx();
		R.Txn = await TxnGetter.MkTxn(R, Ct);
		return R;
	}
}

public partial class MkrDbFnCtx<TDbFnCtx>
	:IMkrDbFnCtx<TDbFnCtx>
	where TDbFnCtx: IDbFnCtx, new()
{
	public IMkrTxn TxnGetter{get;set;}
	public MkrDbFnCtx(IMkrTxn GetTxn){
		this.TxnGetter = GetTxn;
	}
}

public partial class MkrDbFnCtx:MkrDbFnCtx<DbFnCtx>, IMkrDbFnCtx{
	public MkrDbFnCtx(IMkrTxn GetTxn):base(GetTxn){

	}
}

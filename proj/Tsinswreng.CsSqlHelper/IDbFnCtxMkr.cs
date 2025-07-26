namespace Tsinswreng.CsSqlHelper;

public  partial interface IDbFnCtxMkr<TDbFnCtx>
	where TDbFnCtx: IBaseDbFnCtx, new()
{
	public I_GetTxnAsy TxnGetter{get;set;}
	public async Task<TDbFnCtx> MkTxnDbFnCtxAsy(CT Ct){
		var R = new TDbFnCtx();
		R.Txn = await TxnGetter.GetTxnAsy(Ct);
		return R;
	}
}

public  partial class BaseDbFnCtxMkr<TDbFnCtx>
	:IDbFnCtxMkr<TDbFnCtx>
	where TDbFnCtx: IBaseDbFnCtx, new()
{
	public I_GetTxnAsy TxnGetter{get;set;}
	public BaseDbFnCtxMkr(I_GetTxnAsy GetTxn){
		this.TxnGetter = GetTxn;
	}
}

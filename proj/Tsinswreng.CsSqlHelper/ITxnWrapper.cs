namespace Tsinswreng.CsSqlHelper;
using IDbFnCtx = IBaseDbFnCtx;

public  partial class TxnWrapper<TDbFnCtx>
	where TDbFnCtx: IDbFnCtx, new()
{
	public IDbFnCtxMkr<TDbFnCtx> DbFnCtxMkr{get;set;}
	public ITxnRunner TxnRunner{get;set;}
	public TxnWrapper(
		IDbFnCtxMkr<TDbFnCtx> DbFnCtxMkr
		,ITxnRunner TxnRunner
	){
		this.DbFnCtxMkr = DbFnCtxMkr;
		this.TxnRunner = TxnRunner;
	}
	//無參(除末ʹCt外)
	//delegate Task<TRet> _0Arg<TRet>(out TDbFnCtx DbFnCtx, CT Ct, );
	public async Task<TRet> Wrap<TRet>(
		Func<TDbFnCtx, CT, Task<Func<
			CT
			,Task<TRet>
		>>> FnXxx
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var Xxx = await FnXxx(Ctx, Ct);
		var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
			return await Xxx(Ct);
		}, Ct);
		return R;
	}
	//1
	public async Task<TRet> Wrap<TArg0, TRet>(
		Func<TDbFnCtx, CT, Task<Func<
			TArg0
			,CT
			,Task<TRet>
		>>> FnXxx
		,TArg0 Arg0
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var Xxx = await FnXxx(Ctx, Ct);
		var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
			return await Xxx(Arg0, Ct);
		}, Ct);
		return R;
	}
	//2
	public async Task<TRet> Wrap<TArg0, TArg1, TRet>(
		Func<TDbFnCtx, CT, Task<Func<
			TArg0
			,TArg1
			,CT
			,Task<TRet>
		>>> FnXxx
		,TArg0 Arg0
		,TArg1 Arg1
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var Xxx = await FnXxx(Ctx, Ct);
		var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
			return await Xxx(Arg0, Arg1, Ct);
		}, Ct);
		return R;
	}

	//3
	public async Task<TRet> Wrap<TArg0, TArg1, TArg2, TRet>(
		Func<TDbFnCtx, CT, Task<Func<
			TArg0
			,TArg1
			,TArg2
			,CT
			,Task<TRet>
		>>> FnXxx
		,TArg0 Arg0
		,TArg1 Arg1
		,TArg2 Arg2
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var Xxx = await FnXxx(Ctx, Ct);
		var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
			return await Xxx(Arg0, Arg1, Arg2, Ct);
		}, Ct);
		return R;
	}



}

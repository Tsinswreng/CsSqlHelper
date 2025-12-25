namespace Tsinswreng.CsSqlHelper;
using IDbFnCtx = IBaseDbFnCtx;

public partial class TxnWrapper<TDbFnCtx>
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


//------

	//無參(除末ʹCt外)
	//delegate Task<TRet> _0Arg<TRet>(out TDbFnCtx DbFnCtx, CT Ct, );
	public async Task<TRet> Wrap<TRet>(
		Func<TDbFnCtx, CT, Task<Func<
			CT
			,Task<TRet>
		>>> FnXxx
		,CT Ct
	){
		TDbFnCtx Ctx = default!;
		try{
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch{
			if(Ctx is not null){
				await Ctx.Txn!.Rollback(Ct);
				await Ctx.DisposeAsync();
			}
			throw;
		}
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
		TDbFnCtx Ctx = default!;
		try{
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Arg0, Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch{
			if(Ctx is not null){
				await Ctx.Txn!.Rollback(Ct);
				await Ctx.DisposeAsync();
			}
			throw;
		}
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
		TDbFnCtx Ctx = default!;
		try{
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Arg0, Arg1, Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch{
			if(Ctx is not null){
				await Ctx.Txn!.Rollback(Ct);
				await Ctx.DisposeAsync();
			}
			throw;
		}
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
		TDbFnCtx Ctx = default!;
		try{
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Arg0, Arg1, Arg2, Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch{
			if(Ctx is not null){
				await Ctx.Txn!.Rollback(Ct);
				await Ctx.DisposeAsync();
			}
			throw;
		}
	}

#if false

	public async Task<TRtn> Wrap<TRtn>(
		DbFn<TRtn> DbFn
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var FnRun = DbFn.FnRun;
		var R = await FnRun(Ct);
		await DbFn.DisposeAsync();
		return R;
	}

	public async Task<TRtn> Wrap<TArg0, TRtn>(
		DbFn<TArg0, TRtn> DbFn
		,TArg0 Arg0
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var FnRun = DbFn.FnRun;
		var R = await FnRun(Arg0, Ct);
		await DbFn.DisposeAsync();
		return R;
	}

	public async Task<TRtn> Wrap<TArg0, TArg1, TRtn>(
		DbFn<TArg0, TArg1, TRtn> DbFn
		,TArg0 Arg0
		,TArg1 Arg1
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var FnRun = DbFn.FnRun;
		var R = await FnRun(Arg0, Arg1, Ct);
		await DbFn.DisposeAsync();
		return R;
	}

	public async Task<TRtn> Wrap<TArg0, TArg1, TArg2, TRtn>(
		DbFn<TArg0, TArg1, TArg2, TRtn> DbFn
		,TArg0 Arg0
		,TArg1 Arg1
		,TArg2 Arg2
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var FnRun = DbFn.FnRun;
		var R = await FnRun(Arg0, Arg1, Arg2, Ct);
		await DbFn.DisposeAsync();
		return R;
	}

	public async Task<TRtn> Wrap<TArg0, TArg1, TArg2, TArg3, TRtn>(
		DbFn<TArg0, TArg1, TArg2, TArg3, TRtn> DbFn
		,TArg0 Arg0
		,TArg1 Arg1
		,TArg2 Arg2
		,TArg3 Arg3
		,CT Ct
	){
		var Ctx = await DbFnCtxMkr.MkTxnDbFnCtxAsy(Ct);
		var FnRun = DbFn.FnRun;
		var R = await FnRun(Arg0, Arg1, Arg2, Arg3, Ct);
		await DbFn.DisposeAsync();
		return R;
	}
#endif


}

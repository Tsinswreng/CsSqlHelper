namespace Tsinswreng.CsSql;

using TDbFnCtx = IDbFnCtx;
[Obsolete]
public partial class TxnWrapper{
	public IMkrDbFnCtx DbFnCtxMkr{get;set;}
	public ITxnRunner TxnRunner{get;set;}
	public TxnWrapper(
		IMkrDbFnCtx DbFnCtxMkr
		,ITxnRunner TxnRunner
	){
		this.DbFnCtxMkr = DbFnCtxMkr;
		this.TxnRunner = TxnRunner;
	}

	Func<Exception, nil> OnErr = (e)=>null!;
	async Task<nil> Rollback(IDbFnCtx Ctx, CT Ct){
		await Ctx.Txn!.Rollback(OnErr, Ct);
		return NIL;
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
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtx(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch(Exception Ex){
			try{
				if(Ctx is not null){
					await Rollback(Ctx, Ct);
					await Ctx.DisposeAsync();
				}
			}catch(Exception Ex2){
				throw new AggregateException(Ex2, Ex);
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
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtx(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Arg0, Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch(Exception Ex){
			try{
				if(Ctx is not null){
					await Rollback(Ctx, Ct);
					await Ctx.DisposeAsync();
				}
			}catch(Exception Ex2){
				throw new AggregateException(Ex2, Ex);
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
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtx(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Arg0, Arg1, Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch(Exception Ex){
			try{
				if(Ctx is not null){
					await Rollback(Ctx, Ct);
					await Ctx.DisposeAsync();
				}
			}catch(Exception Ex2){
				throw new AggregateException(Ex2, Ex);
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
			Ctx = await DbFnCtxMkr.MkTxnDbFnCtx(Ct);
			var Xxx = await FnXxx(Ctx, Ct);
			var R = await TxnRunner.RunTxn(Ctx.Txn, async(Ct)=>{
				return await Xxx(Arg0, Arg1, Arg2, Ct);
			}, Ct);
			await Ctx.DisposeAsync();
			return R;
		}catch(Exception Ex){
			try{
				if(Ctx is not null){
					await Rollback(Ctx, Ct);
					await Ctx.DisposeAsync();
				}
			}catch(Exception Ex2){
				throw new AggregateException(Ex2, Ex);
			}

			throw;
		}
	}

}

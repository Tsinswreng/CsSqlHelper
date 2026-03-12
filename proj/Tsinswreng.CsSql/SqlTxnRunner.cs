using Tsinswreng.CsCore;

namespace Tsinswreng.CsSql;
public partial class AdoTxnRunner(
	//IDbConnection DbConnection
)
	//:IRunInTxn
	:ITxnRunner
{

	[Impl]
	public async Task<TRet> RunTxn<TRet>(
		ITxn? Txn
		,Func<
			CT, Task<TRet>
		> FnAsy
		,CT Ct
	){
		if(Txn == null){
			TRet R = await FnAsy(Ct);
			return R;
		}
		try{
			await Txn.Begin(Ct);
			TRet R = await FnAsy(Ct);
			await Txn.Commit(Ct);
			return R;
		}
		catch (Exception) {
			await Txn.Rollback(Ct);
			throw;
		}

		// using var Tx = DbConnection.BeginTransaction(IsolationLevel.Serializable);
		// var AdoTx = new AdoTxn(Tx);
		// //var Ctx = new DbFnCtx{Txn = AdoTx};
		// DbFnCtx.Txn = AdoTx;
		// try{
		// 	var ans = await FnAsy(DbFnCtx, ct);
		// 	Tx.Commit();
		// 	return ans;
		// }
		// catch (System.Exception){
		// 	Tx.Rollback();
		// 	throw;
		// }
	}
}

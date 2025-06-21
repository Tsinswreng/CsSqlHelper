using System.Data;
using Tsinswreng.CsSqlHelper;
using Tsinswreng.CsSqlHelper.Cmd;
using Tsinswreng.CsSqlHelper.Db;

namespace Ngaq.Local.Db;
public class SqlTxnRunner(
	IDbConnection DbConnection
)
	:IRunInTxn
	,ITxnRunner
{

	public async Task<TRet> RunInTxn<TRet>(
		Func<
			CT, Task<TRet>
		> FnAsy
		,CT ct
	){
		using var Tx = DbConnection.BeginTransaction(IsolationLevel.Serializable);
		try{
			var ans = await FnAsy(ct);

			Tx.Commit();
			return ans;
		}
		catch (Exception) {
			Tx.Rollback();
			throw;
		}
	}

	public async Task<TRet> RunTxn<TRet>(
		ITxn Txn
		,Func<
			CT, Task<TRet>
		> FnAsy
		,CT ct
	){
		try{
			await Txn.Begin(ct);
			TRet ans = await FnAsy(ct);
			await Txn.Commit(ct);
			return ans;
		}
		catch (Exception) {
			await Txn.Rollback(ct);
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

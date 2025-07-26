namespace Tsinswreng.CsSqlHelper.EFCore;

public  partial class EfTxnRunner:ITxnRunner{
	public async Task<TRet> RunTxn<TRet>(
		ITxn? Txn
		,Func<
			CT, Task<TRet>
		> FnAsy
		, CT Ct
	){
		if(Txn == null){
			TRet R = await FnAsy(Ct);
			return R;
		}
		try{
			await Txn.Begin(Ct);
			TRet ans = await FnAsy(Ct);
			await Txn.Commit(Ct);
			return ans;
		}
		catch (Exception) {
			await Txn.Rollback(Ct);
			throw;
		}
	}
}

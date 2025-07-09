namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// 事務
/// </summary>
public interface ITxn : IDisposable{
/// <summary>
/// 原始事務對象
/// </summary>
	public object? RawTxn{get;}
	public Task<nil> Begin(CT Ct);
	public Task<nil> Commit(CT Ct);
	public Task<nil> Rollback(CT Ct);
}



public static class ExtnITxn{
	public static async Task<TRet> RunTxn<TRet>(
		this ITxn Txn
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
	}
}

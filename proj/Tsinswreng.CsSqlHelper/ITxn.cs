namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// 事務
/// Transaction
/// </summary>
public interface ITxn : IDisposable{
/// <summary>
/// 原始事務對象
/// Raw transaction object
/// </summary>
	public object? RawTxn{get;}
	/// <summary>
	///
	/// </summary>
	/// <param name="Ct"></param>
	/// <returns>null</returns>
	public Task<nil> Begin(CT Ct);
	/// <summary>
	///
	/// </summary>
	/// <param name="Ct"></param>
	/// <returns>null</returns>
	public Task<nil> Commit(CT Ct);
	/// <summary>
	///
	/// </summary>
	/// <param name="Ct"></param>
	/// <returns>null</returns>
	public Task<nil> Rollback(CT Ct);
}


public static class ExtnITxn{
	/// <summary>
	/// run a function in a transaction
	/// </summary>
	/// <typeparam name="TRet"></typeparam>
	/// <param name="Txn"></param>
	/// <param name="FnAsy"></param>
	/// <param name="Ct"></param>
	/// <returns></returns>
	public static async Task<TRet> RunTxn<TRet>(
		this ITxn Txn
		,Func<
			CT, Task<TRet>
		> FnAsy
		,CT Ct
	){
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
	}
}

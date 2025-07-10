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

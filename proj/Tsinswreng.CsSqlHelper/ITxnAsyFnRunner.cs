namespace Tsinswreng.CsSqlHelper;

// [Obsolete]
// public interface IRunInTxn{
// 	// [Obsolete]
// 	// public Task<TRet> RunInTxn<TRet>(
// 	// 	Func<CT, Task<TRet>> FnAsy
// 	// 	, CT ct
// 	// );
// }


public interface ITxnRunner{
	public Task<TRet> RunTxn<TRet>(
		ITxn? Txn
		,Func<
			CT, Task<TRet>
		> FnAsy
		, CT ct
	);
}


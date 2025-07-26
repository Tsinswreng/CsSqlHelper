namespace Tsinswreng.CsSqlHelper;

// [Obsolete]
// public  partial interface IRunInTxn{
// 	// [Obsolete]
// 	// public Task<TRet> RunInTxn<TRet>(
// 	// 	Func<CT, Task<TRet>> FnAsy
// 	// 	, CT ct
// 	// );
// }


public  partial interface ITxnRunner{
	public Task<TRet> RunTxn<TRet>(
		ITxn? Txn
		,Func<
			CT, Task<TRet>
		> FnAsy
		, CT ct
	);
}


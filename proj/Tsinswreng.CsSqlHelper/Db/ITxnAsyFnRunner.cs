//using Tsinswreng.CsSqlHelper;
//using Tsinswreng.CsSqlHelper;
using Tsinswreng.CsSqlHelper;

namespace Tsinswreng.CsSqlHelper.Db;

public interface IRunInTxn{
	public Task<TRet> RunInTxn<TRet>(
		Func<CT, Task<TRet>> FnAsy
		, CT ct
	);
}


public interface ITxnRunner{
	public Task<TRet> RunTxn<TRet>(
		ITxn Txn
		,Func<
			CT, Task<TRet>
		> FnAsy
		, CT ct
	);
}

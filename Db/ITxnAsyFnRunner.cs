//using Tsinswreng.SqlHelper;
using Tsinswreng.SqlHelper;

namespace Ngaq.Core.Infra.Db;

public interface IRunInTxn{
	public Task<T_Ret> RunInTxnAsy<T_Ret>(
		Func<CancellationToken, Task<T_Ret>> FnAsy
		,CancellationToken ct
	);
}


public interface ITxnRunner{
	public Task<T_Ret> RunTxnAsy<T_Ret>(
		ITxnAsy Txn
		,Func<
			CancellationToken, Task<T_Ret>
		> FnAsy
		,CancellationToken ct
	);
}

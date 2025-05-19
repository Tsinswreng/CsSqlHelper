//using Tsinswreng.SqlHelper;
using Tsinswreng.SqlHelper;

namespace Ngaq.Core.Infra.Db;

public interface I_RunInTxn{
	public Task<T_Ret> RunInTxnAsy<T_Ret>(
		Func<CancellationToken, Task<T_Ret>> FnAsy
		,CancellationToken ct
	);
}


public interface I_TxnRunner{
	public Task<T_Ret> RunTxnAsy<T_Ret>(
		I_TxnAsy Txn
		,Func<
			CancellationToken, Task<T_Ret>
		> FnAsy
		,CancellationToken ct
	);
}

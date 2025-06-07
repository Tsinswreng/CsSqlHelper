//using Tsinswreng.CsSqlHelper;
using Tsinswreng.CsSqlHelper;

namespace Ngaq.Core.Infra.Db;

public interface IRunInTxn{
	public Task<T_Ret> RunInTxn<T_Ret>(
		Func<CancellationToken, Task<T_Ret>> FnAsy
		,CancellationToken ct
	);
}


public interface ITxnRunner{
	public Task<T_Ret> RunTxn<T_Ret>(
		ITxn Txn
		,Func<
			CancellationToken, Task<T_Ret>
		> FnAsy
		,CancellationToken ct
	);
}

using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper.Cmd;

public interface I_GetTxnAsy{
	public Task<I_TxnAsy> GetTxnAsy();
}

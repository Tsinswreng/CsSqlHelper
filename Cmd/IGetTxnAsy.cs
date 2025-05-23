using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper.Cmd;

public interface IGetTxnAsy{
	public Task<ITxnAsy> GetTxnAsy();
}

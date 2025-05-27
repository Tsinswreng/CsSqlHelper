using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper.Cmd;

public interface IGetTxn{
	public Task<ITxn> GetTxn();
}

using Tsinswreng.CsSqlHelper.Db;

namespace Tsinswreng.CsSqlHelper.Cmd;

public interface IGetTxn{
	public Task<ITxn> GetTxn();
}

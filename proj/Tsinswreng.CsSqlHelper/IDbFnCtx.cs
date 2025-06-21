using Tsinswreng.CsSqlHelper.Db;

namespace Tsinswreng.CsSqlHelper;

public interface IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
}

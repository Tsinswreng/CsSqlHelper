using Ngaq.Core.Infra.Db;

namespace Tsinswreng.CsSqlHelper;

public class BaseDbFnCtx:IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
}

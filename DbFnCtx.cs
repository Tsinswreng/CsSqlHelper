using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper;

public class BaseDbFnCtx:IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
}

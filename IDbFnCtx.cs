using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper;

public interface IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
}

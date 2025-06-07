using Ngaq.Core.Infra.Db;

namespace Tsinswreng.CsSqlHelper;

public interface IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
}

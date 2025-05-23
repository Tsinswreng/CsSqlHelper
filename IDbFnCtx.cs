using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper;

public interface IDbFnCtx{
	public ITxnAsy? Txn{get;set;}
}

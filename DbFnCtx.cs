using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper;

public class DbFnCtx:IDbFnCtx{
	public ITxnAsy? Txn{get;set;}
}

using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper;

public class DbFnCtx:I_DbFnCtx{
	public I_TxnAsy? Txn{get;set;}
}

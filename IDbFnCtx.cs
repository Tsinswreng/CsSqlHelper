using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper;

public interface I_DbFnCtx{
	public I_TxnAsy? Txn{get;set;}
}

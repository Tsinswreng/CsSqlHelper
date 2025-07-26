namespace Tsinswreng.CsSqlHelper;

public  partial class BaseDbFnCtx:IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
	public IDictionary<str, object?>? Props{get;set;}
}

namespace Tsinswreng.CsSqlHelper;

public class BaseDbFnCtx:IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
	public IDictionary<str, object?>? Props{get;set;}
}

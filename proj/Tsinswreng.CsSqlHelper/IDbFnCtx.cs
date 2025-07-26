namespace Tsinswreng.CsSqlHelper;

public  partial interface IBaseDbFnCtx{
	public ITxn? Txn{get;set;}
	public IDictionary<str, object?>? Props{get;set;}
}

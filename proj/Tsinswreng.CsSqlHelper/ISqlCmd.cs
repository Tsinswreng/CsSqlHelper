namespace Tsinswreng.CsSqlHelper;

public  partial interface ISqlCmd{
	public str? Sql{get;set;}
	public IAsyncEnumerable<IDictionary<str, obj?>> Run(CT Ct);
	public ISqlCmd Args(IDictionary<str, obj?> Args);
	public ISqlCmd Args(IEnumerable<obj?> Args);
	public ISqlCmd WithCtx(IBaseDbFnCtx? DbFnCtx);

}

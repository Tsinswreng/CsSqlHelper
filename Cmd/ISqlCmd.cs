namespace Tsinswreng.SqlHelper.Cmd;

public interface I_SqlCmd{
	public str? Sql{get;set;}
	public IAsyncEnumerable<IDictionary<str, object>> RunAsy(
		CancellationToken ct
	);

	public I_SqlCmd Args(IDictionary<str, object> Args);
	public I_SqlCmd Args(IEnumerable<object> Args);
	public I_SqlCmd WithCtx(I_DbFnCtx? DbFnCtx);


}

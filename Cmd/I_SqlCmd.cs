namespace Tsinswreng.SqlHelper.Cmd;

public interface I_SqlCmd{
	public str? Sql{get;set;}
	public IAsyncEnumerable<IDictionary<str, object>> RunAsy(
		CancellationToken ct
	);

	public I_SqlCmd SetParams(IDictionary<str, object> Params);
	public I_SqlCmd SetParams(IEnumerable<object> Params);


}

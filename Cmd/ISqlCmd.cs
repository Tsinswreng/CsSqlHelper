namespace Tsinswreng.CsSqlHelper.Cmd;

public interface ISqlCmd{
	public str? Sql{get;set;}
	public IAsyncEnumerable<IDictionary<str, object?>> Run(
		CancellationToken ct
	);

	public ISqlCmd Args(IDictionary<str, object?> Args);
	public ISqlCmd Args(IEnumerable<object?> Args);
	public ISqlCmd WithCtx(IBaseDbFnCtx? DbFnCtx);


}

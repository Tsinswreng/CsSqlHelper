namespace Tsinswreng.CsSqlHelper;

public partial interface ISqlCmd: IDisposable, IAsyncDisposable{
	public str? Sql{get;set;}
	public IAsyncEnumerable<IDictionary<str, obj?>> Run(CT Ct);

	/// <summary>
	/// raw arg name to value
	/// {
	/// 	"arg1": val1,
	/// 	"arg2": val2
	/// }
	/// </summary>
	/// <param name="Args"></param>
	/// <returns></returns>
	public ISqlCmd RawArgs(IDictionary<str, obj?> Args);
	/// <summary>
	/// resolved arg string(with prefix, e.g "@" for sqlite) to value
	/// {
	/// 	"@arg1": val1,
	/// 	"@arg2": val2
	/// }
	/// </summary>
	/// <param name="Args"></param>
	/// <returns></returns>
	public ISqlCmd ResolvedArgs(IDictionary<str, obj?> Args);
	public ISqlCmd Args(IEnumerable<obj?> Args);
	public ISqlCmd Args(IArgDict Args){
		return this.RawArgs(Args.ToDict());
	}
	public ISqlCmd WithCtx(IBaseDbFnCtx? DbFnCtx);

}

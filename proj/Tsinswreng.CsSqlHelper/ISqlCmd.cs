namespace Tsinswreng.CsSqlHelper;

public class EvtArgDispose: EventArgs{

}


public partial interface ISqlCmd: IDisposable, IAsyncDisposable{
	/// <summary>
	/// dispose旹 額外ᵈ珩之諸操作
	/// </summary>
	public IList<Func<Task<nil>>> FnsOnDispose{get;set;}
	public str? Sql{get;set;}
	public IAsyncEnumerable<IDictionary<str, obj?>> IterAsyE(CT Ct);
	public Task<IList<IDictionary<str, obj?>>> All(CT Ct);

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

	/// <summary>
	/// 只關聯事務、不做AddToDispose
	/// </summary>
	/// <param name="DbFnCtx"></param>
	/// <returns></returns>
	public ISqlCmd AttachCtxTxn(IDbFnCtx DbFnCtx);


	public async Task<IList<
		IList<IDictionary<str, obj?>>>
	> All2d(CT Ct){
		throw new NotImplementedException();
	}

}


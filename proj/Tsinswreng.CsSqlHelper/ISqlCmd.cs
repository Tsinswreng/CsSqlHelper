namespace Tsinswreng.CsSqlHelper;

public class EvtArgDispose: EventArgs{

}


public partial interface ISqlCmd: IDisposable, IAsyncDisposable{
	/// dispose旹 額外ᵈ珩之諸操作
	public IList<Func<Task<nil>>> FnsOnDispose{get;set;}
	public str? Sql{get;set;}
	public IAsyncEnumerable<
		IAsyncEnumerable<IDictionary<str, obj?>>
	> AsyE2d(
		CT Ct
	);
	public Task<IList<
		IList<IDictionary<str, obj?>>>
	> All2d(CT Ct);
	public IAsyncEnumerable<IDictionary<str, obj?>> AsyE1d(CT Ct);
	public Task<IList<IDictionary<str, obj?>>> All1d(CT Ct);
	

	/// raw arg name to value
	/// {
	/// 	"arg1": val1,
	/// 	"arg2": val2
	/// }
	public ISqlCmd RawArgs(IDictionary<str, obj?> Args);
	/// resolved arg string(with prefix, e.g "@" for sqlite) to value
	/// {
	/// 	"@arg1": val1,
	/// 	"@arg2": val2
	/// }
	public ISqlCmd ResolvedArgs(IDictionary<str, obj?> Args);
	public ISqlCmd Args(IEnumerable<obj?> Args);
	public ISqlCmd Args(IArgDict Args){
		return this.RawArgs(Args.ToDict());
	}

	/// 只關聯事務、不做AddToDispose
	public ISqlCmd AttachCtxTxn(IDbFnCtx DbFnCtx);


	

}


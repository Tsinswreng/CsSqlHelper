namespace Tsinswreng.CsSqlHelper;


[Doc($@"SQL command abstraction")]
public partial interface ISqlCmd: IDisposable, IAsyncDisposable{
	/// dispose旹 額外ᵈ珩之諸操作
	[Doc(@$"
#Sum[Additional operations executed when disposing]
#Rtn[List of callbacks to run during dispose]
")]
	public IList<Func<Task<nil>>> FnsOnDispose{get;set;}
	[Doc($@"SQL text to execute")]
	public str? Sql{get;set;}
	[Doc(@$"
#Sum[Execute and return 2D asynchronous enumerable result sets]
#Params([Cancellation token])
#Rtn[Asynchronous stream of result sets]
")]
	public IAsyncEnumerable<
		IAsyncEnumerable<IDictionary<str, obj?>>
	> AsyE2d(
		CT Ct
	);
	[Doc(@$"
#Sum[Execute and materialize all result sets as 2D collections]
#Params([Cancellation token])
#Rtn[All result sets in memory]
")]
	public Task<IList<
		IList<IDictionary<str, obj?>>>
	> All2d(CT Ct);
	[Doc(@$"
#Sum[Execute and return a flattened asynchronous row stream]
#Params([Cancellation token])
#Rtn[Asynchronous stream containing rows from all result sets]
")]
	public IAsyncEnumerable<IDictionary<str, obj?>> AsyE1d(CT Ct);
	[Doc(@$"
#Sum[Execute and materialize flattened rows as a list]
#Params([Cancellation token])
#Rtn[In-memory rows combined from all result sets]
")]
	public Task<IList<IDictionary<str, obj?>>> All1d(CT Ct);
	

	/// raw arg name to value
	/// {
	/// 	"arg1": val1,
	/// 	"arg2": val2
	/// }
	[Doc(@$"
#Sum[Set raw argument name-to-value mapping]
#Params([Raw argument dictionary where keys are argument names without provider prefix])
#Rtn[{nameof(ISqlCmd)} for chaining]
")]
	public ISqlCmd RawArgs(IDictionary<str, obj?> Args);
	/// resolved arg string(with prefix, e.g "@" for sqlite) to value
	/// {
	/// 	"@arg1": val1,
	/// 	"@arg2": val2
	/// }
	[Doc(@$"
#Sum[Set provider-resolved argument mapping]
#Params([Resolved argument dictionary with provider-specific prefixes, such as @name])
#Rtn[{nameof(ISqlCmd)} for chaining]
")]
	public ISqlCmd ResolvedArgs(IDictionary<str, obj?> Args);
	[Doc(@$"
#Sum[Set positional arguments]
#Params([Ordered argument values])
#Rtn[{nameof(ISqlCmd)} for chaining]
")]
	public ISqlCmd Args(IEnumerable<obj?> Args);
	[Doc(@$"
#Sum[Set arguments from {nameof(IArgDict)}]
#Params([Argument container])
#Rtn[{nameof(ISqlCmd)} for chaining]
")]
	public ISqlCmd Args(IArgDict Args){
		return this.RawArgs(Args.ToDict());
	}

	/// 只關聯事務、不做AddToDispose
	[Doc(@$"
#Sum[Attach transaction context only]
#Params([Database function context containing transaction information])
#Rtn[{nameof(ISqlCmd)} for chaining]
")]
	public ISqlCmd AttachCtxTxn(IDbFnCtx DbFnCtx);


	

}


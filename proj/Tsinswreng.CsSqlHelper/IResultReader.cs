namespace Tsinswreng.CsSqlHelper;


public interface IResultReader{
	[Doc(@$"the SqlCmd who generated this result set")]
	public ISqlCmd Cmd{get;set;}
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
}

public interface IResultReader<T>{
	
}

using System.Collections;

namespace Tsinswreng.CsSql;
/// Async binder for "Many(asyncEnumerable)" that can stream values by batch size asynchronously.
public interface IParamAutoBinderMultiAsy: IParamAutoBinder{
	[Doc(@$"
#Sum[Take next arguments batch from internal async sequence]
#Params([Expected batch size],[Cancellation token])
#Rtn[(True if at least one value is available, Batch list)]
")]
	public ValueTask<(bool HasAny, IList Batch)> TryTakeBatchArgsAsync(u64 BatchSize, CT Ct);
	[Doc(@$"
#Sum[Bind a taken batch into arguments]
#Params([Argument dictionary],[Batch values])
#Rtn[Void]
")]
	public void BindBatch(IArgDict Args, IList Batch);
}


using System.Collections;

namespace Tsinswreng.CsSql;
/// Bind values into <see cref="IArgDict"/> for one execution.
public interface IParamAutoBinder{
	[Doc(@$"
#Sum[Bind values into argument dictionary for current execution]
#Params([Argument dictionary])
#Rtn[Void]
")]
	public void Bind(IArgDict Args);
}

/// Binder for "Many(values)" that can stream values by batch size.
public interface IParamAutoBinderMulti: IParamAutoBinder{
	[Doc(@$"
#Sum[Take next arguments batch from internal sequence]
#Params([Expected batch size],[Output batch list])
#Rtn[True if at least one value is available]
")]
	public bool TryTakeBatchArgs(u64 BatchSize, out IList Args);
	[Doc(@$"
#Sum[Bind a taken batch into arguments]
#Params([Argument dictionary],[Batch values])
#Rtn[Void]
")]
	public void BindBatch(IArgDict Args, IList Batch);
}


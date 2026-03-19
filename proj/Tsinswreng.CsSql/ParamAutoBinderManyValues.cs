using System.Collections;

namespace Tsinswreng.CsSql;
/// Binder for prebuilt value sequence; supports incremental batch consumption.
public class ParamAutoBinderManyValues<TVal>: IParamAutoBinderMulti{
	[Doc(@$"Declared Parameter")]
	public IParam Param { get; set; }
	[Doc(@$"Received Arguments")]
	public IEnumerable<TVal> Args { get; set; }
	protected IEnumerator<TVal> ArgsItor{
		get{
			field ??= Args.GetEnumerator();
			return field;
		}
	}
	public ITable? Tbl { get; set; }
	
	
	public ParamAutoBinderManyValues(IParam Param, IEnumerable<TVal> Args){
		this.Param = Param;
		this.Args = Args;
	}

	[Doc(@$"
#Sum[Bind all values in sequence]
#Params([Argument dictionary])
#Rtn[Void]
")]
	public void Bind(IArgDict Args){
		foreach(var (i, value) in this.Args.Index()){
			var p = Param.ToOfst((u64)i);
			if(Tbl != null){
				Args.AddRaw(p, Tbl.UpperToRaw(value));
			}else{
				Args.AddRaw(p, value);
			}
		}
	}



	[Doc(@$"
#Sum[Take next N values from sequence]
#Params([Maximum items to take],[Taken values])
#Rtn[True when at least one value is taken]
")]
	public bool TryTakeBatchArgs(u64 BatchSize, out IList Batch){
		var args = new List<TVal>();
		var argsItor = ArgsItor;
		// Pull values lazily; do not materialize full source sequence.
		for(u64 i = 0; i < BatchSize; i++){
			if(!argsItor.MoveNext()){
				break;
			}
			args.Add(argsItor.Current);
		}
		Batch = args;
		return args.Count > 0;
	}

	[Doc(@$"
#Sum[Bind a pre-taken value batch]
#Params([Argument dictionary],[Batch values])
#Rtn[Void]
#Throw[{nameof(InvalidCastException)}][When batch element type does not match {nameof(TVal)}]
")]
	public void BindBatch(IArgDict Args, IList Batch){
		var list = new List<TVal>(Batch.Count);
		foreach(var item in Batch){
			if(item is not TVal typed){
				throw new InvalidCastException($"Expected batch item type {typeof(TVal).Name}, got {item?.GetType().Name ?? "null"}.");
			}
			list.Add(typed);
		}
		foreach(var (i, value) in list.Index()){
			var p = Param.ToOfst((u64)i);
			if(Tbl != null){
				Args.AddRaw(p, Tbl.UpperToRaw(value));
			}else{
				Args.AddRaw(p, value);
			}
		}
	}
}


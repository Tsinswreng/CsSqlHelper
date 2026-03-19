namespace Tsinswreng.CsSql;
/// Binder for one fixed value.
public class ParamAutoBinderOne<TVal>: IParamAutoBinder{
	public IParam Param { get; set; }
	public TVal Value { get; set; }
	public ITable? Tbl { get; set; }

	public ParamAutoBinderOne(IParam Param, TVal Value){
		this.Param = Param;
		this.Value = Value;
	}

	[Doc(@$"
#Sum[Bind one fixed value]
#Params([Argument dictionary])
#Rtn[Void]
")]
	public void Bind(IArgDict Args){
		if(Tbl != null){
			Args.AddRaw(Param, Tbl.UpperToRaw(Value));
			return;
		}
		Args.AddRaw(Param, Value);
	}
}


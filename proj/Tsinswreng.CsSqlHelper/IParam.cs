namespace Tsinswreng.CsSqlHelper;
using Tsinswreng.CsCore;

public partial interface I_AddParamPrefix{
	public str AddParamPrefix(str Name);
}

public partial interface IParam
	//:I_ShallowCloneSelf
{
	public str Name{get;set;}
	public I_AddParamPrefix ParamPrefixAdder{get;set;}

	public IParam this[u64 i]{get;}

	public static str NumSuffixParam(str Name, u64 i){
		return Name+"__"+i;
	}
}

public class Param:IParam{
	public Param(str Name, I_AddParamPrefix ParamPrefixAdder){
		this.Name = Name;
		this.ParamPrefixAdder = ParamPrefixAdder;
	}
	public str Name{get;set;}
	public I_AddParamPrefix ParamPrefixAdder{get;set;}

	public static str NumSuffixParam(str Name, u64 i){
		return Name+"__"+i;
	}
	public IParam this[u64 i]{get{
		return new Param(NumSuffixParam(Name, i), ParamPrefixAdder);
	}}
}

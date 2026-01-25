namespace Tsinswreng.CsSqlHelper;
using Tsinswreng.CsCore;

public partial interface I_AddParamPrefix{
	[Doc(@$"Add prefix to the parameter name, e.g @ for sqlite or [] for SqlServer.")]
	public str AddParamPrefix(str Name);
}

[Doc(@$"Represent a parameter in sql.
must override ToString() to return the parameter suitable for sql syntax,
e.g @+ParamName for sqlite's sql syntax or [ParamName] for SqlServer's syntax.
#Child[{nameof(Param)}][Default impl]`
")]
public partial interface IParam
	//:I_ShallowCloneSelf
{
	[Doc(@$"Raw Param name. without param token like @ or : or [] etc.")]
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

	public override str ToString(){
		return ParamPrefixAdder.AddParamPrefix(Name);
	}
}

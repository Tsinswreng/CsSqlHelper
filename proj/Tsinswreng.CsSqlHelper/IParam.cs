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


	// [Obsolete(@$"Use {nameof(ToOfst)}")]
	// public IParam this[u64 Ofst]{get{
	// 	return new Param(NumOfstName(Name, Ofst), ParamPrefixAdder);
	// }}


	[Doc(@$"Copy a {nameof(IParam)} object with a specified number offset.
	Usually used in batch operation, the number offset is used to mark its batch number and avoid duplication param name
	#Param[{nameof(Ofst)}][*when is 0, no suffix would be attached to the name,* in order to make compatible with both single sql and batch sql.]
	#See[{nameof(NumOfstName)}]
	")]
	public IParam ToOfst(u64 Ofst){
		return new Param(NumOfstName(Name, Ofst), ParamPrefixAdder);
	}

	[Doc(@$"Usually used in batch operation, the number affix is used to mark its batch number and avoid duplication param name
	#Param[{nameof(Ofst)}][*when is 0, no suffix would be attached to the name,* in order to make compatible with both single sql and batch sql.]
	")]
	public static str NumOfstName(str Name, u64 Ofst){
		if(Ofst == 0){
			return Name;
		}
		return Name+"__"+Ofst;
	}

	[Doc(@$"Usually used in batch operation, the number affix is used to mark its batch number and avoid duplication param name")]
	public static str NumSuffixName(str Name, u64 i){
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

	public static str NumSuffixName(str Name, u64 i){
		return Name+"__"+i;
	}
	// public IParam this[u64 i]{get{
	// 	return new Param(NumSuffixParam(Name, i), ParamPrefixAdder);
	// }}

	public override str ToString(){
		return ParamPrefixAdder.AddParamPrefix(Name);
	}
}

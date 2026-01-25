namespace Tsinswreng.CsSqlHelper;

public partial interface IArgDict{
	public IDictionary<str, obj?> ParamName_RawValue{get;set;}
	[Doc(@$"
	#Params([],[Raw param name, without prefix(like `@` in sqlite's sql)])
	")]
	public IArgDict AddRaw(str ParamName, obj? Raw);
	public IArgDict AddRaw(IParam Param, obj? Raw);
	public IArgDict AddT<T>(IParam Param, T Raw, str? CodeColName=null);

	[Doc(@$"Batch bind multiple parameters.
	#Param[{nameof(Alt)}][if the length of {nameof(Uppers)} is not equal to {nameof(Params)}, use this value to replace missing values.]
	")]
	public IArgDict AddManyT<T>(
		IList<IParam> Params, IList<T> Uppers
		,str? CodeColName=null, obj? Alt=null
	);
	public IDictionary<str, obj?> ToDict();
}


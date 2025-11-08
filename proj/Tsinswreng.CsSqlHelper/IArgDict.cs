namespace Tsinswreng.CsSqlHelper;

public partial interface IArgDict{
	public IDictionary<str, obj?> ParamName_RawValue{get;set;}
	/// <summary>
	/// 裸ʹ參數名、不帶前ʹ修飾符如"@"
	/// </summary>
	/// <param name="ParamName"></param>
	/// <param name="Raw"></param>
	/// <returns></returns>
	public IArgDict AddRaw(str ParamName, obj? Raw);
	public IArgDict AddRaw(IParam Param, obj? Raw);
	public IArgDict AddT<T>(IParam Param, T Raw, str? CodeColName=null);
	public IDictionary<str, obj?> ToDict();
}


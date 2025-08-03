namespace Tsinswreng.CsSqlHelper;

public partial interface IArgDict{
	/// <summary>
	/// 裸ʹ參數名、不帶前ʹ修飾符如"@"
	/// </summary>
	/// <param name="ParamName"></param>
	/// <param name="Raw"></param>
	/// <returns></returns>
	[Obsolete]
	public IArgDict Add(str ParamName, obj? Raw);
	public IArgDict Add(IParam Param, obj? Raw);
	public IDictionary<str, obj?> ToDict();
}


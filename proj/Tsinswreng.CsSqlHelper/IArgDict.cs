namespace Tsinswreng.CsSqlHelper;

public partial interface IArgDict{
	public IArgDict Add(str ParamName, obj? Raw);
	public IDictionary<str, obj?> ToDict();
}


namespace Tsinswreng.CsSqlHelper;

public  partial interface IArgDict{
	public IArgDict Add(str ParamName, obj? Value);
	public IDictionary<str, obj?> ToDict();
}


namespace Tsinswreng.CsSqlHelper;
public class OptQry{
	public bool IncludeDeleted{get; set;}
	/// IN (@_0, ...) 參數數量
	public u64 InParamCnt{get;set;} = 1;
}

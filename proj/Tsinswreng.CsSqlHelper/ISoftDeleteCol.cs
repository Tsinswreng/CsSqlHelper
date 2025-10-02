namespace Tsinswreng.CsSqlHelper;

public  partial interface ISoftDeleteCol{
	public str CodeColName{get;set;}
	/// <summary>
	/// FnDelete(舊值)=>新值
	/// 參數用DbColType 勿用CodeType
	/// </summary>
	public Func<obj?, obj?> FnDelete{get;set;}
	/// <summary>
	/// FnRestore(舊值)=>新值
	/// 參數用DbColType 勿用CodeType
	/// </summary>
	public Func<obj?, obj?> FnRestore{get;set;}
}

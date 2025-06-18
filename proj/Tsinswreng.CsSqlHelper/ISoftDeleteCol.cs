namespace Tsinswreng.CsSqlHelper;

public interface ISoftDeleteCol{
	public str CodeColName{get;set;}
	/// <summary>
	/// FnDelete(舊值)=>新值
	/// 參數用DbColType 勿用CodeType
	/// </summary>
	public Func<object?, object?> FnDelete{get;set;}
	/// <summary>
	/// FnRestore(舊值)=>新值
	/// 參數用DbColType 勿用CodeType
	/// </summary>
	public Func<object?, object?> FnRestore{get;set;}
}

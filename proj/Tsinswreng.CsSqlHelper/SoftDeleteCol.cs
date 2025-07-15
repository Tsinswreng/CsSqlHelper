namespace Tsinswreng.CsSqlHelper;

public class SoftDelol: ISoftDeleteCol{
	/// <summary>
	/// name in database of the soft delete column
	/// </summary>
	public str CodeColName{get;set;} = "";
	/// <summary>
	/// FnDelete(舊值)=>新值
	/// 舊值未必會被注入
	/// </summary>
	public Func<object?, object?> FnDelete{get;set;} = (a)=>NIL;
	/// <summary>
	/// FnRestore(舊值)=>新值
	/// 舊值未必會被注入
	/// </summary>
	public Func<object?, object?> FnRestore{get;set;} = (a)=>NIL;
}

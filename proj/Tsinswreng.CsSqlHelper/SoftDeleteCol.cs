namespace Tsinswreng.CsSqlHelper;

public partial class SoftDelol: ISoftDeleteCol{
	/// <summary>
	/// name in database of the soft delete column
	/// </summary>
	public str CodeColName{get;set;} = "";
	/// <summary>
	/// FnDelete(舊值Raw)=>新值Raw
	/// 舊值未必會被注入
	/// </summary>
	public Func<obj?, obj?> FnDelete{get;set;} = (a)=>NIL;
	/// <summary>
	/// FnRestore(raw)=>Raw
	/// 舊值未必會被注入
	/// </summary>
	public Func<obj?, obj?> FnRestore{get;set;} = (a)=>NIL;


}

namespace Tsinswreng.CsSql;

public partial class SoftDelol: ISoftDeleteCol{
	/// name in database of the soft delete column
	public str CodeColName{get;set;} = "";
	/// FnDelete(舊值Raw)=>新值Raw
	/// 舊值未必會被注入
	public Func<obj?, obj?> FnDelete{get;set;} = (a)=>NIL;
	/// FnRestore(raw)=>Raw
	/// 舊值未必會被注入
	public Func<obj?, obj?> FnRestore{get;set;} = (a)=>NIL;
	public Func<str> FnSqlIsDel{get;set;} = ()=> "";
	public Func<str> FnSqlIsNonDel{get;set;} = ()=> "";


}

namespace Tsinswreng.CsSqlHelper;

public  partial interface ISqlMkr{

	public ISqlTypeMapper SqlTypeMapper{get;set;}

	/// <summary>
	/// 字段加引號 如Name -> "Name"或`Name`或[Name]等
	/// </summary>
	/// <param name="Name"></param>
	/// <returns></returns>
	public str Qt(str Name);
	/// <summary>
	/// 如Name -> "@Name" 等
	/// </summary>
	/// <param name="Name"></param>
	/// <returns></returns>
	public str Prm(str Name);

/// <summary>
///
/// </summary>
/// <param name="Limit">參數名</param>
/// <param name="Offset">參數名</param>
/// <returns></returns>
	public str PrmLmtOfst(str Limit, str Offset);
}


public static class ExtnISqlMkr{

	/// <summary>
	/// var T = TblMgr.GetTbl<Entity>();
	/// var SqlSeg = T.SqkMkr.PrmOfst(out Lmt, out Ofst);
	///
	/// </summary>
	/// <param name="z"></param>
	/// <param name="Lmt"></param>
	/// <param name="Ofst"></param>
	/// <returns></returns>
	public static str PrmLmtOfst(
		this ISqlMkr z
		,out str Lmt, out str Ofst
	){
		Lmt=nameof(Lmt);
		Ofst=nameof(Ofst);
		return z.PrmLmtOfst(Lmt, Ofst);
	}
}

namespace Tsinswreng.CsSqlHelper;

public partial interface ISqlMkr{

	public ISqlTypeMapper SqlTypeMapper{get;set;}

	/// <summary>
	/// 字段加引號 如Name -> "Name"或`Name`或[Name]等
	/// </summary>
	/// <param name="Name"></param>
	/// <returns></returns>
	public str Quote(str Name);
	/// <summary>
	/// 如Name -> "@Name" 等
	/// </summary>
	/// <param name="Name"></param>
	/// <returns></returns>
	// [Obsolete]
	// public str PrmStr(str Name);

	public IParam Param(str Name);

/// <summary>
///
/// </summary>
/// <param name="Limit">參數名</param>
/// <param name="Offset">參數名</param>
/// <returns></returns>
	public str ParamLimOfst(str Limit, str Offset);

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
	public static str ParamLimOfstStr(
		this ISqlMkr z
		,out str Lmt, out str Ofst
	){
		Lmt=nameof(Lmt);
		Ofst=nameof(Ofst);
		return z.ParamLimOfst(Lmt, Ofst);
	}

	public static str ParamLimOfst(
		this ISqlMkr z
		,out IParam Lmt, out IParam Ofst
	){
		Lmt=z.Param(nameof(Lmt));
		Ofst=z.Param(nameof(Ofst));
		return z.ParamLimOfst(Lmt.Name, Ofst.Name);
	}

	/// <summary>
	/// 直轉`=`、不支持比較 null
	/// </summary>
	/// <param name="z"></param>
	/// <param name="DbColName"></param>
	/// <param name="Param"></param>
	/// <returns></returns>
	public static str Eq(
		this ISqlMkr z
		,str DbColName, IParam Param
	){
		var Col = DbColName;
		//return $"({Col} = {Param} OR ({Col} IS NULL AND {Param} IS NULL))";
		return $"{z.Quote(DbColName)} = {Param}";
	}
}

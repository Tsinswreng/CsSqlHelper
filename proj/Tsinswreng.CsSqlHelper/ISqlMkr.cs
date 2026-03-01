namespace Tsinswreng.CsSqlHelper;

public partial interface ISqlMkr{

	public ISqlTypeMapper SqlTypeMapper{get;set;}

	/// 字段加引號 如Name -> "Name"或`Name`或[Name]等
	public str Quote(str Name);
	/// 如Name -> "@Name" 等
	
	public IParam Param(str Name);

	public str ParamLimOfst(str Limit, str Offset);

}


public static class ExtnISqlMkr{

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

	/// 直轉`=`、不支持比較 null
	public static str Eq(
		this ISqlMkr z
		,str DbColName, IParam Param
	){
		var Col = DbColName;
		//return $"({Col} = {Param} OR ({Col} IS NULL AND {Param} IS NULL))";
		return $"{z.Quote(DbColName)} = {Param}";
	}
}

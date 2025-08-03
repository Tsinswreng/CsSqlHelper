namespace Tsinswreng.CsSqlHelper;

public static class ExtnTblMgr{
	public static str SqlMkSchema(
		this ITblMgr z
	){
		List<str> R = [];
		foreach(var (Type, Tbl) in z.EntityType_Table){
			var U = Tbl.SqlMkTbl();
			R.Add(U);
		}
		return str.Join("\n",R);
	}
}

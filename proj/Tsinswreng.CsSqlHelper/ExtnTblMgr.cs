namespace Tsinswreng.CsSqlHelper;

public static class ExtnTblMgr{
	extension(ITblMgr z){
		public IList<str> SqlsMkSchema(){
			List<str> R = [];
			foreach(var (Type, Tbl) in z.EntityType_Tbl){
				var U = Tbl.SqlMkTbl();
				R.Add(U);
			}
			return R;
		}
		public str SqlMkSchema(){
			return str.Join("\n", z.SqlsMkSchema());
		}
	}
	extension<TSelf>(TSelf z)
		where TSelf: ITblMgr, new()
	{
		public TSelf Clone(){
			var R = new TSelf();
			R.EntityType_Tbl = z.EntityType_Tbl;
			R.DbSrcType = z.DbSrcType;
			R.SqlMkr = z.SqlMkr;
			return R;
		}
	}

}

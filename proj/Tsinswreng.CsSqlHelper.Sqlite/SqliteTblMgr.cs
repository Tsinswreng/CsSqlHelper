namespace Tsinswreng.CsSqlHelper.Sqlite;
public partial class SqliteTblMgr : TblMgr{
	public override IDbStuff DbStuff{get;set;} = SqliteStuff.Inst;
}

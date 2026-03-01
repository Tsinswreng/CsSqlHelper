namespace Tsinswreng.CsSqlHelper.Sqlite;
public partial class SqliteTblMgr : ITblMgr{
	public IDbStuff DbStuff{get;set;} = SqliteStuff.Inst;
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;} = new Dictionary<Type, ITable>();
}

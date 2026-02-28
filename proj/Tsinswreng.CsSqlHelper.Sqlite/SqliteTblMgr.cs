namespace Tsinswreng.CsSqlHelper.Sqlite;
public partial class SqliteTblMgr : ITblMgr{
	public str DbSrcType{get;set;} = ConstDbSrcType.Sqlite;
	public ISqlMkr SqlMkr{get;set;} = new SqliteSqlMkr();
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;} = new Dictionary<Type, ITable>();

}

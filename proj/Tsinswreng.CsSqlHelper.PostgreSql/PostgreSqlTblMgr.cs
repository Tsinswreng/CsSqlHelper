namespace Tsinswreng.CsSqlHelper.PostgreSql;
public  partial class PostgreSqlTblMgr : ITblMgr{
	public str DbSrcType{get;set;} = "PostgreSql";
	public ISqlMkr SqlMkr{get;set;} = new PostgreSqlSqlMkr();

	public IDictionary<Type, ITable> EntityType_Table{get;set;} = new Dictionary<Type, ITable>();

	// public void AddTable<T_Po>(I_Table table){
	// 	Type__Table.Add(typeof(T_Po), table);
	// }

	// public I_Table GetTable<T_Po>(){
	// 	return Type__Table[typeof(T_Po)];
	// }
}

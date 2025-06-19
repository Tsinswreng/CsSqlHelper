namespace Tsinswreng.CsSqlHelper.Sqlite;
public class SqliteTblMgr : ITblMgr{
	protected static SqliteTblMgr? _Inst = null;
	public static SqliteTblMgr Inst => _Inst??= new SqliteTblMgr();
	public str DbSrcType{get;set;} = "Sqlite";
	public ISqlMkr SqlMkr{get;set;} = new SqliteSqlMkr();

	public IDictionary<Type, ITable> Type_Table{get;set;} = new Dictionary<Type, ITable>();

	// public void AddTable<T_Po>(I_Table table){
	// 	Type__Table.Add(typeof(T_Po), table);
	// }

	// public I_Table GetTable<T_Po>(){
	// 	return Type__Table[typeof(T_Po)];
	// }
}

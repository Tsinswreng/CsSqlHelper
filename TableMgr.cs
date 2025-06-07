namespace Tsinswreng.CsSqlHelper;

public class AppTableMgr : ITableMgr{
	protected static AppTableMgr? _Inst = null;
	public static AppTableMgr Inst => _Inst??= new AppTableMgr();
	public str DbType{get;set;} = "Sqlite";
	public ISqlMkr SqlMkr{get;set;} = new SqliteSqlMkr();

	public IDictionary<Type, ITable> Type__Table{get;set;} = new Dictionary<Type, ITable>();

	// public void AddTable<T_Po>(I_Table table){
	// 	Type__Table.Add(typeof(T_Po), table);
	// }

	// public I_Table GetTable<T_Po>(){
	// 	return Type__Table[typeof(T_Po)];
	// }
}

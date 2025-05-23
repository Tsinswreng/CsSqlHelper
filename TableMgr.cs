namespace Tsinswreng.SqlHelper;

public class AppTableMgr : ITableMgr{
	protected static AppTableMgr? _Inst = null;
	public static AppTableMgr Inst => _Inst??= new AppTableMgr();
	public str DbType{get;set;} = "Sqlite";
	public I_SqlMkr SqlMkr{get;set;} = new SqliteSqlMkr();

	public IDictionary<Type, I_Table> Type__Table{get;set;} = new Dictionary<Type, I_Table>();

	// public void AddTable<T_Po>(I_Table table){
	// 	Type__Table.Add(typeof(T_Po), table);
	// }

	// public I_Table GetTable<T_Po>(){
	// 	return Type__Table[typeof(T_Po)];
	// }
}

namespace Tsinswreng.SqlHelper;

public class TableMgr : I_TableMgr{
	protected static TableMgr? _Inst = null;
	public static TableMgr Inst => _Inst??= new TableMgr();

	public Dictionary<Type, I_Table> Type__Table{get;set;} = new Dictionary<Type, I_Table>();

	// public void AddTable<T_Po>(I_Table table){
	// 	Type__Table.Add(typeof(T_Po), table);
	// }

	// public I_Table GetTable<T_Po>(){
	// 	return Type__Table[typeof(T_Po)];
	// }
}

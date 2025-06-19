using Tsinswreng.CsSqlHelper.Impl.Sqlite;

namespace Tsinswreng.CsSqlHelper;
/// <summary>
/// TODO 潙每種數據源 各建基類
/// 使子類繼承㞢後 遷入Ngaq.Local
/// </summary>
public class AppTableMgr : ITblMgr{
	protected static AppTableMgr? _Inst = null;
	public static AppTableMgr Inst => _Inst??= new AppTableMgr();
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

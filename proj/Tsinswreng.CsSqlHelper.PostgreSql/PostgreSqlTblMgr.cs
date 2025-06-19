namespace Tsinswreng.CsSqlHelper.PostgreSql;
public class PostgreSqlTblMgr : ITblMgr{
	protected static PostgreSqlTblMgr? _Inst = null;
	public static PostgreSqlTblMgr Inst => _Inst??= new PostgreSqlTblMgr();
	public str DbSrcType{get;set;} = "PostgreSql";
	public ISqlMkr SqlMkr{get;set;} = new PostgreSqlSqlMkr();

	public IDictionary<Type, ITable> Type_Table{get;set;} = new Dictionary<Type, ITable>();

	// public void AddTable<T_Po>(I_Table table){
	// 	Type__Table.Add(typeof(T_Po), table);
	// }

	// public I_Table GetTable<T_Po>(){
	// 	return Type__Table[typeof(T_Po)];
	// }
}

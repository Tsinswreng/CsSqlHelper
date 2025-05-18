namespace Tsinswreng.SqlHelper;

public interface I_TableMgr{
	public Dictionary<Type, I_Table> Type__Table{get;set;}
	public str DbType{get;set;}
	public I_SqlMkr SqlMkr{get;set;}

	public void AddTable<T_Po>(I_Table table){
		table.SqlMkr = SqlMkr;
		Type__Table.Add(typeof(T_Po), table);
	}

	public I_Table GetTable<T_Po>(){
		return Type__Table[typeof(T_Po)];
	}
}

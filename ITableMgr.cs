namespace Tsinswreng.SqlHelper;

public interface ITableMgr{
	public IDictionary<Type, ITable> Type__Table{get;set;}
	public str DbType{get;set;}
	public ISqlMkr SqlMkr{get;set;}

	public void AddTable<T_Po>(ITable table){
		table.SqlMkr = SqlMkr;
		Type__Table.Add(typeof(T_Po), table);
	}

	public ITable GetTable<T_Po>(){
		return Type__Table[typeof(T_Po)];
	}
}

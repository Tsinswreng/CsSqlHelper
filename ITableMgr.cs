namespace Tsinswreng.CsSqlHelper;

public interface ITblMgr{
	public IDictionary<Type, ITable> Type_Table{get;set;}
	public str DbSrcType{get;set;}
	public ISqlMkr SqlMkr{get;set;}

	public void AddTable<TPo>(ITable table){
		table.SqlMkr = SqlMkr;
		Type_Table.Add(typeof(TPo), table);
	}

	public ITable GetTable<T_Po>(){
		return Type_Table[typeof(T_Po)];
	}
}

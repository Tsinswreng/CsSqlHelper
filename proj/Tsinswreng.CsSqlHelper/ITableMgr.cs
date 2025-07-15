namespace Tsinswreng.CsSqlHelper;

//TODO配置忽略之字段
public interface ITblMgr{
	public IDictionary<Type, ITable> Type_Table{get;set;}
	public str DbSrcType{get;set;}
	public ISqlMkr SqlMkr{get;set;}

	[Obsolete("用AddTable(ITable Tbl)、少寫一遍類型")]
	public void AddTable<TPo>(ITable Tbl){
		Tbl.SqlMkr = SqlMkr;
		Type_Table.Add(typeof(TPo), Tbl);
	}
	public void AddTable(ITable Tbl){
		Tbl.SqlMkr = SqlMkr;
		Type_Table.Add(Tbl.EntityClrType, Tbl);
	}

	public ITable GetTable<T_Po>(){
		return Type_Table[typeof(T_Po)];
	}
}

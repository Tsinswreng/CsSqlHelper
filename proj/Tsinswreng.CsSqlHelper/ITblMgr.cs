namespace Tsinswreng.CsSqlHelper;

//TODO配置忽略之字段
public partial interface ITblMgr{
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;}
	public str DbSrcType{get;set;}
	public ISqlMkr SqlMkr{get;set;}

	// [Obsolete("用AddTable(ITable Tbl)、少寫一遍類型")]
	// public void AddTable<TPo>(ITable Tbl){
	// 	Tbl.SqlMkr = SqlMkr;
	// 	Type_Table.Add(typeof(TPo), Tbl);
	// }
	public void AddTbl(ITable Tbl){
		Tbl.SqlMkr = SqlMkr;
		//EntityType_Tbl.Add(Tbl.CodeEntityType, Tbl);
		EntityType_Tbl[Tbl.CodeEntityType] = Tbl;
	}

	public ITable GetTbl<T_Po>(){
		return EntityType_Tbl[typeof(T_Po)];
	}
}

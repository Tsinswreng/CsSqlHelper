using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper;

//TODO配置忽略之字段
public partial interface ITblMgr{
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;}
	public str DbSrcType{get;set;}
	public ISqlMkr SqlMkr{get;set;}

	public void AddTbl(ITable Tbl){
		Tbl.SqlMkr = SqlMkr;
		Tbl.TblMgr = this;
		//EntityType_Tbl.Add(Tbl.CodeEntityType, Tbl);
		EntityType_Tbl[Tbl.CodeEntityType] = Tbl;
	}

	public ITable<TPo> GetTbl<TPo>(){
		if(!EntityType_Tbl.TryGetValue(typeof(TPo), out var T)){
			throw new Exception($"GetTbl<> Failed. {typeof(TPo)} is not registered.");
		}
		var R = T as ITable<TPo>;
		if(R is null){
			throw new Exception($"Table is not generic ITable<{typeof(TPo)}> ");
		}
		return R;
	}
}

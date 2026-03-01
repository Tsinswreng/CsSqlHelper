using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper;

//TODO配置忽略之字段
public partial interface ITblMgr{
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;}
	public str DbSrcType=>DbStuff.DbSrcType;
	public IDbStuff DbStuff{get;set;}
	public ISqlMkr SqlMkr=>DbStuff.SqlMkr;

	public nil AddTbl(ITable Tbl){
		Tbl.TblMgr = this;
		EntityType_Tbl[Tbl.CodeEntityType] = Tbl;
		return NIL;
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

public static class ExtnITblMgr{
	extension(ITblMgr z){
		public nil AddTbl<T>(ITblSetter<T> TblSetter){
			return z.AddTbl(TblSetter.Tbl);
		}
	}
}

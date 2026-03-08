namespace Tsinswreng.CsSqlHelper.Postgres;
public partial class PostgresTblMgr : ITblMgr{
	public IDbStuff DbStuff{get;set;} = PostgresStuff.Inst;
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;} = new Dictionary<Type, ITable>();
	public IDictionary<Type, IAggReg> AggType_Reg{get;set;} = new Dictionary<Type, IAggReg>();

}

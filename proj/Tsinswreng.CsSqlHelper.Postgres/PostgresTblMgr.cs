namespace Tsinswreng.CsSqlHelper.Postgres;
public partial class PostgresTblMgr : ITblMgr{
	public str DbSrcType{get;set;} = ConstDbSrcType.Postgres;
	public IDbStuff DbStuff{get;set;} = PostgresStuff.Inst;
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;} = new Dictionary<Type, ITable>();

}

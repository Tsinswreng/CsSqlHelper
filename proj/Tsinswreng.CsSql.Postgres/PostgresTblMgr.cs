namespace Tsinswreng.CsSql.Postgres;
public partial class PostgresTblMgr : TblMgr{
	public override IDbStuff DbStuff{get;set;} = PostgresStuff.Inst;
}

namespace Tsinswreng.CsSqlHelper.Postgres;
using Tsinswreng.CsSqlHelper;

public class PostgresStuff:IDbStuff{
	
	public ISqlMkr SqlMkr{get;set;} = PostgresSqlMkr.Inst;
	
	public IDbValConvtr DbValConvtr{get;set;} = PostgresValConvtr.Inst;
}

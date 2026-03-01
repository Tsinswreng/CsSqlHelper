namespace Tsinswreng.CsSqlHelper.Postgres;
using Tsinswreng.CsSqlHelper;

public class PostgresStuff:IDbStuff{
	public static PostgresStuff Inst => field??= new PostgresStuff();
	public ISqlMkr SqlMkr{get;set;} = PostgresSqlMkr.Inst;
	
	public IDbValConvtr DbValConvtr{get;set;} = PostgresValConvtr.Inst;
}

namespace Tsinswreng.CsSqlHelper.Sqlite;
using Tsinswreng.CsSqlHelper;

public class SqliteStuff:IDbStuff{
	
	public ISqlMkr SqlMkr{get;set;} = SqliteSqlMkr.Inst;
	
	public IDbValConvtr DbValConvtr{get;set;} = SqliteValConvtr.Inst;
}


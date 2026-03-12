namespace Tsinswreng.CsSql.Sqlite;
using Tsinswreng.CsSql;

public class SqliteStuff:IDbStuff{
	public static SqliteStuff Inst => field??=new SqliteStuff();
	public EDbSrcType DbSrcType{get;set;} = EDbSrcType.Sqlite;
	public ISqlMkr SqlMkr{get;set;} = SqliteSqlMkr.Inst;
	
	public IDbValConvtr DbValConvtr{get;set;} = SqliteValConvtr.Inst;
}


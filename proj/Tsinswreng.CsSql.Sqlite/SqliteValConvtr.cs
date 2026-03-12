using Tsinswreng.CsCore;

namespace Tsinswreng.CsSql.Sqlite;

public class SqliteValConvtr: IDbValConvtr{
	public static SqliteValConvtr Inst => field??=new SqliteValConvtr();

}

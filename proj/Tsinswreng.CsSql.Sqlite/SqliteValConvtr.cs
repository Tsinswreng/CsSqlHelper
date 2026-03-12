using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper.Sqlite;

public class SqliteValConvtr: IDbValConvtr{
	public static SqliteValConvtr Inst => field??=new SqliteValConvtr();

}

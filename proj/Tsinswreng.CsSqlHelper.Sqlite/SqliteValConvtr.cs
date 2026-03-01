using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper.Sqlite;

public class SqliteValConvtr: IDbValConvtr{
	public static SqliteValConvtr Inst => field??=new SqliteValConvtr();
	[Doc(@$"Converts a value from the database to a value in the code
	e.g NPgsql does not support UInt64, so we need to convert it to Int64.
	")]
	public obj? ToDbVal(obj? CodeVal){
		return CodeVal;
	}
	[Doc(@$"")]
	public obj? ToCodeVal(obj? DbVal){
		return DbVal;
	}
}

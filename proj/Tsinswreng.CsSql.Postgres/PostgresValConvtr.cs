using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper.Postgres;

public class PostgresValConvtr: IDbValConvtr{
	public static PostgresValConvtr Inst => field??=new PostgresValConvtr();
	[Doc(@$"Converts a value from code to PostgreSQL database value")]
	public obj? ToDbVal(obj? CodeVal){
		if(CodeVal == null){
			return DBNull.Value;
		}
		if(CodeVal is UInt64){
			// Npgsql does not natively support UInt64 as parameter value.
			return Convert.ToInt64(CodeVal);
		}
		return CodeVal;
	}
	[Doc(@$"Converts a value from PostgreSQL database to code value")]
	public obj? ToCodeVal(obj? DbVal){
		if(DbVal is DBNull){
			return null!;
		}
		return DbVal;
	}
}

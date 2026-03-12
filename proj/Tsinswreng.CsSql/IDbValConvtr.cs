namespace Tsinswreng.CsSql;


[Doc(@$"Database Value Converter for Sql Parameter")]
public interface IDbValConvtr{
	[Doc(@$"Converts a value from the database to a value in the code
	e.g NPgsql does not support UInt64, so we need to convert it to Int64.
	")]
	public obj? ToDbVal(obj? CodeVal){
		if(CodeVal == null){
			return DBNull.Value;
		}
		return CodeVal;
	}
	[Doc(@$"")]
	public obj? ToCodeVal(obj? DbVal){
		if(DbVal is DBNull){
			return null;
		}
		return DbVal;
	}
}



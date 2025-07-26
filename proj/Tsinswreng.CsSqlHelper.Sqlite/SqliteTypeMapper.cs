namespace Tsinswreng.CsSqlHelper.Sqlite;

public  partial class SqliteTypeMapper : ISqlTypeMapper{
protected static SqliteTypeMapper? _Inst = null;
public static SqliteTypeMapper Inst => _Inst??= new SqliteTypeMapper();

	public IDictionary<Type, str> ClrType_Name{get;set;} = new Dictionary<Type, str>();
	public SqliteTypeMapper(){
		ClrType_Name = new Dictionary<Type, str>(){
			{ EValueTypeNotNull.Byte, "INTEGER" }
			,{ EValueTypeNotNull.SByte, "INTEGER" }
			,{ EValueTypeNotNull.UInt16, "INTEGER" }
			,{ EValueTypeNotNull.Int16, "INTEGER" }
			,{ EValueTypeNotNull.UInt32, "INTEGER" }
			,{ EValueTypeNotNull.Int32, "INTEGER" }
			,{ EValueTypeNotNull.UInt64, "INTEGER" }
			,{ EValueTypeNotNull.Int64, "INTEGER" }
			,{ EValueTypeNotNull.Single, "REAL" }
			,{ EValueTypeNotNull.Double, "REAL" }

			,{ EValueTypeNullable.Byte, "INTEGER" }
			,{ EValueTypeNullable.SByte, "INTEGER" }
			,{ EValueTypeNullable.UInt16, "INTEGER" }
			,{ EValueTypeNullable.Int16, "INTEGER" }
			,{ EValueTypeNullable.UInt32, "INTEGER" }
			,{ EValueTypeNullable.Int32, "INTEGER" }
			,{ EValueTypeNullable.UInt64, "INTEGER" }
			,{ EValueTypeNullable.Int64, "INTEGER" }
			,{ EValueTypeNullable.Single, "REAL" }
			,{ EValueTypeNullable.Double, "REAL" }


			,{ ERefType.String, "TEXT" }
			,{ ERefType.ByteArr, "BLOB" }
		};
	}

	public str ToDbTypeName(Type Type){
		if(ClrType_Name.TryGetValue(Type, out str? result)){
			return result;
		}
		throw new NotImplementedException($"Type {Type.Name} is not supported and cannot be mapped to a Sqlite type name.");
	}

}

namespace Tsinswreng.CsSqlHelper.Sqlite;

public static class EValueTypeNullable{
public static readonly Type Byte = typeof(u8?);
public static readonly Type SByte = typeof(i8?);
public static readonly Type UInt16 = typeof(u16?);
public static readonly Type Int16 = typeof(i16?);
public static readonly Type UInt32 = typeof(u32?);
public static readonly Type Int32 = typeof(i32?);
public static readonly Type UInt64 = typeof(u64?);
public static readonly Type Int64 = typeof(i64?);
public static readonly Type Single = typeof(f32?);
public static readonly Type Double = typeof(f64?);
}

public static class EValueTypeNotNull{

public static readonly Type Byte = typeof(u8);
public static readonly Type SByte = typeof(i8);
public static readonly Type UInt16 = typeof(u16);
public static readonly Type Int16 = typeof(i16);
public static readonly Type UInt32 = typeof(u32);
public static readonly Type Int32 = typeof(i32);
public static readonly Type UInt64 = typeof(u64);
public static readonly Type Int64 = typeof(i64);
public static readonly Type Single = typeof(f32);
public static readonly Type Double = typeof(f64);

}

public static class ERefType{
	public static readonly Type String = typeof(str);
	public static readonly Type ByteArr = typeof(u8[]);
}

public class SqliteTypeMapper : ISqlTypeMapper{
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

namespace Tsinswreng.CsSqlHelper.Impl.Sqlite;

public static class ERawType{

public static readonly Type Byte = typeof(u8);
public static readonly Type SByte = typeof(i8);
public static readonly Type UInt16 = typeof(u16);
public static readonly Type Int16 = typeof(i16);
public static readonly Type UInt32 = typeof(u32);
public static readonly Type Int32 = typeof(i32);
public static readonly Type Int64 = typeof(i64);
public static readonly Type UInt64 = typeof(u64);
public static readonly Type Single = typeof(f32);
public static readonly Type Double = typeof(f64);
public static readonly Type String = typeof(str);
public static readonly Type ByteArr = typeof(u8[]);


}

public class SqliteTypeNameMapper : ITypeNameMapper{
	public IDictionary<Type, str> ClrType_Name{get;set;} = new Dictionary<Type, str>();
	public SqliteTypeNameMapper(){
		var d = ClrType_Name;
		d = new Dictionary<Type, str>(){
			{ ERawType.Byte, "INTEGER" }
			,{ ERawType.SByte, "INTEGER" }
			,{ ERawType.UInt16, "INTEGER" }
			,{ ERawType.Int16, "INTEGER" }
			,{ ERawType.UInt32, "INTEGER" }
			,{ ERawType.Int32, "INTEGER" }
			,{ ERawType.Int64, "INTEGER" }
			,{ ERawType.UInt64, "INTEGER" }
			,{ ERawType.Single, "REAL" }
			,{ ERawType.Double, "REAL" }
			,{ ERawType.String, "TEXT" }
			,{ ERawType.ByteArr, "BLOB" }
		};
	}

	public str ToDbTypeName(Type Type){
		if(ClrType_Name.TryGetValue(Type, out str? result)){
			return result;
		}
		throw new NotImplementedException($"Type {Type.Name} is not supported.");
	}

}

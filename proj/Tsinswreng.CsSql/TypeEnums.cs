namespace Tsinswreng.CsSqlHelper;
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

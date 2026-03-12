namespace Tsinswreng.CsSqlHelper.Test.Domains;

/// <summary>
/// 測試實體：主鍵使用 blob(byte[])，其餘字段覆蓋常見基礎類型
/// </summary>
public class PoAllBasicTypes {
	/// <summary>
	/// 主鍵：u8[] / blob
	/// </summary>
	public byte[] Id { get; set; } = [];

	public byte U8Val { get; set; }
	public sbyte I8Val { get; set; }
	public ushort U16Val { get; set; }
	public short I16Val { get; set; }
	public uint U32Val { get; set; }
	public int I32Val { get; set; }
	public ulong U64Val { get; set; }
	public long I64Val { get; set; }

	public float F32Val { get; set; }
	public double F64Val { get; set; }

	public string StrVal { get; set; } = string.Empty;
	public string? StrNullable { get; set; }

	public byte[] BlobVal { get; set; } = [];
	public byte[]? BlobNullable { get; set; }

	public int? I32Nullable { get; set; }
	public long? I64Nullable { get; set; }
	public double? F64Nullable { get; set; }
}

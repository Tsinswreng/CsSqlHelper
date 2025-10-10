namespace Tsinswreng.CsSqlHelper.Postgres;


public partial class PostgresTypeMapper : ISqlTypeMapper {
	protected static PostgresTypeMapper? _Inst = null;

	public static PostgresTypeMapper Inst => _Inst ??= new PostgresTypeMapper();

	// <del>这里用 object 作为 key，支持三类枚举的混合 Dictionary</del>
	public IDictionary<Type, string> ClrType_Name { get; set; } = new Dictionary<Type, string>();

	public PostgresTypeMapper() {
		ClrType_Name = new Dictionary<Type, string>()
		{
				// 非空值类型映射
				{ EValueTypeNotNull.Byte, "smallint" },
				{ EValueTypeNotNull.SByte, "smallint" },
				{ EValueTypeNotNull.UInt16, "integer" },
				{ EValueTypeNotNull.Int16, "smallint" },
				{ EValueTypeNotNull.UInt32, "bigint" },
				{ EValueTypeNotNull.Int32, "integer" },
				{ EValueTypeNotNull.UInt64, "numeric" },
				{ EValueTypeNotNull.Int64, "bigint" },
				{ EValueTypeNotNull.Single, "real" },
				{ EValueTypeNotNull.Double, "double precision" },
				// { EValueTypeNotNull.Decimal, "numeric" },
				// { EValueTypeNotNull.Boolean, "boolean" },
				// { EValueTypeNotNull.DateTime, "timestamp without time zone" },
				// { EValueTypeNotNull.DateTimeOffset, "timestamp with time zone" },
				// { EValueTypeNotNull.TimeSpan, "interval" },

				// 可空值类型映射（对应非空部分一样）
				{ EValueTypeNullable.Byte, "smallint" },
				{ EValueTypeNullable.SByte, "smallint" },
				{ EValueTypeNullable.UInt16, "integer" },
				{ EValueTypeNullable.Int16, "smallint" },
				{ EValueTypeNullable.UInt32, "bigint" },
				{ EValueTypeNullable.Int32, "integer" },
				{ EValueTypeNullable.UInt64, "numeric" },
				{ EValueTypeNullable.Int64, "bigint" },
				{ EValueTypeNullable.Single, "real" },
				{ EValueTypeNullable.Double, "double precision" },
				// { EValueTypeNullable.Decimal, "numeric" },
				// { EValueTypeNullable.Boolean, "boolean" },
				// { EValueTypeNullable.DateTime, "timestamp without time zone" },
				// { EValueTypeNullable.DateTimeOffset, "timestamp with time zone" },
				// { EValueTypeNullable.TimeSpan, "interval" },

				// 引用类型
				{ ERefType.String, "text" },
				{ ERefType.ByteArr, "bytea" },
			};
	}

	public string ToDbTypeName(Type Type) {
		if (ClrType_Name.TryGetValue(Type, out var result)) {
			return result;
		}

		throw new NotImplementedException($"Type {Type} is not supported and cannot be mapped to a PostgreSQL type name.");
	}
}



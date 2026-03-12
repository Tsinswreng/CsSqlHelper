namespace Tsinswreng.CsSql;

[Doc(@$"constants for database source type.")]
[Obsolete(@$"use {nameof(EDbSrcType)} instead.")]
public class ConstDbSrcType{
	public const str Sqlite = "Sqlite";
	public const str Postgres = "Postgres";
}

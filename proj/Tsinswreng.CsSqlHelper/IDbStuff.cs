namespace Tsinswreng.CsSqlHelper;

[Doc(@$"Collection of things of a specialized Database Type(e,g Sqlite, Postgres)")]
public interface IDbStuff{
	[Doc(@$"Sql string Maker")]
	public ISqlMkr SqlMkr{get;set;}
	
	[Doc(@$"Database Value Converter")]
	public IDbValConvtr DbValConvtr{get;set;}
}

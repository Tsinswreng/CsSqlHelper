namespace Tsinswreng.CsSqlHelper;

[Doc(@$"Collection of things of a specialized Database Type(e,g Sqlite, Postgres)")]
public interface IDbStuff{
	public EDbSrcType DbSrcType{get;set;}
	[Doc(@$"Sql string Maker")]
	public ISqlMkr SqlMkr{get;set;}
	
	[Doc(@$"Database Value Converter")]
	public IDbValConvtr DbValConvtr{get;set;}
}

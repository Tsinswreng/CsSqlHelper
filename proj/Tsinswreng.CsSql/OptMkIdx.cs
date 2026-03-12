namespace Tsinswreng.CsSql;

[Doc(@$"option to create index on a table")]
public interface IOptMkIdx{
	public bool IfNotExists{get;set;}
	public bool Unique{get;set;}
	[Doc("@$Conditional Index")]
	public str Where{get;set;}
}

public class OptMkIdx:IOptMkIdx{
	public bool IfNotExists{get;set;}
	public bool Unique{get;set;}
	public str Where{get;set;} = str.Empty;
}

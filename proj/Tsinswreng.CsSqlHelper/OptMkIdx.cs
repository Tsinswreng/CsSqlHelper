namespace Tsinswreng.CsSqlHelper;

[Doc(@$"option to create index on a table")]
public interface IOptMkIdx{
	public bool IfNotExists{get;set;}
	public bool Unique{get;set;}
}

public class OptMkIdx:IOptMkIdx{
	
}

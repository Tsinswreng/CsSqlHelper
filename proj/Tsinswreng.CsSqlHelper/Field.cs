namespace Tsinswreng.CsSqlHelper;

public class Field:IField{
	public ITable Tbl{get;set;}
	public str CodeName{get;set;}
	public str DbName{get;set;}
	public override string ToString() {
		return Tbl.Qt(DbName);
	}
}

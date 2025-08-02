namespace Tsinswreng.CsSqlHelper.PostgreSql;

public partial class PostgreSqlParam:IParam{
	public str Name{get;set;} = "";
	public PostgreSqlParam(str Name){
		this.Name = Name;
	}
	public override string ToString() {
		return "@"+Name;
	}
}

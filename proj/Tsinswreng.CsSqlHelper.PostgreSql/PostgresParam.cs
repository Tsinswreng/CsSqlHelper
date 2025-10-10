namespace Tsinswreng.CsSqlHelper.PostgreSql;

public partial class PostgresParam:IParam{
	public str Name{get;set;} = "";
	public PostgresParam(str Name){
		this.Name = Name;
	}
	public override string ToString() {
		return "@"+Name;
	}
}

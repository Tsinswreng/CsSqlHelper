namespace Tsinswreng.CsSqlHelper.Sqlite;

public partial class SqliteParam:IParam{
	public str Name{get;set;} = "";
	public SqliteParam(str Name){
		this.Name = Name;
	}
	public override string ToString() {
		return "@"+Name;
	}
}

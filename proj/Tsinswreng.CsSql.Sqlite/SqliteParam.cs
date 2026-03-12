namespace Tsinswreng.CsSqlHelper.Sqlite;

public class SqliteParamPrefix:I_AddParamPrefix{
protected static SqliteParamPrefix? _Inst = null;
public static SqliteParamPrefix Inst => _Inst??= new SqliteParamPrefix();

	public str AddParamPrefix(str Name){
		return "@"+Name;
	}
}


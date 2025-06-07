namespace Tsinswreng.CsSqlHelper;

public class SqliteSqlMkr
	:ISqlMkr
{
	protected static SqliteSqlMkr? _Inst = null;
	public static SqliteSqlMkr Inst => _Inst??= new SqliteSqlMkr();

	public str Quote(str Name){
		return "\"" + Name + "\"";
	}

	public str Param(str Name){
		return "@" + Name;
	}

	public str LimitOffset(str Limit, str Offset){
		return $"LIMIT {Param(Limit)} OFFSET {Param(Offset)}";
	}


}

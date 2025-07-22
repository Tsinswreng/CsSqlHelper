namespace Tsinswreng.CsSqlHelper.PostgreSql;

public class PostgreSqlSqlMkr
	:ISqlMkr
{
	protected static PostgreSqlSqlMkr? _Inst = null;
	public static PostgreSqlSqlMkr Inst => _Inst??= new PostgreSqlSqlMkr();
	public ISqlTypeMapper SqlTypeMapper{get;set;} = PostgreSqlTypeMapper.Inst;

	public str Qt(str Name){
		return "\"" + Name + "\"";
	}

	public str Prm(str Name){
		return "@" + Name;
	}

	public str PrmLmtOfst(str Limit, str Offset){
		return $"LIMIT {Prm(Limit)} OFFSET {Prm(Offset)}";
	}


}

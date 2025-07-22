namespace Tsinswreng.CsSqlHelper.Sqlite;

public class SqliteSqlMkr
	:ISqlMkr
{
	protected static SqliteSqlMkr? _Inst = null;
	public static SqliteSqlMkr Inst => _Inst??= new SqliteSqlMkr();
	public ISqlTypeMapper SqlTypeMapper{get;set;} = SqliteTypeMapper.Inst;

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

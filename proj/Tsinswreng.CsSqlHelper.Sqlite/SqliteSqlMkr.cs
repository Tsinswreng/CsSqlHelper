namespace Tsinswreng.CsSqlHelper.Sqlite;

public partial class SqliteSqlMkr
	:ISqlMkr
{
	protected static SqliteSqlMkr? _Inst = null;
	public static SqliteSqlMkr Inst => _Inst??= new SqliteSqlMkr();
	public ISqlTypeMapper SqlTypeMapper{get;set;} = SqliteTypeMapper.Inst;

	public str Quote(str Name){
		return "\"" + Name + "\"";
	}

	public str PrmStr(str Name){
		return "@" + Name;
	}

	public IParam Param(str Name){
		var R = new SqliteParam(Name);
		return R;
	}

	public str ParamLimOfst(str Limit, str Offset){
		return $"LIMIT {Param(Limit)} OFFSET {Param(Offset)}";
	}
}

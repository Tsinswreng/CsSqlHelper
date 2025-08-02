namespace Tsinswreng.CsSqlHelper.PostgreSql;

public  partial class PostgreSqlSqlMkr
	:ISqlMkr
{
	protected static PostgreSqlSqlMkr? _Inst = null;
	public static PostgreSqlSqlMkr Inst => _Inst??= new PostgreSqlSqlMkr();
	public ISqlTypeMapper SqlTypeMapper{get;set;} = PostgreSqlTypeMapper.Inst;

	public str Qt(str Name){
		return "\"" + Name + "\"";
	}

	[Obsolete]
	public str PrmStr(str Name){
		return "@" + Name;
	}

	public IParam Prm(str Name){
		var R = new PostgreSqlParam(Name);
		return R;
	}

	public str PrmLmtOfst(str Limit, str Offset){
		return $"LIMIT {Prm(Limit)} OFFSET {Prm(Offset)}";
	}

}

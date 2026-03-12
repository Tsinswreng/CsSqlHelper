namespace Tsinswreng.CsSql.Sqlite;

public partial class SqliteSqlMkr
	:ISqlMkr
{
	public EDbSrcType DbSrcType => EDbSrcType.Sqlite;
	//protected static SqliteSqlMkr? _Inst = null;
	public static SqliteSqlMkr Inst => field??= new SqliteSqlMkr();
	public ISqlTypeMapper SqlTypeMapper{get;set;} = SqliteTypeMapper.Inst;

	public str Quote(str Name){
		return "\"" + Name + "\"";
	}

	public str PrmStr(str Name){
		return "@" + Name;
	}

	public IParam Param(str Name){
		var R = new Param(Name, SqliteParamPrefix.Inst);
		return R;
	}

	public str ParamLimOfst(str Limit, str Offset){
		return $"LIMIT {Param(Limit)} OFFSET {Param(Offset)}";
	}
}

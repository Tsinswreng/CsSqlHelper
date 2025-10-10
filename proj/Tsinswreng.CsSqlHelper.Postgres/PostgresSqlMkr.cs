namespace Tsinswreng.CsSqlHelper.Postgres;

public partial class PostgresSqlMkr
	:ISqlMkr
{
	protected static PostgresSqlMkr? _Inst = null;
	public static PostgresSqlMkr Inst => _Inst??= new PostgresSqlMkr();
	public ISqlTypeMapper SqlTypeMapper{get;set;} = PostgresTypeMapper.Inst;

	public str Quote(str Name){
		return "\"" + Name + "\"";
	}

	[Obsolete]
	public str PrmStr(str Name){
		return "@" + Name;
	}

	public IParam Param(str Name){
		var R = new PostgresParam(Name);
		return R;
	}

	public str ParamLimOfst(str Limit, str Offset){
		return $"LIMIT {Param(Limit)} OFFSET {Param(Offset)}";
	}

}

//TODO 類似ₐXxxCmd間 做抽象復用。SqliteCmd 新於 PostgresCmd
namespace Tsinswreng.CsSqlHelper.Postgres;

using System.Collections;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Npgsql;
using Tsinswreng.CsCore;
using Tsinswreng.CsSqlHelper.BaseImpl;
public partial class PostgresCmd : BaseSqlCmd<NpgsqlCommand, NpgsqlTransaction> {
	public PostgresCmd(NpgsqlCommand RawCmd):base(RawCmd){

	}
	public override EDbSrcType DbSrcType => EDbSrcType.Postgres;
	public override IDbValConvtr DbValConvtr{get;protected set;} = PostgresValConvtr.Inst;
	public override nil ParamAddWithValue(DbParameterCollection Params, string? parameterName, object? value) {
		if(Params is not NpgsqlParameterCollection prm){
			throw new ArgumentException("Params is not SqliteParameterCollection");
		}
		if(parameterName is null || value is null){
			throw new ArgumentNullException("parameterName is null || value is null");
		}
		prm.AddWithValue(parameterName, value);
		return NIL;
	}

	public override string ToResolvedArg(string RawArg) {
		return "@"+RawArg;
	}
}


//TODO 類似ₐXxxCmd間 做抽象復用。SqliteCmd 新於 PostgresCmd
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using Tsinswreng.CsCore;
using Tsinswreng.CsTools;
using Tsinswreng.CsSqlHelper.BaseImpl;
namespace Tsinswreng.CsSqlHelper.Sqlite;
using IDbFnCtx = Tsinswreng.CsSqlHelper.IDbFnCtx;

public partial class SqliteCmd : BaseSqlCmd<SqliteCommand, SqliteTransaction> {
	public SqliteCmd(SqliteCommand RawCmd):base(RawCmd){

	}
	public override IDbValConvtr DbValConvtr{get;protected set;} = SqliteValConvtr.Inst;
	public override nil ParamAddWithValue(DbParameterCollection Params, string? parameterName, object? value) {
		if(Params is not SqliteParameterCollection prm){
			throw new ArgumentException("Params is not SqliteParameterCollection");
		}
		prm.AddWithValue(parameterName, value);
		return NIL;
	}

	public override string ToResolvedArg(string RawArg) {
		return "@"+RawArg;
	}
}

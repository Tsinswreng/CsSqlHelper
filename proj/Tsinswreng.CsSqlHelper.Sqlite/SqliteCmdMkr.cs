namespace Tsinswreng.CsSqlHelper.Sqlite;

using Microsoft.Data.Sqlite;
using Tsinswreng.CsCore;


using IDbFnCtx = Tsinswreng.CsSqlHelper.IBaseDbFnCtx;
public partial class SqliteCmdMkr
	:ISqlCmdMkr
	,I_GetTxnAsy
{
	// [Obsolete]
	// public IDbConnection? DbConnection{get;set;}

	//#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	// [Obsolete]
	// public SqliteCmdMkr(IDbConnection DbConnection){
	// 	this.DbConnection = DbConnection;
	// }

	public IDbConnMgr DbConnGetter{get;set;}

	public SqliteCmdMkr(IDbConnMgr DbConnGetter){
		this.DbConnGetter = DbConnGetter;
	}


	[Doc($"""

	""")]
	[Impl(typeof(ISqlCmdMkr))]
	public async Task<ISqlCmd> MkCmd(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	){
		var DbConnection = DbFnCtx?.DbConn?? await DbConnGetter.GetConnAsy(Ct);
		if(DbConnection is not SqliteConnection sqlConn){
			throw new InvalidOperationException("DbConnection is not SqlConnection");
		}
		var RawCmd = sqlConn.CreateCommand();
		RawCmd.CommandText = Sql;
		var R = new SqliteCmd(RawCmd);
		R.Sql = Sql;
		if(DbFnCtx != null){
			R.WithCtx(DbFnCtx);
		}
		R.FnsOnDispose.Add(async()=>{
			return await DbConnGetter.AfterUsingConnAsy(DbConnection, default);
		});
		return R;
	}

	[Impl(typeof(ISqlCmdMkr))]
	public async Task<ISqlCmd> Prepare(ISqlCmd Cmd, CT Ct){
		if(Cmd is not SqliteCmd SqlCmd){
			throw new InvalidOperationException("ISqlCmd is not SqliteCmd");
		}
		SqlCmd.RawCmd.Prepare();
		return Cmd;
	}


	/// <summary>
	/// Prepare叵用于CREATE TABLE
	/// </summary>
	/// <param name="DbFnCtx"></param>
	/// <param name="Sql"></param>
	/// <param name="Ct"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	[Impl(typeof(ISqlCmdMkr))]
	public async Task<ISqlCmd> Prepare(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	){
		var Cmd = await MkCmd(DbFnCtx, Sql, Ct);
		return await Prepare(Cmd, Ct);
	}

	// [Impl(typeof(I_GetTxnAsy))]
	// public async Task<ITxn> GetTxnAsy(CT Ct){
	// 	var DbConnection = await DbConnGetter.GetConnAsy(Ct);
	// 	var Tx = DbConnection.BeginTransaction();
	// 	var Ans = new AdoTxn(Tx);
	// 	return Ans;
	// }

	[Impl(typeof(I_GetTxnAsy))]
	public async Task<ITxn> GetTxnAsy(
		IBaseDbFnCtx Ctx, CT Ct
	){
		var DbConnection = await DbConnGetter.GetConnAsy(Ct);
		Ctx.DbConn = DbConnection;
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		return Ans;
	}
}

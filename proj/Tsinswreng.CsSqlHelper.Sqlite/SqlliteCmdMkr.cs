using System.Data;
using Microsoft.Data.Sqlite;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper.Sqlite;
using IDbFnCtx = Tsinswreng.CsSqlHelper.IBaseDbFnCtx;
public partial class SqliteCmdMkr
	:ISqlCmdMkr
	,I_GetTxnAsy
{
	public IDbConnection DbConnection{get;set;}
	public SqliteCmdMkr(IDbConnection DbConnection){
		this.DbConnection = DbConnection;
	}

	[Impl(typeof(ISqlCmdMkr))]
	public async Task<ISqlCmd> MkCmd(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	){
		if(DbConnection is not SqliteConnection sqlConn){
			throw new InvalidOperationException("DbConnection is not SqlConnection");
		}
		var RawCmd = sqlConn.CreateCommand();
		RawCmd.CommandText = Sql;
		var ans = new SqliteCmd(RawCmd);
		if(DbFnCtx!= null){
			ans.WithCtx(DbFnCtx);
		}
		return ans;
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

	[Impl(typeof(I_GetTxnAsy))]
	public async Task<ITxn> GetTxnAsy(CT Ct){
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		return Ans;
	}
}

using System.Data;
using Microsoft.Data.Sqlite;
using Tsinswreng.CsSqlHelper.Cmd;
using Tsinswreng.CsSqlHelper.Db;

namespace Tsinswreng.CsSqlHelper.Sqlite;

public class SqliteCmdMkr
	:ISqlCmdMkr
	,IGetTxn
{
	public IDbConnection DbConnection{get;set;}
	public SqliteCmdMkr(IDbConnection DbConnection){
		this.DbConnection = DbConnection;
	}

	public virtual async Task<ISqlCmd> MkCmd(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		,CT ct
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

	public virtual async Task<ISqlCmd> Prepare(ISqlCmd Cmd, CT Ct){
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
	public async Task<ISqlCmd> Prepare(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		, CT Ct
	){
		var Cmd = await MkCmd(DbFnCtx, Sql, Ct);
		return await Prepare(Cmd, Ct);
	}

	public async Task<ITxn> GetTxn(){
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		return Ans;
	}
}

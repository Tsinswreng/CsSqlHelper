using System.Data;
using Npgsql;
using Tsinswreng.CsSqlHelper.Cmd;

namespace Tsinswreng.CsSqlHelper.PostgreSql;

public class PostgreSqlCmdMkr
	:ISqlCmdMkr
	,IGetTxn
{
	public IDbConnection DbConnection{get;set;}
	public PostgreSqlCmdMkr(IDbConnection DbConnection){
		this.DbConnection = DbConnection;
	}

	public virtual async Task<ISqlCmd> MkCmd(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	){
		if(DbConnection is not NpgsqlConnection sqlConn){
			throw new InvalidOperationException("DbConnection is not NpgsqlConnection");
		}
		var RawCmd = sqlConn.CreateCommand();
		RawCmd.CommandText = Sql;
		var ans = new PostgreSqlCmd(RawCmd);
		if(DbFnCtx!= null){
			ans.WithCtx(DbFnCtx);
		}
		return ans;
	}

	public virtual async Task<ISqlCmd> Prepare(ISqlCmd Cmd, CT Ct){
		if(Cmd is not PostgreSqlCmd SqlCmd){
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

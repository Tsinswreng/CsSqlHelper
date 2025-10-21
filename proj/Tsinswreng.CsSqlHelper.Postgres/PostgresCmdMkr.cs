namespace Tsinswreng.CsSqlHelper.Postgres;
using System.Data;
using Npgsql;
using Tsinswreng.CsCore;
using IDbFnCtx = Tsinswreng.CsSqlHelper.IBaseDbFnCtx;

public partial class PostgresCmdMkr
	:ISqlCmdMkr
	,I_GetTxnAsy
{
	// public IDbConnection DbConnection{get;set;}
	// public PostgresCmdMkr(IDbConnection DbConnection){
	// 	this.DbConnection = DbConnection;
	// }

	public I_GetDbConnAsy DbConnGetter{get;set;}
	public PostgresCmdMkr(I_GetDbConnAsy DbConnGetter){
		this.DbConnGetter = DbConnGetter;
	}

	[Impl]
	public async Task<ISqlCmd> MkCmd(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	){
		var DbConnection = await DbConnGetter.GetConnAsy(Ct);
		if(DbConnection is not NpgsqlConnection sqlConn){
			throw new InvalidOperationException("DbConnection is not NpgsqlConnection");
		}
		var RawCmd = sqlConn.CreateCommand();
		RawCmd.CommandText = Sql;
		var ans = new PostgresCmd(RawCmd);
		if(DbFnCtx!= null){
			ans.WithCtx(DbFnCtx);
		}
		return ans;
	}

	/// <summary>
	/// Npgsql叵 先Prepare後傳參數。若需效sqlite芝prepare後多次傳異ʹ參數 宜用Batch
	/// </summary>
	[Impl]
	public async Task<ISqlCmd> Prepare(ISqlCmd Cmd, CT Ct){

		return Cmd;
		// if(Cmd is not PostgresCmd SqlCmd){
		// 	throw new InvalidOperationException("ISqlCmd is not SqliteCmd");
		// }
		// SqlCmd.RawCmd.Prepare();
		// return Cmd;
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
		IDbFnCtx? DbFnCtx
		,str Sql
		, CT Ct
	){
		var Cmd = await MkCmd(DbFnCtx, Sql, Ct);
		return await Prepare(Cmd, Ct);
	}

	public async Task<ITxn> GetTxnAsy(CT Ct){
		var DbConnection = await DbConnGetter.GetConnAsy(Ct);
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		return Ans;
	}
}

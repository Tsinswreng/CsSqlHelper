namespace Tsinswreng.CsSqlHelper.Postgres;
using System.Data;
using Npgsql;
using Tsinswreng.CsCore;
using IDbFnCtx = Tsinswreng.CsSqlHelper.IBaseDbFnCtx;

public partial class PostgresCmdMkr
	:ISqlCmdMkr
	,I_GetTxnAsy
{

	public IDbConnMgr DbConnGetter{get;set;}
	public PostgresCmdMkr(IDbConnMgr DbConnGetter){
		this.DbConnGetter = DbConnGetter;
	}

	[Impl]
	public async Task<ISqlCmd> MkCmd(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	){
		var DbConnection = DbFnCtx?.DbConn??await DbConnGetter.GetConnAsy(Ct);
		if(DbConnection is not NpgsqlConnection sqlConn){
			throw new InvalidOperationException("DbConnection is not NpgsqlConnection");
		}
		var RawCmd = sqlConn.CreateCommand();
		RawCmd.CommandText = Sql;
		var R = new PostgresCmd(RawCmd);
		R.Sql = Sql;
		if(DbFnCtx!= null){
			R.WithCtx(DbFnCtx);
		}
		R.FnsOnDispose.Add(async()=>{
			return await DbConnGetter.AfterUsingConnAsy(DbConnection, default);
		});
		return R;
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

	// [Obsolete]
	// public async Task<ITxn> GetTxnAsy(CT Ct){
	// 	var DbConnection = await DbConnGetter.GetConnAsy(Ct);
	// 	var Tx = DbConnection.BeginTransaction();
	// 	var Ans = new AdoTxn(Tx);
	// 	return Ans;
	// }


	public async Task<ITxn> GetTxnAsy(
		IBaseDbFnCtx Ctx, CT Ct
	){
		// pg中同一交易中 ʹ多條命令 須屬于同一連接。後MkCmd旹所用ʹ連接 亦 優先從Ctx.DbConn中取
		//事務過後 Ctx會Dispose 故每次開Ctx旹其DbConn必取自DbConnGetter
		var DbConnection = Ctx.DbConn??await DbConnGetter.GetConnAsy(Ct);
		//var DbConnection = await DbConnGetter.GetConnAsy(Ct);
		Ctx.DbConn = DbConnection;
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		Ctx.Txn = Ans;
		return Ans;
	}
}

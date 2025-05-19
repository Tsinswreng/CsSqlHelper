using System.Data;
using Microsoft.Data.Sqlite;
using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper.Cmd;



public class SqlCmdMkr
	:I_SqlCmdMkr
	,I_GetTxnAsy
{
	public IDbConnection DbConnection{get;set;}
	public SqlCmdMkr(IDbConnection DbConnection){
		this.DbConnection = DbConnection;
	}
	public async Task<I_SqlCmd> PrepareAsy(
		I_DbFnCtx? DbFnCtx
		,str Sql
		,CancellationToken ct
	){
		if(DbConnection is not SqliteConnection sqlConn){
			throw new InvalidOperationException("DbConnection is not SqlConnection");
		}
		var Cmd = sqlConn.CreateCommand();
		Cmd.CommandText = Sql;
		Cmd.Prepare();
		var ans = new SqliteCmd(Cmd);
		if(DbFnCtx!= null){
			ans.WithCtx(DbFnCtx);
		}
		return ans;
	}

	public async Task<I_TxnAsy> GetTxnAsy(){
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		return Ans;
	}
}

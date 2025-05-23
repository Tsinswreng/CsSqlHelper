using System.Data;
using Microsoft.Data.Sqlite;
using Ngaq.Core.Infra.Db;

namespace Tsinswreng.SqlHelper.Cmd;



public class SqlCmdMkr
	:ISqlCmdMkr
	,IGetTxnAsy
{
	public IDbConnection DbConnection{get;set;}
	public SqlCmdMkr(IDbConnection DbConnection){
		this.DbConnection = DbConnection;
	}
	public async Task<ISqlCmd> PrepareAsy(
		IDbFnCtx? DbFnCtx
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

	public async Task<ITxnAsy> GetTxnAsy(){
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		return Ans;
	}
}

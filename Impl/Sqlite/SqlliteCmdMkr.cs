using System.Data;
using Microsoft.Data.Sqlite;
using Ngaq.Core.Infra.Db;

namespace Tsinswreng.CsSqlHelper.Cmd;



public class SqliteCmdMkr
	:ISqlCmdMkr
	,IGetTxn
{
	public IDbConnection DbConnection{get;set;}
	public SqliteCmdMkr(IDbConnection DbConnection){
		this.DbConnection = DbConnection;
	}
	public async Task<ISqlCmd> Prepare(
		IBaseDbFnCtx? DbFnCtx
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

	public async Task<ITxn> GetTxn(){
		var Tx = DbConnection.BeginTransaction();
		var Ans = new AdoTxn(Tx);
		return Ans;
	}
}

using System.Data;
using Microsoft.Data.Sqlite;

namespace Tsinswreng.SqlHelper.Cmd;



public class SqlCmdMkr:I_SqlCmdMkr{
	public IDbConnection DbConnection{get;set;}
	public async Task<I_SqlCmd> PrepareAsy(
		str Sql
		,CancellationToken ct
	){
		if(DbConnection is not SqliteConnection sqlConn){
			throw new InvalidOperationException("DbConnection is not SqlConnection");
		}
		var Cmd = sqlConn.CreateCommand();
		Cmd.CommandText = Sql;
		Cmd.Prepare();
		var ans = new SqliteCmd(Cmd);
		return ans;
	}
}

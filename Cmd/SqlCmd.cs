using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;

namespace Tsinswreng.SqlHelper.Cmd;


public class SqliteCmd: I_SqlCmd{
	public SqliteCommand DbCmd{get;set;}
	public str? Sql{get;set;}
	public SqliteCmd(SqliteCommand DbCmd){
		this.DbCmd = DbCmd;
	}

/// <summary>
/// @名稱 佔位
/// </summary>
/// <param name="Params"></param>
/// <returns></returns>
	public SqliteCmd SetParams(IDictionary<str, object> Params){
		foreach(var (k,v) in Params){
			DbCmd.Parameters.AddWithValue("@"+k, v);
		}
		return this;
	}

/// <summary>
/// 「?」 佔位
/// </summary>
/// <param name="Params"></param>
/// <returns></returns>
	public SqliteCmd SetParams(IEnumerable<object> Params){
		foreach(var v in Params){
			DbCmd.Parameters.AddWithValue("?", v); //這対嗎?
		}
		return this;
	}

	public async IAsyncEnumerable<IDictionary<str, object>> RunAsy(
		[EnumeratorCancellation]
		CancellationToken ct
	){
		using var Reader = await DbCmd.ExecuteReaderAsync(ct);
		while(await Reader.ReadAsync(ct)){
			var RawDict = new Dictionary<str, object>();
			for(int i = 0; i < Reader.FieldCount; i++){
				RawDict.Add(Reader.GetName(i), Reader.GetValue(i));
			}
			yield return RawDict;
		}
	}
}

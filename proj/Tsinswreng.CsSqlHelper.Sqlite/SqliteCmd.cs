using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper.Sqlite;
using IDbFnCtx = Tsinswreng.CsSqlHelper.IBaseDbFnCtx;

public partial class SqliteCmd
	:ISqlCmd
	,IAsyncDisposable
{
	public SqliteCommand RawCmd{get;set;}
	public str? Sql{get;set;}
	public SqliteCmd(SqliteCommand DbCmd){
		RawCmd = DbCmd;
	}

	public ISqlCmd WithCtx(IDbFnCtx? Ctx){
		if(Ctx?.Txn?.RawTxn != null){
			RawCmd.Transaction = (SqliteTransaction)Ctx.Txn.RawTxn;
		}
		return this;
	}

	[Impl]
	public ISqlCmd ResolvedArgs(IDictionary<str, obj?> Args){
		RawCmd.Parameters.Clear();//不清空舊參數 續ˣ珩DbCmd蜮報錯
		foreach(var (k,v) in Args){
			RawCmd.Parameters.AddWithValue(k, CodeValToDbVal(v));
		}
		return this;
	}


/// <summary>
/// 傳入之字典不帶@名稱 佔位
/// </summary>
/// <param name="Args"></param>
/// <returns></returns>
	[Impl]
	public ISqlCmd RawArgs(IDictionary<str, obj?> Args){
		RawCmd.Parameters.Clear();//不清空舊參數 續ˣ珩DbCmd蜮報錯
		foreach(var (k,v) in Args){
			RawCmd.Parameters.AddWithValue("@"+k, CodeValToDbVal(v));
		}
		return this;
	}

/// <summary>
/// @0, @1, @2 ...
/// </summary>
/// <param name="Params"></param>
/// <returns></returns>
	public ISqlCmd Args(IEnumerable<obj?> Params){
		RawCmd.Parameters.Clear();
		var i = 0;
		foreach(var v in Params){
			RawCmd.Parameters.AddWithValue("@"+i, CodeValToDbVal(v));
		i++;}
		return this;
	}

/// <summary>
/// 若含null則做DBNull與null之轉、否則原樣返
/// </summary>
/// <param name="DbVal"></param>
/// <returns></returns>
	public obj? DbValToCodeVal(obj? DbVal){
		if(DbVal is DBNull){
			return null!;
		}
		return DbVal;
	}

	public obj? CodeValToDbVal(obj? CodeVal){
		if(CodeVal == null){
			return DBNull.Value;
		}
		return CodeVal;
	}

	public async IAsyncEnumerable<IDictionary<str, obj?>> Run(
		[EnumeratorCancellation]
		CT Ct
	){
		using var Reader = await RawCmd.ExecuteReaderAsync(Ct);
		while(await Reader.ReadAsync(Ct)){
			var RawDict = new Dictionary<str, obj?>();
			for(var i = 0; i < Reader.FieldCount; i++){
				RawDict.Add(Reader.GetName(i), DbValToCodeVal(Reader.GetValue(i)));
			}
			yield return RawDict;
		}
	}

	public void Dispose(){
		RawCmd.Dispose();
	}

	public ValueTask DisposeAsync() {
		RawCmd.Dispose();
		return default;
	}
}

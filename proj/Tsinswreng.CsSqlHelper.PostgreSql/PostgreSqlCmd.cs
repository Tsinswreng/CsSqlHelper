using System.Data;
using System.Runtime.CompilerServices;
using Npgsql;
using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper.PostgreSql;


public partial class PostgreSqlCmd: ISqlCmd{
	public NpgsqlCommand RawCmd{get;set;}
	public str? Sql{get;set;}
	public PostgreSqlCmd(NpgsqlCommand DbCmd){
		RawCmd = DbCmd;
	}


	public ISqlCmd WithCtx(IBaseDbFnCtx? Ctx){
		if(Ctx?.Txn?.RawTxn != null){
			RawCmd.Transaction = (NpgsqlTransaction)Ctx.Txn.RawTxn;
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
/// @名稱 佔位
/// </summary>
/// <param name="Params"></param>
/// <returns></returns>
	public ISqlCmd RawArgs(IDictionary<str, object?> Params){
		RawCmd.Parameters.Clear();//不清空舊參數 續ˣ珩DbCmd蜮報錯
		foreach(var (k,v) in Params){
			RawCmd.Parameters.AddWithValue("@"+k, CodeValToDbVal(v));
		}
		return this;
	}

/// <summary>
/// @0, @1, @2 ...
/// </summary>
/// <param name="Params"></param>
/// <returns></returns>
	public ISqlCmd Args(IEnumerable<object?> Params){
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
	public object? DbValToCodeVal(object? DbVal){
		if(DbVal is DBNull){
			return null!;
		}
		return DbVal;
	}

	public object? CodeValToDbVal(object? CodeVal){
		if(CodeVal == null){
			return DBNull.Value;
		}
		return CodeVal;
	}

	public async IAsyncEnumerable<IDictionary<str, object?>> IterIAsy(
		[EnumeratorCancellation]
		CT ct
	){
		using var Reader = await RawCmd.ExecuteReaderAsync(ct);
		while(await Reader.ReadAsync(ct)){
			var RawDict = new Dictionary<str, object?>();
			for(int i = 0; i < Reader.FieldCount; i++){
				RawDict.Add(Reader.GetName(i), DbValToCodeVal(Reader.GetValue(i)));
			}
			yield return RawDict;
		}
	}

	public async Task<IList<IDictionary<str, obj?>>> All(CT Ct){
		using var reader = await RawCmd.ExecuteReaderAsync(Ct);
		var result = new List<IDictionary<str, obj?>>();

		while (await reader.ReadAsync(Ct)){
			var row = new Dictionary<str, obj?>(reader.FieldCount);
			for (var i = 0; i < reader.FieldCount; i++)
				row[reader.GetName(i)] = DbValToCodeVal(reader.GetValue(i));
			result.Add(row);
		}
		return result;
	}


	public void Dispose(){
		RawCmd.Dispose();
	}
	public ValueTask DisposeAsync(){
		RawCmd.Dispose();
		return default;
	}

}

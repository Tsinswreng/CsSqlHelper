namespace Tsinswreng.CsSqlHelper.Postgres;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Npgsql;
using Tsinswreng.CsCore;

public partial class PostgresCmd: ISqlCmd{
	public IList<Func<Task<nil>>> FnsOnDispose{get;set;} = new List<Func<Task<nil>>>();
	public NpgsqlCommand RawCmd{get;set;}
	public str? Sql{get;set;}
	public PostgresCmd(NpgsqlCommand DbCmd){
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
		if(CodeVal is UInt64){
//  System.NotSupportedException: The CLR type System.UInt64 isn't natively supported by Npgsql or your PostgreSQL. To use it with a PostgreSQL composite you need to specify DataTypeName or to map it, please refer to the documentation.
			return Convert.ToInt64(CodeVal);
		}
		return CodeVal;
	}

	public async IAsyncEnumerable<IDictionary<str, object?>> IterAsyE(
		[EnumeratorCancellation]
		CT Ct
	){
		using var Reader = await RawCmd.ExecuteReaderAsync(Ct);
		while(await Reader.ReadAsync(Ct)){
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
			for (var i = 0; i < reader.FieldCount; i++){
				row[reader.GetName(i)] = DbValToCodeVal(reader.GetValue(i));
			}
			result.Add(row);
		}
		return result;
	}


	public async Task<IList<
		IList<IDictionary<str, obj?>>>
	> All2d(CT Ct){
		using var reader = await RawCmd.ExecuteReaderAsync(Ct);
		var result2d = new List<IList<IDictionary<str, obj?>>>();

		do{
			var result = new List<IDictionary<str, obj?>>();
			while (await reader.ReadAsync(Ct)){
				var row = new Dictionary<str, obj?>(reader.FieldCount);
				for (var i = 0; i < reader.FieldCount; i++){
					row[reader.GetName(i)] = DbValToCodeVal(reader.GetValue(i));
				}
				result.Add(row);
			}
			result2d.Add(result);
		}while(await reader.NextResultAsync(Ct));
		return result2d;
	}


	public void Dispose(){
		var R = DisposeAsync();
		// RawCmd.Dispose();
		// foreach(var fn in FnsOnDispose){
		// 	fn();
		// }
	}
	public async ValueTask DisposeAsync(){
		RawCmd.Dispose();
		foreach(var fn in FnsOnDispose){
			await fn();
		}
		return;
	}

}

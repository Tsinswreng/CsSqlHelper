using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using Tsinswreng.CsCore;
using Tsinswreng.CsTools;

namespace Tsinswreng.CsSqlHelper;
using IDbFnCtx = Tsinswreng.CsSqlHelper.IBaseDbFnCtx;

public abstract partial class BaseSqlCmd<
	TRawCmd ,TRawTxn
>
	:ISqlCmd
	,IAsyncDisposable
	where TRawCmd : DbCommand
	where TRawTxn : DbTransaction
{
	public TRawCmd RawCmd{get;set;}
	public IList<Func<Task<nil>>> FnsOnDispose{get;set;} = new List<Func<Task<nil>>>();
	public str? Sql{get;set;}
	public BaseSqlCmd(TRawCmd DbCmd){
		RawCmd = DbCmd;
	}

	public virtual ISqlCmd AttachCtxTxn(ITxn Txn)
	{
		if(Txn.RawTxn is not TRawTxn RawTxn){
			throw new ArgumentException("Txn.RawTxn is not TRawTxn RawTxn");
		}
		RawCmd.Transaction = RawTxn;
		return this;
	}

	public virtual ISqlCmd AttachCtxTxn(IDbFnCtx? Ctx){
		if(Ctx?.Txn is not null){
			AttachCtxTxn(Ctx.Txn);
		}
		return this;
	}

	public abstract nil ParamAddWithValue(
		DbParameterCollection Params, string? parameterName, object? value
	);

	public abstract str ToResolvedArg(str RawArg);
	//e.g return "@"+RawArg

	[Impl]
	public ISqlCmd ResolvedArgs(IDictionary<str, obj?> Args){
		RawCmd.Parameters.Clear();//不清空舊參數 續ˣ珩DbCmd蜮報錯
		foreach(var (k,v) in Args){
			this.ParamAddWithValue(
				RawCmd.Parameters
				,k, CodeValToDbVal(v)
			);
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
			this.ParamAddWithValue(
				RawCmd.Parameters
				,ToResolvedArg(k), CodeValToDbVal(v)
			);
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
			//RawCmd.Parameters.AddWithValue("@"+i, CodeValToDbVal(v));
			this.ParamAddWithValue(
				RawCmd.Parameters
				,ToResolvedArg(i+""), CodeValToDbVal(v)
			);
		i++;}
		return this;
	}

	/// <summary>
	/// 若含null則做DBNull與null之轉、否則原樣返
	/// </summary>
	/// <param name="DbVal"></param>
	/// <returns></returns>
	public virtual obj? DbValToCodeVal(obj? DbVal){
		if(DbVal is DBNull){
			return null!;
		}
		return DbVal;
	}

	public virtual obj? CodeValToDbVal(obj? CodeVal){
		if(CodeVal == null){
			return DBNull.Value;
		}
		return CodeVal;
	}

	public virtual async IAsyncEnumerable<IDictionary<str, obj?>> IterAsyE(
		[EnumeratorCancellation]
		CT Ct
	){
		using var Dl = new DisposableList();
		DbDataReader Reader = null!;
		try{
			Reader = await RawCmd.ExecuteReaderAsync(Ct);
			Dl.Add(Reader);
		}
		catch (System.Exception e){
			var Err = new DbErr(
				e.Message
				+"\nSql:\n"+Sql
				+"\nParams"+RawCmd.Parameters.ToReadableString()
				,e
			);
			Dl.Dispose();
			throw Err;
		}
		while(await Reader.ReadAsync(Ct)){
			var RawDict = new Dictionary<str, obj?>();
			for(var i = 0; i < Reader.FieldCount; i++){
				RawDict.Add(Reader.GetName(i), DbValToCodeVal(Reader.GetValue(i)));
			}
			yield return RawDict;
		}
	}

	public virtual async Task<IList<IDictionary<str, obj?>>> All(CT Ct){
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

	public ValueTask DisposeAsync() {
		RawCmd.Dispose();
		return default;
	}
}

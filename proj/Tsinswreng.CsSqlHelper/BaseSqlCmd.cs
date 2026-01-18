using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using Tsinswreng.CsCore;
using Tsinswreng.CsTools;

namespace Tsinswreng.CsSqlHelper;

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

	public virtual ISqlCmd AttachCtxTxn(ITxn Txn){
		if(Txn.RawTxn is not TRawTxn RawTxn){
			throw new ArgumentException("Txn.RawTxn is not TRawTxn RawTxn");
		}
		RawCmd.Transaction = RawTxn;
		return this;
	}

	public virtual ISqlCmd AttachCtxTxn(IDbFnCtx Ctx){
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

	public virtual async IAsyncEnumerable<
		IAsyncEnumerable<IDictionary<str, obj?>>
	> AsyE2d(
		[EnumeratorCancellation]
		CT Ct
	){
		using var Dl = new DisposableList();
		DbDataReader Reader = null!;
		try{
			// 执行查询获取Reader，和原代码逻辑完全一致
			Reader = await RawCmd.ExecuteReaderAsync(Ct);
			Dl.Add(Reader);
		}
		catch (System.Exception e){
			// 原代码的异常封装逻辑，一行不改
			var Err = new DbErr(
				e.Message
				+ "\nSql:\n" + Sql
				+ "\nParams" + RawCmd.Parameters.ToReadableString()
				, e
			);
			Dl.Dispose();
			throw Err;
		}

		// do-while 是多结果集的标准循环，先读第一个结果集，再切换
		do{
			// ★ 核心：为【当前结果集】创建一个「内层异步枚举器」，读取当前结果集的所有行
			// 外层直接 yield return 这个内层枚举器，完美实现二维IAsyncEnumerable
			yield return ReadCurrentResultSetRowsAsync(Reader, Ct);

			// ★ 切换到下一个结果集，带取消令牌，无则退出循环
		} while (await Reader.NextResultAsync(Ct));
	}

	// ★ 私有内层方法：负责读取【单个结果集】的所有行，返回单行数据的异步枚举
	private async IAsyncEnumerable<IDictionary<str, obj?>> ReadCurrentResultSetRowsAsync(
		DbDataReader reader,
		[EnumeratorCancellation]
		CT Ct
	){
		// 读取当前结果集的每一行，和你原IterAsyE的单行读取逻辑完全一致
		while (await reader.ReadAsync(Ct)){
			var RawDict = new Dictionary<str, obj?>();
			for (var i = 0; i < reader.FieldCount; i++){
//TODO 優化異常ʹ訊
// reader.GetName(i)ʃ得ʹ列名ˋ不帶表名、如Select* 後作多表join 遇同名列則褈添ⁿ致錯、Select A.Id, B.Id旹亦然
				RawDict.Add(reader.GetName(i), DbValToCodeVal(reader.GetValue(i)));
			}
			yield return RawDict;
		}
	}

	/// <summary>
	/// 多個結果集ʹ內容ˇ 皆扁平化
	/// </summary>
	/// <param name="Ct"></param>
	/// <returns></returns>
	public virtual async IAsyncEnumerable<IDictionary<str, obj?>> AsyE1d(
		[EnumeratorCancellation]
		CT Ct
	){
		var d2 = AsyE2d(Ct);
		await foreach (var d1 in d2){
			await foreach (var row in d1){
				yield return row;
			}
		}
	}


	public virtual async Task<IList<IDictionary<str, obj?>>> All1d(CT Ct){
		return await AsyE1d(Ct).ToListAsync(Ct);
	}


	public void Dispose(){
		RawCmd.Dispose();
	}

	public ValueTask DisposeAsync() {
		RawCmd.Dispose();
		return default;
	}
}

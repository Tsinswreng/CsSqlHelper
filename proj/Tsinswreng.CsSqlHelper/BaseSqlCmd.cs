// using System.Collections.Generic;
// using System.Data;
// using System.Runtime.CompilerServices;
// using System.Threading;
// using System.Threading.Tasks;
// using Tsinswreng.CsCore;

// namespace Tsinswreng.CsSqlHelper;

// using IDbFnCtx = IBaseDbFnCtx;

// public abstract class BaseSqlCmd<TCommand> : ISqlCmd
//     where TCommand : class, IDbCommand, IDisposable
// {
//     public IList<Func<Task<nil>>> FnsOnDispose { get; set; } = new List<Func<Task<nil>>>();
//     public TCommand RawCmd { get; protected set; }
//     public str? Sql { get; set; }

//     protected BaseSqlCmd(TCommand dbCmd)
//     {
//         RawCmd = dbCmd;
//     }

//     public virtual ISqlCmd WithCtx(IDbFnCtx? Ctx)
//     {
//         if (Ctx?.Txn?.RawTxn != null)
//         {
//             SetTransaction((dynamic)Ctx.Txn.RawTxn);
//         }
//         return this;
//     }

//     [Impl]
//     public virtual ISqlCmd ResolvedArgs(IDictionary<str, obj?> Args)
//     {
//         ClearParameters();//不清空舊參數 續ˣ珩DbCmd蜮報錯
//         foreach (var (k, v) in Args)
//         {
//             AddParameter(k, CodeValToDbVal(v));
//         }
//         return this;
//     }

//     /// <summary>
//     /// 傳入之字典不帶@名稱 佔位
//     /// </summary>
//     /// <param name="Args"></param>
//     /// <returns></returns>
//     [Impl]
//     public virtual ISqlCmd RawArgs(IDictionary<str, obj?> Args)
//     {
//         ClearParameters();//不清空舊參數 續ˣ珩DbCmd蜮報錯
//         foreach (var (k, v) in Args)
//         {
//             AddParameter("@" + k, CodeValToDbVal(v));
//         }
//         return this;
//     }

//     /// <summary>
//     /// @0, @1, @2 ...
//     /// </summary>
//     /// <param name="Params"></param>
//     /// <returns></returns>
//     public virtual ISqlCmd Args(IEnumerable<obj?> Params)
//     {
//         ClearParameters();
//         var i = 0;
//         foreach (var v in Params)
//         {
//             AddParameter("@" + i, CodeValToDbVal(v));
//             i++;
//         }
//         return this;
//     }

//     /// <summary>
//     /// 若含null則做DBNull與null之轉、否則原樣返
//     /// </summary>
//     /// <param name="DbVal"></param>
//     /// <returns></returns>
//     public virtual obj? DbValToCodeVal(obj? DbVal)
//     {
//         if (DbVal is DBNull)
//         {
//             return null!;
//         }
//         return DbVal;
//     }

//     public virtual obj? CodeValToDbVal(obj? CodeVal)
//     {
//         if (CodeVal == null)
//         {
//             return DBNull.Value;
//         }
//         return CodeVal;
//     }

//     /// <summary>
//     /// 异步枚举读取结果
//     /// </summary>
//     public async IAsyncEnumerable<IDictionary<str, obj?>> IterAsyE(
//         [EnumeratorCancellation]
//         CancellationToken Ct
//     )
//     {
//         using var reader = await RawCmd.ExecuteReaderAsync(Ct).ConfigureAwait(false);
//         while (await reader.ReadAsync(Ct).ConfigureAwait(false))
//         {
//             var row = new Dictionary<str, obj?>();
//             for (var i = 0; i < reader.FieldCount; i++)
//             {
//                 row.Add(reader.GetName(i), DbValToCodeVal(reader.GetValue(i)));
//             }
//             yield return row;
//         }
//     }

//     /// <summary>
//     /// 读取所有结果到列表
//     /// </summary>
//     public async Task<IList<IDictionary<str, obj?>>> All(CancellationToken Ct)
//     {
//         using var reader = await RawCmd.ExecuteReaderAsync(Ct).ConfigureAwait(false);
//         var result = new List<IDictionary<str, obj?>>();

//         while (await reader.ReadAsync(Ct).ConfigureAwait(false))
//         {
//             var row = new Dictionary<str, obj?>(reader.FieldCount);
//             for (var i = 0; i < reader.FieldCount; i++)
//             {
//                 row[reader.GetName(i)] = DbValToCodeVal(reader.GetValue(i));
//             }
//             result.Add(row);
//         }
//         return result;
//     }

//     public virtual void Dispose()
//     {
//         RawCmd.Dispose();
//     }

//     public virtual ValueTask DisposeAsync()
//     {
//         RawCmd.Dispose();
//         return default;
//     }

//     #region 抽象方法（子类实现数据库差异化逻辑）
//     protected abstract void ClearParameters();
//     protected abstract void AddParameter(string name, object? value);
//     protected abstract void SetTransaction(dynamic transaction);
//     #endregion
// }

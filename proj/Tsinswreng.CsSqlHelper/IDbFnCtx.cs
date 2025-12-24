using System.Data;

namespace Tsinswreng.CsSqlHelper;

public partial interface IBaseDbFnCtx
	:IAsyncDisposable
{
	public ITxn? Txn{get;set;}
	public IDbConnection? DbConn{get;set;}
	public IDictionary<str, obj?>? Props{get;set;}
	public ICollection<obj?>? ObjsToDispose{get;set;}
#if Impl
	 = new List<obj?>();
#endif
	async ValueTask IAsyncDisposable.DisposeAsync(){
		if(ObjsToDispose == null){return;}
		foreach(var obj in ObjsToDispose){
			if(obj is IAsyncDisposable DispAsy){
				await DispAsy.DisposeAsync();
			}else if(obj is IDisposable Disp){
				Disp.Dispose();
			}
		}
	}
}

public static partial class ExtnIBaseDbFnCtx{
	extension<TSelf>(TSelf z)
		where TSelf: IBaseDbFnCtx
	{
		private TSelf _AddToDispose(
			obj? Disposable
		){
			z.ObjsToDispose??=new List<obj?>();
			z.ObjsToDispose.Add(Disposable);
			return z;
		}

		public TSelf AddToDispose(
			IDisposable Disposable
		){
			return z._AddToDispose(Disposable);
		}

		public TSelf AddToDisposeAsy(
			IAsyncDisposable Disposable
		){
			return z._AddToDispose(Disposable);
		}

		public TSelf AddToDispose(
			IEnumerable<obj?> DisposableObjs
		){
			z.ObjsToDispose??=new List<obj?>();
			foreach(var obj in DisposableObjs){
				z.ObjsToDispose.Add(obj);
			}
			return z;
		}
	}

	extension<TSelf>(TSelf z)
		where TSelf: IBaseDbFnCtx
	{
		/// <summary>
		/// Prepare並AddToDispose
		/// </summary>
		/// <typeparam name="TSelf"></typeparam>
		/// <param name="z"></param>
		/// <param name="CmdMkr"></param>
		/// <param name="Sql"></param>
		/// <param name="Ct"></param>
		/// <returns></returns>
		public async Task<ISqlCmd> PrepareToDispose(
			ISqlCmdMkr CmdMkr
			,str Sql
			,CT Ct
		){
			var R = await CmdMkr.Prepare(z, Sql, Ct);
			z?.AddToDispose(R);
			return R;
		}
		public ISqlCmd RunCmd(
			ISqlCmd Cmd
			,IArgDict Arg
		){
			Cmd.WithCtx(z).Args(Arg);
			return Cmd;
		}
	}


	// public static async Task<ISqlCmd> Prepare(
	// 	this IBaseDbFnCtx z
	// 	,ISqlCmdMkr SqlCmdMkr
	// 	,str Sql
	// 	,CT Ct
	// ){
	// 	var R = await SqlCmdMkr.Prepare(z, Sql, Ct);
	// 	z.AddToDispose(R);
	// 	return R;
	// }
}

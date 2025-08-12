namespace Tsinswreng.CsSqlHelper;

public partial interface IBaseDbFnCtx
	:IAsyncDisposable
{
	public ITxn? Txn{get;set;}
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
	private static TSelf _AddToDispose<TSelf>(
		this TSelf z
		,obj? Disposable
	)where TSelf: IBaseDbFnCtx{
		z.ObjsToDispose??=new List<obj?>();
		z.ObjsToDispose.Add(Disposable);
		return z;
	}

	public static TSelf AddToDispose<TSelf>(
		this TSelf z
		,IDisposable Disposable
	)where TSelf: IBaseDbFnCtx{
		return z._AddToDispose(Disposable);
	}

	public static TSelf AddToDisposeAsy<TSelf>(
		this TSelf z
		,IAsyncDisposable Disposable
	)where TSelf: IBaseDbFnCtx{
		return z._AddToDispose(Disposable);
	}

	public static TSelf AddToDispose<TSelf>(
		this TSelf z
		,IEnumerable<obj?> DisposableObjs
	)where TSelf: IBaseDbFnCtx{
		z.ObjsToDispose??=new List<obj?>();
		foreach(var obj in DisposableObjs){
			z.ObjsToDispose.Add(obj);
		}
		return z;
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

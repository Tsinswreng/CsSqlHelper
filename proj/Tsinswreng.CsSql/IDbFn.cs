
namespace Tsinswreng.CsSqlHelper;

[Obsolete]
public interface IDbFn:IAsyncDisposable{
	public IDbFnCtx? Ctx{get;set;}
}
[Obsolete]
public class DbFn : IDbFn {
	public ISqlCmd? SqlCmd{get;set;}
	public IDbFnCtx? Ctx{get;set;}
	public ICollection<obj?> ObjsToDispose{get;set;} = new List<obj?>();
	public async ValueTask DisposeAsync() {
		if(SqlCmd is IAsyncDisposable disposable){
			await disposable.DisposeAsync();
		}
		foreach(var obj in ObjsToDispose){
			if(obj is IAsyncDisposable disposable2){
				await disposable2.DisposeAsync();
			}
		}
	}
}
[Obsolete]
public class DbFn<TRtn>:DbFn{
	public Func<CT, Task<TRtn>> FnRun{get;set;} = null!;
}
[Obsolete]
public class DbFn<TArg0, TRtn>:DbFn{
	public Func<TArg0, CT, Task<TRtn>> FnRun{get;set;} = null!;
}
[Obsolete]
public class DbFn<TArg0, TArg1, TRtn>:DbFn{
	public Func<TArg0, TArg1, CT, Task<TRtn>> FnRun{get;set;} = null!;
}
[Obsolete]
public class DbFn<TArg0, TArg1, TArg2, TRtn>:DbFn{
	public Func<TArg0, TArg1, TArg2, CT, Task<TRtn>> FnRun{get;set;} = null!;
}
[Obsolete]
public class DbFn<TArg0, TArg1, TArg2, TArg3, TRtn>:DbFn{
	public Func<TArg0, TArg1, TArg2, TArg3, CT, Task<TRtn>> FnRun{get;set;} = null!;
}
// public class Test{
// 	public void T(){
// 		IDbFn<Func<
// 			CT
// 			,Task<nil>
// 		>> DbFn;
// 	}
// }

namespace Tsinswreng.CsSqlHelper;

public class EvtArgDispose: EventArgs{

}



public partial interface ISqlCmd: IDisposable, IAsyncDisposable{
	/// <summary>
	/// dispose旹 額外ᵈ珩之諸操作
	/// </summary>
	public IList<Func<Task<nil>>> FnsOnDispose{get;set;}
	public str? Sql{get;set;}
	public IAsyncEnumerable<IDictionary<str, obj?>> IterIAsy(CT Ct);
	public Task<IList<IDictionary<str, obj?>>> All(CT Ct);

	/// <summary>
	/// raw arg name to value
	/// {
	/// 	"arg1": val1,
	/// 	"arg2": val2
	/// }
	/// </summary>
	/// <param name="Args"></param>
	/// <returns></returns>
	public ISqlCmd RawArgs(IDictionary<str, obj?> Args);
	/// <summary>
	/// resolved arg string(with prefix, e.g "@" for sqlite) to value
	/// {
	/// 	"@arg1": val1,
	/// 	"@arg2": val2
	/// }
	/// </summary>
	/// <param name="Args"></param>
	/// <returns></returns>
	public ISqlCmd ResolvedArgs(IDictionary<str, obj?> Args);
	public ISqlCmd Args(IEnumerable<obj?> Args);
	public ISqlCmd Args(IArgDict Args){
		return this.RawArgs(Args.ToDict());
	}
	public ISqlCmd WithCtx(IBaseDbFnCtx? DbFnCtx);

}

public static class ExtnISqlCmd{
	/// <summary>
	/// Prepare並AddToDispose
	/// </summary>
	/// <typeparam name="TSelf"></typeparam>
	/// <param name="z"></param>
	/// <param name="CmdMkr"></param>
	/// <param name="Sql"></param>
	/// <param name="Ct"></param>
	/// <returns></returns>
	public static async Task<ISqlCmd> PrepareToDispose<TSelf>(
		this TSelf? z
		,ISqlCmdMkr CmdMkr
		,str Sql
		,CT Ct
	)where TSelf: IBaseDbFnCtx{
		var R = await CmdMkr.Prepare(z, Sql, Ct);
		z?.AddToDispose(R);
		return R;
	}
	public static ISqlCmd Attach<TSelf>(
		this TSelf? z
		,ISqlCmd Cmd
		,IArgDict Arg
	)
		where TSelf: IBaseDbFnCtx
	{
		Cmd.WithCtx(z).Args(Arg);
		return Cmd;
	}
	public static async Task<IDictionary<str, obj?>> DictFirst(
		this ISqlCmd z, CT Ct
	){
		var AsyE = z.IterIAsy(Ct);
		var i = 0;
		IDictionary<str, obj?> R = null!;
		await foreach(var e in AsyE){
			if(i >= 1){
				break;
			}
			R = e;
			i++;
		}
		if(i == 0){
			throw new InvalidOperationException("no result");
		}
		return R;
	}

	public static async Task<T?> FirstOrDefault<T>(
		this ISqlCmd z, ITable Tbl, CT Ct
	)where T:new(){
		var AsyE = z.IterIAsy(Ct);
		var i = 0;
		IDictionary<str, obj?> Dict = null!;
		await foreach(var e in AsyE){
			if(i >= 1){
				break;
			}
			Dict = e;
			i++;
		}
		if(i == 0){
			return default;
		}
		var R = Tbl.DbDictToEntity<T>(Dict);
		return R;
	}

	public static async Task<T?> First<T>(
		this ISqlCmd z, ITable Tbl, CT Ct
	)where T:new(){
		var R = await z.FirstOrDefault<T>(Tbl, Ct);
		if(R is null){
			throw new InvalidOperationException("no result");
		}
		return R;
	}



	public static async Task<IDictionary<str, obj?>?> DictFirstOrDefault(
		this ISqlCmd z, CT Ct
	){
		var AsyE = z.IterIAsy(Ct);
		var i = 0;
		IDictionary<str, obj?> R = null!;
		await foreach(var e in AsyE){
			if(i >= 1){
				break;
			}
			R = e;
			i++;
		}
		if(i == 0){
			return null;
		}
		return R;
	}


}

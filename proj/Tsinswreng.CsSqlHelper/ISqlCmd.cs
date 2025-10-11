namespace Tsinswreng.CsSqlHelper;

public partial interface ISqlCmd: IDisposable, IAsyncDisposable{
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
			throw new InvalidOperationException("no result");
		}
		var R = Tbl.DbDictToEntity<T>(Dict);
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

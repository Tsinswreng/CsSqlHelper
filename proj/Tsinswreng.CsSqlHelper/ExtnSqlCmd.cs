namespace Tsinswreng.CsSqlHelper;
public static class ExtnSqlCmd{
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
	public static ISqlCmd RunCmd<TSelf>(
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
		var AsyE = z.IterAsyE(Ct);
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
		var AsyE = z.IterAsyE(Ct);
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
		var AsyE = z.IterAsyE(Ct);
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

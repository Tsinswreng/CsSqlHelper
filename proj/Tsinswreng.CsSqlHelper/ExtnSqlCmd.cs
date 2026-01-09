namespace Tsinswreng.CsSqlHelper;
public static class ExtnSqlCmd{


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


	extension(ISqlCmd z){
		public ISqlCmd WithCtx(IDbFnCtx? Ctx){
			if(Ctx is null){
				return z;
			}
			z.AttachCtxTxn(Ctx);
			Ctx.AddToDispose(z);
			return z;
		}

		public async Task<IAsyncEnumerable<T>> IterAsyE<T>(
			ITable Tbl, CT Ct
		)where T:new()
		{
			var allRaw = z.IterAsyE(Ct);
			return allRaw.Select(x=>Tbl.DbDictToEntity<T>(x));
		}

		public async Task<T?> FirstOrDefault<T>(
			ITable Tbl, CT Ct
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

		public async Task<T?> First<T>(
			ITable Tbl, CT Ct
		)where T:new(){
			var R = await z.FirstOrDefault<T>(Tbl, Ct);
			if(R is null){
				throw new InvalidOperationException("no result");
			}
			return R;
		}



		public async Task<IDictionary<str, obj?>?> DictFirstOrDefault(
			CT Ct
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
}

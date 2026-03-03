namespace Tsinswreng.CsSqlHelper;
public static class ExtnSqlCmd{
	extension(ISqlCmd z){
		
		public async IAsyncEnumerable<IDictionary<str, obj?>> AsyE1d(CT ct){
			var R = await z.ExeReader(ct);
			var itbl = R.AsyE1d(ct);
			await foreach(var e in itbl){
				yield return e;
			}
		}
		public async Task<IList<IDictionary<str, obj?>>> All1d(CT ct){
			var R = await z.ExeReader(ct);
			return await R.All1d(ct);
		}
		public async IAsyncEnumerable<IAsyncEnumerable<IDictionary<str, obj?>>> AsyE2d(CT ct){
			var R = await z.ExeReader(ct);
			var itbl = R.AsyE2d(ct);
			await foreach(var e in itbl){
				yield return e;
			}
		}
		public async Task<IList<IList<IDictionary<str, obj?>>>> All2d(CT ct){
			var R = await z.ExeReader(ct);
			return await R.All2d(ct);
		}
			
		
		public async Task<IDictionary<str, obj?>> DictFirst(
			CT Ct
		){
			var AsyE = z.AsyE1d(Ct);
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
			var allRaw = z.AsyE1d(Ct);
			return allRaw.Select(x=>Tbl.DbDictToEntity<T>(x));
		}

		public async Task<T?> FirstOrDefault<T>(
			ITable Tbl, CT Ct
		)where T:new(){
			var AsyE = z.AsyE1d(Ct);
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
			var AsyE = z.AsyE1d(Ct);
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

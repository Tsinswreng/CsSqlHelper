namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsSqlHelper;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

public static class ExtnISqlCmdMkr{
	extension(ISqlCmdMkr z){
		public AutoBatch<TItem, TRet> AutoBatch<TItem, TRet>(
			IDbFnCtx Ctx,
			ISqlDuplicator SqlDuplicator,
			Func<AutoBatch<TItem, TRet>, IList<TItem>, CT, Task<TRet>> FnAsy,
			u64 BatchSize = 0
		)
		{
			if(BatchSize == 0){
				if(z.DbSrcType.Eq(EDbSrcType.Sqlite)){
					BatchSize = 1;
				}else{
					BatchSize = 500;
				}
			}
			return CsSqlHelper.AutoBatch<TItem, TRet>.Mk(Ctx, z, SqlDuplicator, FnAsy, BatchSize);
		}
		
		public async IAsyncEnumerable<
			IDictionary<str, obj?>
		> RunSql<E>(//AsyE1d
			IDbFnCtx Ctx
			,ISqlSplicer<E> Sql
			,[EnumeratorCancellation] CT Ct
		){
			var BatchSize = z.DbSrcType.Eq(EDbSrcType.Sqlite) ? 1ul : 500ul;
			var binders = Sql.ParamAutoBinders;
			var manyBinders = binders
				.Where(x=>x is IParamAutoBinderManyValuesBatch)
				.Cast<IParamAutoBinderManyValuesBatch>()
				.ToList();
			var oneBinders = binders.Where(x=>x is not IParamAutoBinderManyValuesBatch).ToList();

			ISqlCmd? fullBatchCmd = null;
			ISqlCmd? finalBatchCmd = null;
			try{
				if(manyBinders.Count == 0){
					var cmd = await z.Prepare(Ctx, Sql.ToSqlStr(1), Ct);
					finalBatchCmd = cmd;
					var args = ArgDict.Mk(Sql.Tbl);
					foreach(var binder in oneBinders){
						binder.Bind(Sql.Tbl, args, new List<obj?>());
					}
					var d2 = cmd.Args(args).AsyE2d(Ct);
					await foreach(var d1 in d2.WithCancellation(Ct)){
						var hasAny = false;
						await foreach(var row in d1.WithCancellation(Ct)){
							hasAny = true;
							yield return row;
						}
						if(!hasAny){
							yield return null!;
						}
					}
					yield break;
				}

				while(true){
					if(!manyBinders[0].TryTakeBatch(BatchSize, out var firstBatch)){
						break;
					}
					var cnt = (u64)firstBatch.Count;
					var batches = new List<IList>{firstBatch};
					for(var i=1; i<manyBinders.Count; i++){
						if(!manyBinders[i].TryTakeBatch(cnt, out var batchI)){
							throw new InvalidOperationException("ParamAutoBinder.Many(...) length mismatch.");
						}
						if((u64)batchI.Count != cnt){
							throw new InvalidOperationException("ParamAutoBinder.Many(...) length mismatch.");
						}
						batches.Add(batchI);
					}

					ISqlCmd cmd;
					if(cnt == BatchSize){
						fullBatchCmd ??= await z.Prepare(Ctx, Sql.ToSqlStr(BatchSize), Ct);
						cmd = fullBatchCmd;
					}else{
						if(finalBatchCmd != null && !ReferenceEquals(finalBatchCmd, fullBatchCmd)){
							await finalBatchCmd.DisposeAsync();
						}
						finalBatchCmd = await z.Prepare(Ctx, Sql.ToSqlStr(cnt), Ct);
						cmd = finalBatchCmd;
					}

					var args = ArgDict.Mk(Sql.Tbl);
					foreach(var binder in oneBinders){
						binder.Bind(Sql.Tbl, args, firstBatch);
					}
					for(var i=0; i<manyBinders.Count; i++){
						manyBinders[i].BindBatch(Sql.Tbl, args, batches[i]);
					}

					var d2 = cmd.Args(args).AsyE2d(Ct);
					await foreach(var d1 in d2.WithCancellation(Ct)){
						var hasAny = false;
						await foreach(var row in d1.WithCancellation(Ct)){
							hasAny = true;
							yield return row;
						}
						if(!hasAny){
							yield return null!;
						}
					}
				}
			}finally{
				if(finalBatchCmd != null && !ReferenceEquals(finalBatchCmd, fullBatchCmd)){
					await finalBatchCmd.DisposeAsync();
				}
				if(fullBatchCmd != null){
					await fullBatchCmd.DisposeAsync();
				}
			}
		}
		
	}
}

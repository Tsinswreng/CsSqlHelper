namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsSqlHelper;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

public static class ExtnISqlCmdMkr{
	extension(ISqlCmdMkr z){
		[Doc(@$"
#Sum[Create auto batch helper with provider-based default size]
#Params([Db function context],[SQL duplicator],[Batch execution delegate],[Batch size, 0 means auto])
#TParams([Batch item type],[Batch return type])
#Rtn[{nameof(AutoBatch<TItem, TRet>)} instance]
")]
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
		
		[Doc(@$"
#Sum[Execute auto-bound duplicated SQL lazily and flatten as row stream]
#Params([Db function context],[SQL splicer carrying auto binders],[Cancellation token])
#Rtn[Flattened async row stream; emits null placeholder for empty result-set]
#See([{nameof(ISqlSplicer.ParamAutoBinders)}],[{nameof(IParamAutoBinderManyValuesBatch)}])
")]
		public async IAsyncEnumerable<
			IDictionary<str, obj?>
		> RunSql(//AsyE1d
			IDbFnCtx Ctx
			,ISqlSplicer Sql
			,[EnumeratorCancellation] CT Ct
		){
			// Keep same default policy as AutoBatch helper.
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
				// No Many binder: run once with fixed binders only.
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
						// Preserve 1:1 "input sql segment -> output element" shape for caller mapping.
						if(!hasAny){
							yield return null!;
						}
					}
					yield break;
				}

				while(true){
					// Take one logical batch from first sequence binder.
					if(!manyBinders[0].TryTakeBatch(BatchSize, out var firstBatch)){
						break;
					}
					var cnt = (u64)firstBatch.Count;
					var batches = new List<IList>{firstBatch};
					// All Many binders must have the same batch length.
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
						// Reuse prepared full-batch command.
						fullBatchCmd ??= await z.Prepare(Ctx, Sql.ToSqlStr(BatchSize), Ct);
						cmd = fullBatchCmd;
					}else{
						// Tail batch needs different duplicated SQL; prepare a separate command.
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

					// Iterate per result-set to detect empty set and emit null placeholder.
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

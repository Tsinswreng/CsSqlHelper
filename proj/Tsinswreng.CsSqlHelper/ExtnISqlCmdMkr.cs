namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsSqlHelper;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

public static class ExtnISqlCmdMkr{
	private static (
		IList<IParamAutoBinderManyValuesBatch> ManyBinders,
		IList<IParamAutoBinder> OneBinders
	) SplitBinders(IList<IParamAutoBinder> binders){
		var many = binders
			.Where(x=>x is IParamAutoBinderManyValuesBatch)
			.Cast<IParamAutoBinderManyValuesBatch>()
			.ToList();
		var one = binders.Where(x=>x is not IParamAutoBinderManyValuesBatch).ToList();
		return (many, one);
	}

	private static async IAsyncEnumerable<IDictionary<str, obj?>> YieldRowsOrNull(
		ISqlCmd cmd,
		IArgDict args,
		[EnumeratorCancellation] CT ct
	){
		// Iterate per result-set to preserve one-slot output for empty result-sets.
		var d2 = cmd.Args(args).AsyE2d(ct);
		await foreach(var d1 in d2.WithCancellation(ct)){
			var hasAny = false;
			await foreach(var row in d1.WithCancellation(ct)){
				hasAny = true;
				yield return row;
			}
			if(!hasAny){
				yield return null!;
			}
		}
	}

	private static async Task<ISqlCmd> GetCmdForBatch(
		ISqlCmdMkr mkr,
		IDbFnCtx ctx,
		IAutoBindSqlDuplicator sql,
		u64 batchSize,
		u64 cnt,
		ISqlCmd? fullBatchCmd,
		CT ct
	){
		if(cnt == batchSize){
			fullBatchCmd ??= await mkr.Prepare(ctx, sql.DuplicateSql(batchSize), ct);
			return fullBatchCmd;
		}
		return await mkr.Prepare(ctx, sql.DuplicateSql(cnt), ct);
	}

	private static IArgDict BuildArgsForBatch(
		IList<IParamAutoBinder> oneBinders,
		IList<IParamAutoBinderManyValuesBatch> manyBinders,
		IList firstBatch,
		IList<IList> batches
	){
		var args = ArgDict.Mk();
		foreach(var binder in oneBinders){
			binder.Bind(args, firstBatch);
		}
		for(var i=0; i<manyBinders.Count; i++){
			manyBinders[i].BindBatch(args, batches[i]);
		}
		return args;
	}

	private static bool TryTakeAlignedBatches(
		IList<IParamAutoBinderManyValuesBatch> manyBinders,
		u64 batchSize,
		out IList firstBatch,
		out IList<IList> batches
	){
		firstBatch = new List<obj?>();
		batches = new List<IList>();
		if(!manyBinders[0].TryTakeBatch(batchSize, out firstBatch)){
			return false;
		}
		var cnt = (u64)firstBatch.Count;
		batches = new List<IList>{firstBatch};
		for(var i=1; i<manyBinders.Count; i++){
			if(!manyBinders[i].TryTakeBatch(cnt, out var batchI)){
				throw new InvalidOperationException("ParamAutoBinder.Many(...) length mismatch.");
			}
			if((u64)batchI.Count != cnt){
				throw new InvalidOperationException("ParamAutoBinder.Many(...) length mismatch.");
			}
			batches.Add(batchI);
		}
		return true;
	}

	extension(ISqlCmdMkr z){
		[Doc(@$"
#Sum[Create auto batch helper with provider-based default size]
#Params([Db function context],[SQL duplicator],[Batch execution delegate],[Batch size, 0 means auto])
#TParams([Batch item type],[Batch return type])
#Rtn[{nameof(AutoBatch)} instance]
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
#See([{nameof(IAutoBindSqlDuplicator.ParamAutoBinders)}],[{nameof(IParamAutoBinderManyValuesBatch)}])
")]
		public async IAsyncEnumerable<
			IDictionary<str, obj?>
		> RunSql(//AsyE1d
			IDbFnCtx Ctx
			,IAutoBindSqlDuplicator Sql
			,[EnumeratorCancellation] CT Ct
		){
			// Keep same default policy as AutoBatch helper.
			var BatchSize = z.DbSrcType.Eq(EDbSrcType.Sqlite) ? 1ul : 500ul;
			var (manyBinders, oneBinders) = SplitBinders(Sql.ParamAutoBinders);

			ISqlCmd? fullBatchCmd = null;
			ISqlCmd? finalBatchCmd = null;
			try{
				// No Many binder: run once with fixed binders only.
				if(manyBinders.Count == 0){
					var cmd = await z.Prepare(Ctx, Sql.DuplicateSql(1), Ct);
					finalBatchCmd = cmd;
					var args = ArgDict.Mk();
					foreach(var binder in oneBinders){
						binder.Bind(args, new List<obj?>());
					}
					await foreach(var row in YieldRowsOrNull(cmd, args, Ct)){
						yield return row;
					}
					yield break;
				}
				//有ManyBinder
				while(true){
					// Take one logical batch from first sequence binder.
					if(!TryTakeAlignedBatches(manyBinders, BatchSize, out var firstBatch, out var batches)){
						break;
					}
					var cnt = (u64)firstBatch.Count;

					ISqlCmd cmd;
					if(cnt == BatchSize){
						// Reuse prepared full-batch command.
						fullBatchCmd ??= await GetCmdForBatch(z, Ctx, Sql, BatchSize, cnt, fullBatchCmd, Ct);
						cmd = fullBatchCmd;
					}else{
						// Tail batch needs different duplicated SQL; prepare a separate command.
						if(finalBatchCmd != null && !ReferenceEquals(finalBatchCmd, fullBatchCmd)){
							await finalBatchCmd.DisposeAsync();
						}
						finalBatchCmd = await GetCmdForBatch(z, Ctx, Sql, BatchSize, cnt, fullBatchCmd, Ct);
						cmd = finalBatchCmd;
					}

					var args = BuildArgsForBatch(oneBinders, manyBinders, firstBatch, batches);

					await foreach(var row in YieldRowsOrNull(cmd, args, Ct)){
						yield return row;
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

namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsSqlHelper;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

public static class ExtnISqlCmdMkr{
	private static (
		IList<IParamAutoBinderMulti> ManyBinders,
		IList<IParamAutoBinder> OneBinders
	) SplitBinders(IList<IParamAutoBinder> binders){
		var many = binders
			.Where(x=>x is IParamAutoBinderMulti)
			.Cast<IParamAutoBinderMulti>()
			.ToList();
		var one = binders.Where(x=>x is not IParamAutoBinderMulti).ToList();
		return (many, one);
	}

	private static async IAsyncEnumerable<IDictionary<str, obj?>> YieldRowsOrNull(
		ISqlCmd SqlCmd,
		IArgDict ArgDict,
		[EnumeratorCancellation] CT Ct
	){
		// Iterate per result-set to preserve one-slot output for empty result-sets.
		var d2 = SqlCmd.Args(ArgDict).AsyE2d(Ct);
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

	private static async Task<ISqlCmd> EnsureCmdForBatch(
		ISqlCmdMkr CmdMkr,
		IDbFnCtx Ctx,
		IAutoBindSqlDuplicator Sql,
		u64 BatchSize, // 批大小
		u64 Cnt, //入參數量 (或滿一批 亦或不滿批(末批))
		ISqlCmd? FullBatchCmd,
		CT Ct
	){
		if(Cnt == BatchSize){
			FullBatchCmd ??= await CmdMkr.Prepare(Ctx, Sql.DuplicateSql(BatchSize), Ct);
			return FullBatchCmd;
		}
		return await CmdMkr.Prepare(Ctx, Sql.DuplicateSql(Cnt), Ct);
	}

	private static IArgDict BuildArgsForBatch(
		IList<IParamAutoBinder> oneBinders,
		IList<IParamAutoBinderMulti> manyBinders,
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
		IList<IParamAutoBinderMulti> MultiBinders,
		u64 BatchSize,
		out IList FirstBatchArgs,
		out IList<IList> ArgBatchList
	){
		FirstBatchArgs = new List<obj?>();
		ArgBatchList = new List<IList>();
		if(!MultiBinders[0].TryTakeBatchArgs(BatchSize, out FirstBatchArgs)){
			return false;
		}
		var cnt = (u64)FirstBatchArgs.Count;
		ArgBatchList = new List<IList>{FirstBatchArgs};
		for(var i=1; i<MultiBinders.Count; i++){
			if(!MultiBinders[i].TryTakeBatchArgs(cnt, out var batchI)){
				throw new InvalidOperationException("ParamAutoBinder.Many(...) length mismatch.");
			}
			if((u64)batchI.Count != cnt){
				throw new InvalidOperationException("ParamAutoBinder.Many(...) length mismatch.");
			}
			ArgBatchList.Add(batchI);
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
#See([{nameof(IAutoBindSqlDuplicator.ParamAutoBinders)}],[{nameof(IParamAutoBinderMulti)}])
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
			var (multiBinders, oneBinders) = SplitBinders(Sql.ParamAutoBinders);

			ISqlCmd? fullBatchCmd = null;
			ISqlCmd? finalBatchCmd = null;
			try{
				// No Multi binder: run once with fixed binders only.
				if(multiBinders.Count == 0){
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
				//有 Multi Binder
				while(true){
					// Take one logical batch from first sequence binder.
					if(!TryTakeAlignedBatches(multiBinders, BatchSize, out var firstBatch, out var batches)){
						break;
					}
					var cnt = (u64)firstBatch.Count;

					ISqlCmd curCmd;
					if(cnt == BatchSize){
						// Reuse prepared full-batch command.
						fullBatchCmd ??= await EnsureCmdForBatch(z, Ctx, Sql, BatchSize, cnt, fullBatchCmd, Ct);
						curCmd = fullBatchCmd;
					}else{
						// Tail batch needs different duplicated SQL; prepare a separate command.
						if(finalBatchCmd != null && !ReferenceEquals(finalBatchCmd, fullBatchCmd)){
							await finalBatchCmd.DisposeAsync();
						}
						finalBatchCmd = await EnsureCmdForBatch(z, Ctx, Sql, BatchSize, cnt, fullBatchCmd, Ct);
						curCmd = finalBatchCmd;
					}

					var args = BuildArgsForBatch(oneBinders, multiBinders, firstBatch, batches);

					await foreach(var row in YieldRowsOrNull(curCmd, args, Ct)){
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

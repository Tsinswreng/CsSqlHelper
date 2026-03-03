using Tsinswreng.CsTools;

namespace Tsinswreng.CsSqlHelper;
public class AutoBatch<TItem, TRet> : BatchCollector<TItem, TRet> {
	public AutoBatch() {

	}
	/// TODO pg旹 500>100>50; sqlite 單條循環>50>100>500 按數據庫選批大小
	public static new u64 DfltBatchSize { get; set; } = 100;
	public I_DuplicateSql SqlDuplicator { get; set; }
	//public u64 BatchSize;
	public ISqlCmd FullBatch { get; set; } = null!;
	public ISqlCmd FinalBatch { get; set; } = null!;
	public IDbFnCtx Ctx { get; set; }
	public ISqlCmdMkr SqlCmdMkr { get; set; }
	public ISqlCmd SqlCmd { get; set; }
	public static AutoBatch<TItem, TRet> Mk(
		IDbFnCtx Ctx
		, ISqlCmdMkr SqlCmdMkr
		, I_DuplicateSql SqlDuplicator
		, Func<
			AutoBatch<TItem, TRet> //Self
			, IList<TItem>
			, CT
			, Task<TRet>
		> FnAsy
		, u64 BatchSize = 0
	) {
		if (BatchSize == 0) {
			BatchSize = DfltBatchSize;
		}
		var R = new AutoBatch<TItem, TRet>();
		R.Ctx = Ctx;
		R.SqlCmdMkr = SqlCmdMkr;
		R.SqlDuplicator = SqlDuplicator;
		var ArgFn = FnAsy;
		R.FnAsy = async (Items, Ct) => {
			var size = (u64)Items.Count;
			R.SqlCmd = R.FullBatch;
			var FnGetRepeatedSql = R.SqlDuplicator.DuplicateSql;
			if ((u64)Items.Count < R.BatchSize) {//不滿一批 即末批
				R.FinalBatch = await R.Ctx.PrepareToDispose(R.SqlCmdMkr, FnGetRepeatedSql(size), Ct);
				R.SqlCmd = R.FinalBatch;
			}
			else if (R.FullBatch == null) { //滿一批但FullBatch未初始化 即首批
				R.FullBatch = await R.Ctx.PrepareToDispose(R.SqlCmdMkr, FnGetRepeatedSql(BatchSize), Ct);
				R.SqlCmd = R.FullBatch;
			}
			return await ArgFn(R, Items, Ct);
		};
		R.Init(R.FnAsy, BatchSize);
		return R;
	}

}

namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsSqlHelper;

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
		> RunSql(//AsyE1d
			IDbFnCtx Ctx
			,IArgsSqlDuplicator Sql
			,CT Ct
		){
			
		}
		
	}
}

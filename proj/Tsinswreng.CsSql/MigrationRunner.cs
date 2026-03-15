namespace Tsinswreng.CsSql;

/// 通用遷移執行器：在單個事務中應用未執行的遷移。
public class MigrationRunner{
	public IMigrationMgr MigrationMgr;
	public ISqlCmdMkr SqlCmdMkr;
	public IMkrTxn MkrTxn;
	public IRepo<SchemaHistory, i64> RepoSchemaHistory;
	public TxnWrapper TxnWrapper;

	public MigrationRunner(
		IMigrationMgr MigrationMgr
		,ISqlCmdMkr SqlCmdMkr
		,IMkrTxn MkrTxn
		,IRepo<SchemaHistory, i64> RepoSchemaHistory
		,TxnWrapper TxnWrapper
	){
		this.RepoSchemaHistory = RepoSchemaHistory.UseSqlHelperDictMapper();
		this.MigrationMgr = MigrationMgr;
		this.SqlCmdMkr = SqlCmdMkr;
		this.MkrTxn = MkrTxn;
		this.TxnWrapper = TxnWrapper;
	}

	public async Task<nil> UpAsy(CT Ct){
		return await TxnWrapper.Wrap<nil>(
			async(Ctx, Ct)=>{
				return async(Ct)=>{
					await MigrationMgr.RunPendingMigrations(
						Ctx: Ctx
						,SqlCmdMkr: SqlCmdMkr
						,MkrTxn: MkrTxn
						,RepoHistory: RepoSchemaHistory
						,Ct: Ct
					);
					return NIL;
				};
			}
			,Ct
		);
	}
}

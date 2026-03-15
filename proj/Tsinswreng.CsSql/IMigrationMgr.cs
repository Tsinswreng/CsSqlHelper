namespace Tsinswreng.CsSql;
public interface IMigrationMgr{
	public IList<ISqlMigrationInfo> SqlMigrationInfos{get;set;}
	public IMigrationMgr AddMigration(ISqlMigrationInfo Info){
		this.SqlMigrationInfos.Add(Info);
		return this;
	}

	/// 對于升級場景：執行所有未執行的遷移並記錄到 SchemaHistory
	public Task<nil> RunPendingMigrations(
		IDbFnCtx Ctx
		,ISqlCmdMkr SqlCmdMkr
		,IMkrTxn MkrTxn
		,IRepo<SchemaHistory, i64> RepoHistory
		,CT Ct
	);

	/// 對于全新安裝場景：把所有遷移都標記爲已執行（不重複跑Sql）
	public Task<nil> MarkAllApplied(
		IDbFnCtx Ctx
		,IRepo<SchemaHistory, i64> RepoHistory
		,CT Ct
	);
}


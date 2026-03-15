namespace Tsinswreng.CsSql;

/// 遷移管理器。
///
/// 職責：
/// - 保存當前程序集/應用啓用的遷移清單
/// - 在升級場景下只執行「尚未執行」的遷移
/// - 在全新建庫場景下，把遷移全部標記爲已應用
///
/// 注意：
/// - 它只負責「遷移編排」，不負責決定某個業務程序集有哪些遷移
/// - Local/Biz 只需要各自把遷移註冊進來即可
public interface IMigrationMgr{
	/// 已註冊的遷移清單。
	/// 約定按 CreatedMs 由小到大追加，這樣可保證執行順序穩定。
	public IList<ISqlMigrationInfo> SqlMigrationInfos{get;set;}

	/// 向當前管理器追加一條遷移。
	/// 是否去重由上層輔助擴展決定。
	public IMigrationMgr AddMigration(ISqlMigrationInfo Info){
		this.SqlMigrationInfos.Add(Info);
		return this;
	}

	/// 對于升級場景：執行所有未執行的遷移並記錄到 SchemaHistory。
	///
	/// 典型場景：
	/// - 用戶已安裝舊版客戶端，升級到新版後首次啓動
	/// - 服務端已有舊庫，部署新版本時手動執行遷移
	public Task<nil> RunPendingMigrations(
		IDbFnCtx Ctx
		,ISqlCmdMkr SqlCmdMkr
		,IMkrTxn MkrTxn
		,IRepo<SchemaHistory, i64> RepoHistory
		,CT Ct
	);

	/// 對于全新安裝場景：把所有遷移都標記爲已執行（不重複跑 Sql）。
	///
	/// 典型場景：
	/// - 先直接用最新版映射建完整新庫
	/// - 再把遷移歷史表補齊，避免之後誤把舊遷移重新執行一遍
	public Task<nil> MarkAllApplied(
		IDbFnCtx Ctx
		,IRepo<SchemaHistory, i64> RepoHistory
		,CT Ct
	);
}


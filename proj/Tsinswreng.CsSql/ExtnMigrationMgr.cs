namespace Tsinswreng.CsSql;

/// 與業務無關的遷移管理輔助函數。
/// 用於按 CreatedMs 去重註冊遷移，避免 Local/Biz 重複寫相同邏輯。
public static class ExtnMigrationMgr{
	extension(IMigrationMgr z){
		/// 判斷某個 CreatedMs 的遷移是否已註冊。
		public bool HasMigrationCreatedMs(i64 CreatedMs){
			foreach(var Info in z.SqlMigrationInfos){
				if(Info.CreatedMs == CreatedMs){
					return true;
				}
			}
			return false;
		}

		/// 只在未註冊時添加單個遷移。
		public IMigrationMgr AddMigrationIfAbsent(ISqlMigrationInfo Info){
			if(!z.HasMigrationCreatedMs(Info.CreatedMs)){
				z.AddMigration(Info);
			}
			return z;
		}

		/// 批量註冊遷移（按 CreatedMs 去重）。
		public IMigrationMgr AddMigrationsIfAbsent(IEnumerable<ISqlMigrationInfo> Infos){
			foreach(var Info in Infos){
				z.AddMigrationIfAbsent(Info);
			}
			return z;
		}
	}
}

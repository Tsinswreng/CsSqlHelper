namespace Tsinswreng.CsSql;

public static class ExtnSchemaHistoryRepo{
	extension(IRepo<SchemaHistory, i64> z)
	{
		/// 將 SchemaHistory Repo 之 DictMapper 統一綁定到 SqlHelperDictMapper。
		public IRepo<SchemaHistory, i64> UseSqlHelperDictMapper(){
			if(z is not SqlRepo<SchemaHistory, i64> SqlRepoSchemaHistory){
				throw new ArgumentException("RepoSchemaHistory must be SqlRepo<SchemaHistory, i64>");
			}
			SqlRepoSchemaHistory.DictMapper = SqlHelperDictMapper.Inst;
			return z;
		}
	}
}

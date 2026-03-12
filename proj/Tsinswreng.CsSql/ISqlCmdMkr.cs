namespace Tsinswreng.CsSqlHelper;


public partial interface ISqlCmdMkr:I_DbSrcType{

	/// 璫允 先Prepare後傳參數。㕥便 複用SqlCmd、每次傳不同參數。
	public Task<ISqlCmd> Prepare(ISqlCmd Cmd, CT Ct);

	public Task<ISqlCmd> Prepare(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	);

	public Task<ISqlCmd> Prepare(
		IDbFnCtx? DbFnCtx
		,ISql Sql
		,CT Ct
	){
		return Prepare(DbFnCtx, Sql.RawStr, Ct);
	}

/// 無prepare、適用于CREATE TABLE等
	public Task<ISqlCmd> MkCmd(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	);

}



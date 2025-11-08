namespace Tsinswreng.CsSqlHelper;


public partial interface ISqlCmdMkr{

	/// <summary>
	/// 璫允 先Prepare後傳參數。㕥便 複用SqlCmd、每次傳不同參數。
	/// </summary>
	/// <param name="Cmd"></param>
	/// <param name="Ct"></param>
	/// <returns></returns>
	public Task<ISqlCmd> Prepare(ISqlCmd Cmd, CT Ct);

	public Task<ISqlCmd> Prepare(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	);

	public Task<ISqlCmd> Prepare(
		IBaseDbFnCtx? DbFnCtx
		,ISql Sql
		,CT Ct
	){
		return Prepare(DbFnCtx, Sql.RawStr, Ct);
	}

/// <summary>
/// 無prepare、適用于CREATE TABLE等
/// </summary>
/// <param name="DbFnCtx"></param>
/// <param name="Sql"></param>
/// <param name="Ct"></param>
/// <returns></returns>
	public Task<ISqlCmd> MkCmd(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		,CT Ct
	);

}



namespace Tsinswreng.CsSqlHelper;


public interface ISqlCmdMkr{

	public Task<ISqlCmd> Prepare(ISqlCmd Cmd, CT Ct);

	public Task<ISqlCmd> Prepare(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		,CT ct
	);

/// <summary>
/// 無prepare、適用于CREATE TABLE等
/// </summary>
/// <param name="DbFnCtx"></param>
/// <param name="Sql"></param>
/// <param name="ct"></param>
/// <returns></returns>
	public Task<ISqlCmd> MkCmd(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		,CT ct
	);

}

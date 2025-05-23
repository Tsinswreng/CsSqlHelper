namespace Tsinswreng.SqlHelper.Cmd;


public interface ISqlCmdMkr{

	public Task<ISqlCmd> PrepareAsy(
		IDbFnCtx? DbFnCtx
		,str Sql
		,CancellationToken ct
	);

}

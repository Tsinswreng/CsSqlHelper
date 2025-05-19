namespace Tsinswreng.SqlHelper.Cmd;


public interface I_SqlCmdMkr{

	public Task<I_SqlCmd> PrepareAsy(
		I_DbFnCtx? DbFnCtx
		,str Sql
		,CancellationToken ct
	);

}

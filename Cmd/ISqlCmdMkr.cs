namespace Tsinswreng.SqlHelper.Cmd;


public interface ISqlCmdMkr{

	public Task<I_SqlCmd> PrepareAsy(
		I_DbFnCtx? DbFnCtx
		,str Sql
		,CancellationToken ct
	);

}

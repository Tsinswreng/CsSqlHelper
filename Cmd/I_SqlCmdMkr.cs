namespace Tsinswreng.SqlHelper.Cmd;


public interface I_SqlCmdMkr{
	public Task<I_SqlCmd> PrepareAsy(
		str Sql
		,CancellationToken ct
	);

}

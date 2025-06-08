namespace Tsinswreng.CsSqlHelper.Cmd;


public interface ISqlCmdMkr{

	public Task<ISqlCmd> Prepare(
		IBaseDbFnCtx? DbFnCtx
		,str Sql
		,CT ct
	);

}

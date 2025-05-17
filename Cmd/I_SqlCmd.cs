namespace Tsinswreng.SqlHelper.Cmd;


public interface I_SqlCmd{
	public str Sql{get;set;}
	public Task<nil> PrepareAsy(CancellationToken ct);

}

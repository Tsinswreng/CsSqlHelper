namespace Tsinswreng.CsSqlHelper.Db;

public interface ITxn : IDisposable{
	public object? RawTxn{get;}
	public Task<nil> Begin(CT Ct);
	public Task<nil> Commit(CT Ct);
	public Task<nil> Rollback(CT Ct);
}

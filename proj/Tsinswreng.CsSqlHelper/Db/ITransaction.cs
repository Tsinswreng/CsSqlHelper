namespace Ngaq.Core.Infra.Db;

public interface ITxn : IDisposable{
	public object? RawTxn{get;}
	public Task<nil> Begin(CancellationToken Ct);
	public Task<nil> Commit(CancellationToken Ct);
	public Task<nil> Rollback(CancellationToken Ct);
}

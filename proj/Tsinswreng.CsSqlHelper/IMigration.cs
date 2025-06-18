namespace Tsinswreng.CsSqlHelper;

public interface IMigration{
	public i64 CreatedAt{get;set;}
	/// <summary>
	/// version after ran this migration
	/// </summary>
	//public str Version{get;set;}

	public Task<nil> UpAsy(CT Ct);
	public Task<nil> DownAsy(CT Ct);
}

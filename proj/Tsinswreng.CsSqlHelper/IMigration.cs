namespace Tsinswreng.CsSqlHelper;

public interface ISqlMigration{
	public IList<str> SqlsUp{get;}
	public IList<str> SqlsDown{get;}
}

public partial interface IMigration{
	public i64 CreatedMs{get;set;}
	/// <summary>
	/// version after ran this migration
	/// </summary>
	//public str Version{get;set;}

	public Task<nil> UpAsy(CT Ct);
	public Task<nil> DownAsy(CT Ct);
}

namespace Tsinswreng.CsSqlHelper;
public interface IMigrationMgr{
	public IList<IMigration> Migrations{get;set;}
}


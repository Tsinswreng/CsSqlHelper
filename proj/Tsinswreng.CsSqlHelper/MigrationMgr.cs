namespace Tsinswreng.CsSqlHelper;

public class MigrationMgr: IMigrationMgr{
	public IList<IMigration> Migrations{get;set;} = new List<IMigration>();
	//TODO
}

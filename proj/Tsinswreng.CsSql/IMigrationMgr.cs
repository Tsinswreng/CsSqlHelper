namespace Tsinswreng.CsSqlHelper;
public interface IMigrationMgr{
	public IList<IMigration> Migrations{get;set;}
	public IMigrationMgr AddMigration(IMigration Migration){
		this.Migrations.Add(Migration);
		return this;
	}
}


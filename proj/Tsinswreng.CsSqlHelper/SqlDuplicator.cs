namespace Tsinswreng.CsSqlHelper;

public interface I_DuplicateSql{
	public str DuplicateSql(u64 Cnt);
}

public class FnSqlDuplicator : I_DuplicateSql{
	public str DuplicateSql(u64 Cnt){
		return FnDuplicateSql(Cnt);
	}
	public Func<u64, str> FnDuplicateSql{get;set;}
	public FnSqlDuplicator(Func<u64, str> FnDuplicateSql){
		this.FnDuplicateSql = FnDuplicateSql;
	}
	public static I_DuplicateSql Mk(Func<u64, str> FnDuplicateSql){
		return new FnSqlDuplicator(FnDuplicateSql);
	}
}


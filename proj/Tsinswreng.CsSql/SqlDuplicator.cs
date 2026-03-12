namespace Tsinswreng.CsSql;

public interface ISqlDuplicator{
	public str DuplicateSql(u64 Cnt);
}

public interface IArgIndexer{
	public obj? GetArgAt(u64 Ofst);
}

public interface IArgsSqlDuplicator:ISqlDuplicator, IArgIndexer{
	
}


public class FnSqlDuplicator : ISqlDuplicator{
	public str DuplicateSql(u64 Cnt){
		return FnDuplicateSql(Cnt);
	}
	public Func<u64, str> FnDuplicateSql{get;set;}
	public FnSqlDuplicator(Func<u64, str> FnDuplicateSql){
		this.FnDuplicateSql = FnDuplicateSql;
	}
	public static ISqlDuplicator Mk(Func<u64, str> FnDuplicateSql){
		return new FnSqlDuplicator(FnDuplicateSql);
	}
}


namespace Tsinswreng.CsSql;

public interface IDbErr{

}

public class DbErr
	:Exception
	,IDbErr
{
	public DbErr(str Msg, Exception? Inner):base(Msg, Inner){}

}

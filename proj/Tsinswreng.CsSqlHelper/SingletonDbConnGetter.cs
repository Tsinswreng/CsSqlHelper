namespace Tsinswreng.CsSqlHelper;

using System.Data;

public class SingletonDbConnGetter : IDbConnMgr{
	public SingletonDbConnGetter(IDbConnection DbConn){
		this.DbConn = DbConn;
	}
	IDbConnection DbConn{get;set;}
	public async Task<IDbConnection> GetConnAsy(CT Ct){
		return DbConn;
	}
	public async Task<nil> AfterUsingConnAsy(IDbConnection Conn, CT Ct){
		return NIL;
	}
}

namespace Tsinswreng.CsSqlHelper;

using System.Data;

public class SingletonDbConnGetter : I_GetDbConnAsy{
	public SingletonDbConnGetter(IDbConnection DbConn){
		this.DbConn = DbConn;
	}
	IDbConnection DbConn{get;set;}
	public async Task<IDbConnection> GetConnAsy(CT Ct){
		return DbConn;
	}
}

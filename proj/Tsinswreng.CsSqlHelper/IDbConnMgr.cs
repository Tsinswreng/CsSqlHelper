using System.Data;

namespace Tsinswreng.CsSqlHelper;

public interface IDbConnMgr{
	public Task<IDbConnection> GetConnAsy(CT Ct);
	/// <summary>
	/// 若潙單例連接則不dipose、否則dispose
	/// 歸還連接勿複用業務ʹ取消標記
	/// </summary>
	/// <param name="Conn"></param>
	/// <returns></returns>
	public Task<nil> AfterUsingConnAsy(IDbConnection Conn, CT Ct);
}

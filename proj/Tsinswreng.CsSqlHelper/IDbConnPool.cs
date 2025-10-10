using System.Data;

namespace Tsinswreng.CsSqlHelper;

public interface IDbConnPool{
	public Task<IDbConnection> GetConnAsy(CT Ct);
}

using System.Data;

namespace Tsinswreng.CsSqlHelper;

public interface I_GetDbConnAsy{
	public Task<IDbConnection> GetConnAsy(CT Ct);
}

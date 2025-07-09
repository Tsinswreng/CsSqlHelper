namespace Tsinswreng.CsSqlHelper;

public interface I_GetTxnAsy{
	public Task<ITxn> GetTxnAsy(CT Ct);
	
}

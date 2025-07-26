namespace Tsinswreng.CsSqlHelper;

public  partial interface I_GetTxnAsy{
	public Task<ITxn> GetTxnAsy(CT Ct);

}

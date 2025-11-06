namespace Tsinswreng.CsSqlHelper;

public partial interface I_GetTxnAsy{
	// [Obsolete]
	// public Task<ITxn> GetTxnAsy(CT Ct);

	public Task<ITxn> GetTxnAsy(
		IBaseDbFnCtx Ctx, CT Ct
	);

}

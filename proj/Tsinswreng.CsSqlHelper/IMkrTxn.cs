namespace Tsinswreng.CsSqlHelper;

public partial interface IMkrTxn{
	// [Obsolete]
	// public Task<ITxn> GetTxnAsy(CT Ct);

	public Task<ITxn> MkTxnAsy(
		IDbFnCtx Ctx, CT Ct
	);

}

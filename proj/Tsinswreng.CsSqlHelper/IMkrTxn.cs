namespace Tsinswreng.CsSqlHelper;

public partial interface IMkrTxn{
	// [Obsolete]
	// public Task<ITxn> GetTxnAsy(CT Ct);

	public Task<ITxn> MkTxn(
		IDbFnCtx Ctx, CT Ct
	);

}

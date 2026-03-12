namespace Tsinswreng.CsSql;

public partial interface IMkrTxn{
	// [Obsolete]
	// public Task<ITxn> GetTxnAsy(CT Ct);

	[Doc(@$"make and bind transaction to `{nameof(Ctx)}`")]
	public Task<ITxn> MkEtBindTxn(
		IDbFnCtx Ctx, CT Ct
	);

}

using Microsoft.EntityFrameworkCore;

namespace Tsinswreng.CsSqlHelper.EFCore;

public  partial class EfTxnMkr(
	DbContext DbContext
)

	:I_GetTxnAsy
{
	public async Task<ITxn> GetTxnAsy(CT Ct){
		var Tx = await DbContext.Database.BeginTransactionAsync(Ct);
		var R = new EfTxn(Tx);
		return R;
	}
}

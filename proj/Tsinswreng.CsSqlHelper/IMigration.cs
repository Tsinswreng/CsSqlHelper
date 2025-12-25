using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper;

public interface I_CreatedMs{
	public i64 CreatedMs{get;set;}
}

public interface ISqlMigration:I_CreatedMs{
	public IList<str> SqlsUp{get;}
	public IList<str> SqlsDown{get;}
}

public partial interface IMigration:I_CreatedMs{
	public Task<nil> UpAsy(CT Ct);
	public Task<nil> DownAsy(CT Ct);
}

public class SqlMigration :IMigration, ISqlMigration{
	public i64 CreatedMs{get;set;}
	public IList<str> SqlsUp{get;} = [];
	public IList<str> SqlsDown{get;} = [];

	ITblMgr TblMgr;
	ISqlCmdMkr SqlCmdMkr;
	I_GetTxnAsy TxnGetter;
	public SqlMigration(
		ITblMgr TblMgr
		,ISqlCmdMkr SqlCmdMkr
		,I_GetTxnAsy TxnGetter
	){
		this.TblMgr = TblMgr;
		this.SqlCmdMkr = SqlCmdMkr;
	}

	async Task<nil> RunSql(IBaseDbFnCtx Ctx, str Sql, CT Ct){
		//IBaseDbFnCtx Ctx = new BaseDbFnCtx();
		var Cmd = await SqlCmdMkr.MkCmd(Ctx, Sql, Ct);
		await Cmd.All(Ct);
		return NIL;
	}

	public async Task<Func<
		CT, Task<nil>
	>> FnUpAsy(IBaseDbFnCtx Ctx, CT Ct){
		return async(Ct)=>{
			foreach(var sql in SqlsUp){
				await RunSql(Ctx, sql, Ct);
			}
			return NIL;
		};
	}

	[Impl(typeof(IMigration))]
	public async Task<nil> UpAsy(CT Ct){
		IBaseDbFnCtx Ctx = new BaseDbFnCtx();
		Ctx.Txn = await TxnGetter.GetTxnAsy(Ctx, Ct);
		try{
			var Fn = await FnUpAsy(Ctx, Ct);
			await Fn(Ct);
			await Ctx.DisposeAsync();
		}
		catch (System.Exception){

			throw;
		}
		return NIL;
	}

	[Impl(typeof(IMigration))]
	public async Task<nil> DownAsy(CT Ct){
		throw new NotImplementedException();
	}
}

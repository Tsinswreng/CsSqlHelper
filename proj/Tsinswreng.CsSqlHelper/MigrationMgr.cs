using Tsinswreng.CsCore;
using Tsinswreng.CsPage;

namespace Tsinswreng.CsSqlHelper;

public class MigrationMgr: IMigrationMgr{
	public IList<IMigration> Migrations{get;set;} = new List<IMigration>();
	ITblMgr TblMgr;
	ISqlCmdMkr SqlCmdMkr;
	public MigrationMgr(
		ITblMgr TblMgr
		,ISqlCmdMkr SqlCmdMkr
	){
		this.TblMgr = TblMgr;
		this.SqlCmdMkr = SqlCmdMkr;
	}

	public async Task<Func<
		CT
		,Task<SchemaHistory?>
	>> FnGetLastHistory(IDbFnCtx Ctx, CT Ct){
		var T = TblMgr.GetTbl<SchemaHistory>();
		var PCreatedMs = T.Prm(nameof(SchemaHistory.CreatedMs));
		var Sql =
$"""
SELECT * FROM {T.Qt(T.DbTblName)}
WHERE 1=1
ORDER BY {T.Fld(PCreatedMs)} DESC
{T.SqlMkr.ParamLimOfst(out var Lim, out var Ofst)}
""";
		var Cmd = await Ctx.PrepareToDispose(SqlCmdMkr, Sql, Ct);
		return async (Ct)=>{
			//相當於limit 1、但屏蔽sql方言ʹ異
			var PageQry = new PageQry{
				PageSize = 1,
				PageIdx = 0,
			};
			var R = await Cmd
			.Args(
				ArgDict.Mk(T).AddPageQry(PageQry, Lim, Ofst)
			)
			.FirstOrDefault<SchemaHistory>(T, Ct);
			return R;
		};
	}

	public async Task<SchemaHistory?> GetLastHistory(CT Ct){
		var Ctx = new DbFnCtx();
		var Fn = await FnGetLastHistory(Ctx, Ct);
		var R = await Fn(Ct);
		return R;
	}


	IList<IMigration> GetUndeployedMigrations(
		[See(nameof(IMigration.CreatedMs))]
		i64 LastCreatedMs
		,i64 BeforeMs
	){
		//this.Migrations//其中的CreatedMs已經是從小到大的了
		var undeployed = new List<IMigration>();
		foreach (var m in this.Migrations){
			// 列表已按 CreatedMs 从小到大排好，一旦超过上限就可以提前退出
			if (m.CreatedMs >= BeforeMs){
				break;
			}
			if (m.CreatedMs > LastCreatedMs){
				undeployed.Add(m);
			}
		}

		return undeployed;
	}

	// public async Task<nil> UpAsy(CT Ct){
	// 	IBaseDbFnCtx Ctx = new BaseDbFnCtx();
	// 	Ctx.Txn = await MkrTxn.GetTxnAsy(Ctx, Ct);
	// 	try{
	// 		var Fn = await FnUpAsy(Ctx, Ct);
	// 		await Fn(Ct);
	// 	}
	// 	catch (System.Exception){
	// 		await Ctx.Txn.Rollback(Ct);
	// 		throw;
	// 	}
	// 	await Ctx.DisposeAsync();
	// 	return NIL;
	// }

	// public async Task<nil> SetupOneMigration(
	// 	IBaseDbFnCtx Ctx, IMigration Migration, CT Ct
	// ){
	// 	await Migration.UpAsy(Ct);
	// }
}

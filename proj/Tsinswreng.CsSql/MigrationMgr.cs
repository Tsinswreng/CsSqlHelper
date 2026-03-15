using Tsinswreng.CsCore;
using Tsinswreng.CsPage;

namespace Tsinswreng.CsSql;

public class MigrationMgr: IMigrationMgr{
	public IList<ISqlMigrationInfo> SqlMigrationInfos{get;set;} = new List<ISqlMigrationInfo>();
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
ORDER BY {T.QtCol(PCreatedMs)} DESC
{T.SqlMkr.ParamLimOfst(out var Lim, out var Ofst)}
""";
		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
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

	IList<ISqlMigrationInfo> GetUndeployedInfos(
		[See(nameof(ISqlMigrationInfo.CreatedMs))]
		i64 LastCreatedMs
	){
		var Undeployed = new List<ISqlMigrationInfo>();
		foreach(var Info in SqlMigrationInfos){
			if(Info.CreatedMs > LastCreatedMs){
				Undeployed.Add(Info);
			}
		}
		return Undeployed;
	}

	[Impl(typeof(IMigrationMgr))]
	public async Task<nil> RunPendingMigrations(
		IDbFnCtx Ctx
		,ISqlCmdMkr SqlCmdMkr
		,IMkrTxn MkrTxn
		,IRepo<SchemaHistory, i64> RepoHistory
		,CT Ct
	){
		var LastHistory = await GetLastHistory(Ct);
		var LastCreatedMs = LastHistory?.CreatedMs ?? 0;
		var Undeployed = GetUndeployedInfos(LastCreatedMs);
		if(Undeployed.Count == 0){ return NIL; }
		var InsertHistory = await RepoHistory.FnInsertManyNoPrepare(Ctx, Ct);
		foreach(var Info in Undeployed){
			var Migration = SqlMigration.MkSqlMigration(
				SqlCmdMkr: SqlCmdMkr
				,MkrTxn: MkrTxn
				,SqlMigrationInfo: Info
			);
			var FnUp = await Migration.FnUpAsy(Ctx, Ct);
			await FnUp(Ct);
			var History = new SchemaHistory{
				Id = Info.CreatedMs
				,CreatedMs = Info.CreatedMs
				,Name = Info.GetType().Name
			};
			await InsertHistory([History], Ct);
		}
		return NIL;
	}

	[Impl(typeof(IMigrationMgr))]
	public async Task<nil> MarkAllApplied(
		IDbFnCtx Ctx
		,IRepo<SchemaHistory, i64> RepoHistory
		,CT Ct
	){
#pragma warning disable CS0618
		var InsertHistory = await RepoHistory.FnInsertManyNoPrepare(Ctx, Ct);
#pragma warning restore CS0618
		foreach(var Info in SqlMigrationInfos){
			var History = new SchemaHistory{
				Id = Info.CreatedMs
				,CreatedMs = Info.CreatedMs
				,Name = Info.GetType().Name
			};
			await InsertHistory([History], Ct);
		}
		return NIL;
	}
}


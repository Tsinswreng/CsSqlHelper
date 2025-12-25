namespace Tsinswreng.CsSqlHelper;

public interface ISqlMigration{
	public i64 CreatedMs{get;set;}
	public IList<str> SqlsUp{get;}
	public IList<str> SqlsDown{get;}
}

public partial interface IMigration{
	public i64 CreatedMs{get;set;}
	/// <summary>
	/// version after ran this migration
	/// </summary>
	//public str Version{get;set;}

	public Task<nil> UpAsy(CT Ct);
	public Task<nil> DownAsy(CT Ct);
}

public class SqlMigration{
	public i64 CreatedMs{get;set;}
	public IList<str> SqlsUp{get;} = [];
	public IList<str> SqlsDown{get;} = [];

	ITblMgr TblMgr;
	ISqlCmdMkr SqlCmdMkr;
	public SqlMigration(
		ITblMgr TblMgr
		,ISqlCmdMkr SqlCmdMkr
	){
		this.TblMgr = TblMgr;
		this.SqlCmdMkr = SqlCmdMkr;
	}

	async Task<Func<
		CT, Task<nil>
	>> FnRunSql(IBaseDbFnCtx Ctx, str Sql, CT Ct){
		//IBaseDbFnCtx Ctx = new BaseDbFnCtx();
		var Cmd = await SqlCmdMkr.MkCmd(Ctx, Sql, Ct);
		return async(Ct)=>{
			await Cmd.All(Ct);
			return NIL;
		};
	}

	public async Task<Func<
		CT, Task<nil>
	>> FnUpAsy(IBaseDbFnCtx Ctx, CT Ct){
		
		return async(Ct)=>{

			return NIL;
		};
	}
	public async Task<nil> DownAsy(CT Ct){
		return NIL;
	}
}

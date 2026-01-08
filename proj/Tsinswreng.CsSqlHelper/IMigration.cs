using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper;

public interface I_CreatedMs{
	public i64 CreatedMs{get;}
}


public partial interface IMigration:I_CreatedMs{
	public Task<Func<
		CT, Task<nil>
	>> FnUpAsy(IBaseDbFnCtx Ctx, CT Ct);

	public Task<Func<
		CT, Task<nil>
	>> FnDownAsy(IBaseDbFnCtx Ctx, CT Ct);
}

public interface ISqlMigrationInfo:I_CreatedMs{
	public IList<str> SqlsUp{get;}
	public IList<str> SqlsDown{get;}
}

public class SqlMigrationInfo:ISqlMigrationInfo{
	public virtual i64 CreatedMs{get;set;}
	public virtual IList<str> SqlsUp{get;set;} = [];
	public virtual IList<str> SqlsDown{get;set;} = [];
}



public class SqlMigration
	:ISqlMigrationInfo
	,IMigration
{

	//PassArgsByNameBecauseArgsCntMayChange
	public static SqlMigration MkSqlMigration(
		ISqlCmdMkr SqlCmdMkr
		,I_GetTxnAsy MkrTxn
		,ISqlMigrationInfo SqlMigrationInfo
	){
		var R = new SqlMigration(
			SqlCmdMkr: SqlCmdMkr
			,MkrTxn: MkrTxn
		);
		R.CreatedMs = SqlMigrationInfo.CreatedMs;
		R.SqlsUp = SqlMigrationInfo.SqlsUp;
		R.SqlsDown = SqlMigrationInfo.SqlsDown;
		return R;
	}

	public i64 CreatedMs{get;set;}
	public IList<str> SqlsUp{get;set;} = [];
	public IList<str> SqlsDown{get;set;} = [];

	ISqlCmdMkr SqlCmdMkr;
	I_GetTxnAsy MkrTxn;
	public SqlMigration(
		ISqlCmdMkr SqlCmdMkr
		,I_GetTxnAsy MkrTxn
	){
		this.SqlCmdMkr = SqlCmdMkr;
		this.MkrTxn = MkrTxn;
	}

	async Task<nil> RunSql(IBaseDbFnCtx Ctx, str Sql, CT Ct){
		//IBaseDbFnCtx Ctx = new BaseDbFnCtx();
		var Cmd = await SqlCmdMkr.MkCmd(Ctx, Sql, Ct);
		await Cmd.All(Ct);
		return NIL;
	}

	[Impl(typeof(IMigration))]
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
	public Task<Func<
		CT, Task<nil>
	>> FnDownAsy(IBaseDbFnCtx Ctx, CT Ct){
		throw new NotImplementedException();
	}
}

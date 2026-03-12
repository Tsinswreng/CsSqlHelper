#define Impl
namespace Tsinswreng.CsSql;
using System.Data;


[Doc(@$"Database Function Context")]
public partial class DbFnCtx:IDbFnCtx{
	[Doc(@$"Transaction")]
	public ITxn? Txn{get;set;}
	[Doc(@$"don't need to set {nameof(DbConn)} manually
	when you pass DbFnCtx to {nameof(ISqlCmdMkr.MkCmd)}
	if {nameof(DbConn)} is null, it will be initialized by {nameof(IDbConnMgr)}
	")]
	public IDbConnection? DbConn{get;set;}
	[Doc(@$"KV Properties")]
	public IDictionary<str, object?>? Props{get;set;}
	public ICollection<obj?>? ObjsToDispose{get;set;}
#if Impl
	 = new List<obj?>();
#endif
	[Obsolete]
	public u64 BatchSize{get;set;} = 1;

}

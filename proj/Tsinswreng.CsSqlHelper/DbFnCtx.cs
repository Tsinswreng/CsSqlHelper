#define Impl
namespace Tsinswreng.CsSqlHelper;
using System.Data;



public partial class DbFnCtx:IDbFnCtx{
	public ITxn? Txn{get;set;}
	public IDbConnection? DbConn{get;set;}
	public IDictionary<str, object?>? Props{get;set;}
	public ICollection<obj?>? ObjsToDispose{get;set;}
#if Impl
	 = new List<obj?>();
#endif

}

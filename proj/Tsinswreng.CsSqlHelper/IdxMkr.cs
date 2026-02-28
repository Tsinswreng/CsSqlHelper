using System.Linq.Expressions;

namespace Tsinswreng.CsSqlHelper;

public class IdxMkr<T>{
	public ITable<T> Tbl;
	
	public ITable<T> CfgIdx(
		IOptMkIdx? Opt,
		params IEnumerable<str>[] Cols
	){
		throw new NotImplementedException();
	}
	
	public ITable<T> CfgIdxExpr(
		IOptMkIdx? Opt,
		params Expression<Func<T,obj?>>[] Exprs
	){
		throw new NotImplementedException();
	}

}

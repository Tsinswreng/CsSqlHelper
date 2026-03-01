using System.Linq.Expressions;
using Tsinswreng.CsTools;

namespace Tsinswreng.CsSqlHelper;

public interface ITblSetter<T>{
	public ITable<T> Tbl{get;set;}
	public ITable<T> Idx(
		IOptMkIdx? Opt,
		params IEnumerable<str>[] Cols
	);
	
	public ITable<T> IdxExpr(
		IOptMkIdx? Opt,
		params Expression<Func<T,obj?>>[] Exprs
	);
	
}

public class TblSetter<T>:ITblSetter<T>{
	public TblSetter(ITable<T> Tbl){
		this.Tbl = Tbl;
	}
	public ITable<T> Tbl{get;set;}
	
	
}

public static class ExtnITblSetter {
	extension<TEntity>(ITblSetter<TEntity> z){
		public ColMkr<TEntity, obj?, obj?> Col(
			str NameInCode
		){
			var t = z.Tbl;
			var col = t.GetCol(NameInCode);
			var R = new ColMkr<TEntity, obj?, obj?>();
			R.TableT = t;
			R.Table = t;
			R.Column = col;
			return R;
		}

		public ColMkr<TEntity, obj?, obj?> Col(
			Expression<Func<TEntity, obj?>> ExprMemb
		){
			var memb = ToolExpr.GetMemberName(ExprMemb);
			return z.Col<TEntity>(memb);
		}

		// public ColMkr<TEntity, TRaw, TUpper> Col<TRaw, TUpper>(
		// 	str NameInCode
		// ){
		// 	var col = z.GetCol(NameInCode);
		// 	var R = new ColMkr<TEntity, TRaw, TUpper>();
		// 	R.TableT = z;
		// 	R.Column = col;
		// 	return R;
		// }

	}
}

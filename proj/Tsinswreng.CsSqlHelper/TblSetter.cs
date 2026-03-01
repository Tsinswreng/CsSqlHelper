using System.Linq.Expressions;
using Tsinswreng.CsTools;

namespace Tsinswreng.CsSqlHelper;

public interface ITblSetter<T>{
	public ITable<T> Tbl{get;set;}
	[Doc($"""
	define index for table using strings of field names in code.
	#Params([Options])
	#Examples([
		using E = MyEntity;
		fn(null, [nameof(E.Key)], [(nameof(E.Owner)), nameof(E.Head)]) ->
		
		means create two indexs, one is (E.Key), the other is (E.Owner, E.Head)

	])
	""")]
	public ITable<T> Idx(
		IOptMkIdx? Opt,
		params IEnumerable<str>[] Cols
	);
	
	[Doc($$"""
	define index for table using member expressions.
	#Examples([
	fn(null
		,x=>x.Key
		,x=>new{x.Owner, x.Head}
	)
	means create two indexs, one is (E.Key), the other is (E.Owner, E.Head)
	])
	""")]
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
	
	/// <summary>
	/// Define index using member expressions
	/// </summary>
	public ITable<T> IdxExpr(
		IOptMkIdx? Opt,
		params Expression<Func<T, obj?>>[] Exprs
	) {
		var colSets = new List<IEnumerable<str>>();
		foreach (var expr in Exprs) {
			var colNames = ToolExpr.GetMemberNames(expr);
			colSets.Add(colNames);
		}
		return Idx(Opt, colSets.ToArray());
	}

	/// <summary>
	/// Define index using code column names
	/// </summary>
	public ITable<T> Idx(
		IOptMkIdx? Opt,
		params IEnumerable<str>[] Cols
	) {
		var tbl = Tbl;
		
		foreach (var colSet in Cols) {
			var colList = colSet.ToList();
			var isUnique = Opt?.Unique ?? false;
			var whereCondition = str.IsNullOrEmpty(Opt?.Where) ? null : new[] { Opt.Where };
			
			// Generate index name: Idx_{TableName}_{Col1}_{Col2}... or Ux_{TableName}_{Col1}_{Col2}...
			var prefix = isUnique ? "Ux" : "Idx";
			var colNamePart = string.Join("_", colList);
			var indexName = $"{prefix}_{tbl.DbTblName}_{colNamePart}";
			
			tbl.AddIndexByCodeCols(indexName, colList, isUnique, whereCondition);
		}
		
		return tbl;
	}
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

namespace Tsinswreng.CsSqlHelper;

using System.Linq.Expressions;
using Tsinswreng.CsCore;
using Tsinswreng.CsDictMapper;
using Tsinswreng.CsPage;
using Tsinswreng.CsTools;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;
using Str_Any = System.Collections.Generic.Dictionary<str, obj?>;
public static partial class ExtnITableT{
	extension<T>(ITable<T> z){
		//"db_col_name"  不帶表名前綴
		public str DbCol(Expression<Func<T, obj?>> ExprMemb){
			var t = (ITable)z;
			var memb = ToolExpr.GetMemberName(ExprMemb);
			return t.DbCol(memb);
		}

		public IField QtCol(Expression<Func<T, obj?>> ExprMemb){
			var t = (ITable)z;
			return t.QtCol<T>(ExprMemb);
		}

		public str Memb(Expression<Func<T, obj?>> ExprMemb){
			var t = (ITable)z;
			return t.Memb(ExprMemb);
		}

		public ISqlSplicer<T> SqlSplicer(){
			var t = (ITable)z;
			return t.SqlSplicer<T>();
		}

		public TPo DbDictToEntity<TPo>(
			IStr_Any DbDict
		)where TPo:new(){
			var t = (ITable)z;
			return t.DbDictToEntity<TPo>(DbDict);
		}

	}
}

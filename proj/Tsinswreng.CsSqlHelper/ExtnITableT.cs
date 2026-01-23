namespace Tsinswreng.CsSqlHelper;

using System.Linq.Expressions;
using Tsinswreng.CsCore;
using Tsinswreng.CsDictMapper;
using Tsinswreng.CsPage;
using Tsinswreng.CsTools;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;
using Str_Any = System.Collections.Generic.Dictionary<str, obj?>;
public static class ExtnITableT{
	extension<T>(ITable<T> z){
		//"db_name"  不帶表名前綴
		public IField Fld(Expression<Func<T, obj?>> ExprMemb){
			var t = (ITable)z;
			return t.Fld<T>(ExprMemb);
		}
		public ISqlSplicer<T> SqlSplicer(){
			var t = (ITable)z;
			return t.SqlSplicer<T>();
		}

	}
}

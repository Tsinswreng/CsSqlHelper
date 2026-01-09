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
		public IField Fld(Expression<Func<T, obj?>> GetMember){
			var memberName = ToolExpr.GetMemberName<T>(GetMember);
			var R = new Field(z, memberName);
			return R;
		}
		public ISqlSplicer<T> SqlSplicer(){
			var R = new SqlSplicer<T>();
			R.Tbl = z;
			return R;
		}

	}
}

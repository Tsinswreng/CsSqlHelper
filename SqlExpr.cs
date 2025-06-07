using System.Linq.Expressions;


namespace Tsinswreng.CsSqlHelper;

public class SqlExpr{
	public SqlExpr Select<T>(
		Expression<Func<T, object>> expr
	){
		return this;
	}

	public SqlExpr Where<T>(Expression<Func<T, bool>> expr){
		return this;
	}

	public SqlExpr Page(u64 PageNum, u64 PageSize){
		return this;
	}

	public str ToSql(){
		return "";
	}
}


// class Test_{
// 	void Test(){
// 		SqlExpr E = new SqlExpr();
// 		// E
// 		// 	.Select<Po_Word>(w=>new {w.Id})
// 		// 	.Where
// 		// 	.Page(2,16)
// 		// ;
// 		List<Po_Word> Words = [];
// 		var Qry = from p in Words
// 		select p.Id;
// 	}
// }

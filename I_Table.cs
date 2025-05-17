using System.Collections;

namespace Tsinswreng.SqlHelper;

public interface I_Table{
	public Type EntityType{get;set;}
	public str Name{get;set;}
	#if Impl
	= "";
	#endif
	public IDictionary<str, I_Column> Columns{get;set;}
	#if Impl
	= new Dictionary<str, I_Column>();
	#endif

	public IDictionary<str, str> DbColName__CodeColName{get;set;}
	#if Impl
	= new Dictionary<str, object>();
	#endif

	public IDictionary<str, object> ExampleDict{get;set;}
	#if Impl
	= new Dictionary<str, object>();
	#endif

	public IDictionary<str, object> ToCodeDict(IDictionary<str, object> DbDict){
		var ans = new Dictionary<str, object>();
		foreach(var (kDb, vDb) in DbDict){
			var kCode = DbColName__CodeColName[kDb];
			var colCode = Columns[kCode];
			var vCode = colCode.ToCodeType(vDb);
			ans[kCode] = vCode;
		}
		return ans;
	}

}

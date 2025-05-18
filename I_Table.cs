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
	public str CodeIdName{get;set;}
	#if Impl
	= "Id";
	#endif

	public IDictionary<str, str> DbColName__CodeColName{get;set;}
	#if Impl
	= new Dictionary<str, str>();
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

	public IDictionary<str, object> ToDbDict(IDictionary<str, object> CodeDict){
		var ans = new Dictionary<str, object>();
		foreach(var (kCode, vCode) in CodeDict){
			var Col = Columns[kCode];
			var vDb = Col.ToDbType(vCode);
			ans[Col.NameInDb] = vDb;
		}
		return ans;
	}

}

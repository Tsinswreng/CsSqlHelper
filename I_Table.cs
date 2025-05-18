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

	public I_SqlMkr SqlMkr{get;set;}

}

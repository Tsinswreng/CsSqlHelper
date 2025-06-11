using System.Collections;
using Tsinswreng.CsSrcGen.Dict;

namespace Tsinswreng.CsSqlHelper;

using IStr_Any = System.Collections.Generic.IDictionary<string, object?>;
using Str_Any = System.Collections.Generic.Dictionary<string, object?>;

public interface ITable{
	public IDictMapper DictMapper{get;set;}
	public Type EntityType{get;set;}
	public str Name{get;set;}
	#if Impl
	= "";
	#endif
	public IDictionary<str, IColumn> Columns{get;set;}
	#if Impl
	= new Dictionary<str, I_Column>();
	#endif
	public str CodeColId{get;set;}
	#if Impl
	= "Id";
	#endif

	public ISoftDeleteCol? SoftDeleteCol{get;set;}

	public IDictionary<str, str> DbColName_CodeColName{get;set;}
	#if Impl
	= new Dictionary<str, str>();
	#endif

	public IStr_Any ExampleDict{get;set;}
	#if Impl
	= new Dictionary<str, object>();
	#endif

	public ISqlMkr SqlMkr{get;set;}

}

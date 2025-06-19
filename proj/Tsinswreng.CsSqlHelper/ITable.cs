using System.Collections;
using Tsinswreng.CsDictMapper.DictMapper;

namespace Tsinswreng.CsSqlHelper;

using IStr_Any = System.Collections.Generic.IDictionary<string, object?>;
using Str_Any = System.Collections.Generic.Dictionary<string, object?>;

public interface ITable{
	public IDictMapperShallow DictMapper{get;set;}
	public Type EntityType{get;set;}
/// <summary>
/// 表名
/// </summary>
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

	public IDictionary<str, Type> Key_Type{get;set;}
#if Impl
	= new Dictionary<str, Type>();
#endif
	public ISqlMkr SqlMkr{get;set;}
	/// <summary>
	/// 在CREATE TABLE() 塊內
	/// </summary>
	public IList<str> InnerAdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
	/// <summary>
	/// 在CREATE TABLE() 塊外
	/// </summary>
	public IList<str> OuterAdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
}

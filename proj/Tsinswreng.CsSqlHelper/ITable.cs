using System.Collections;
using Tsinswreng.CsDictMapper;

namespace Tsinswreng.CsSqlHelper;

using IStr_Any = System.Collections.Generic.IDictionary<string, object?>;
using Str_Any = System.Collections.Generic.Dictionary<string, object?>;

public partial interface ITable<T>:ITable{

}

public enum ERelationType{
	Unknown,
	OneToOne,
	OneToMany,
	ManyToMany,
	ManyToOne,
}

public class JoinCond{
	public IColumn Left{get;set;}
	public IColumn Right{get;set;}
}

public class Relation{
	public ERelationType Type{get;set;}
	public ITable TargetTbl{get;set;}
	//public
}

public class Relations{

}

/// <summary>
/// Table in database
/// </summary>
public partial interface ITable{
	public ITblMgr? TblMgr{get;set;}
	/// <summary>
	/// mapper to convert between object and dictionary
	/// </summary>
	public IDictMapperShallow DictMapper{get;set;}
	public Type CodeEntityType{get;set;}
	/// <summary>
	/// table name in database
	/// </summary>
	public str DbTblName{get;set;}
#if Impl
	= "";
#endif
/// <summary>
/// Key: property name in entity class
/// value: IColumn object
/// </summary>
	public IDictionary<str, IColumn> Columns{get;set;}
#if Impl
	= new Dictionary<str, I_Column>();
#endif
	/// <summary>
	/// 編程代碼中實體主鍵字段名
	/// </summary>
	public str CodeIdName{get;set;}
#if Impl
	= "Id";
#endif

/// <summary>
/// 軟刪除欄位
/// </summary>
	public ISoftDeleteCol? SoftDelCol{get;set;}
/// <summary>
/// 資料庫欄位名稱_代碼欄位名稱
/// </summary>
	public IDictionary<str, str> DbColName_CodeColName{get;set;}
#if Impl
	= new Dictionary<str, str>();
#endif

	public IDictionary<str, Type> CodeCol_UpperType{get;set;}
#if Impl
	= new Dictionary<str, Type>();
#endif
	public ISqlMkr SqlMkr{get;set;}
	/// <summary>
	/// 在CREATE TABLE() 塊內 如 `DEFAULT 0`
	/// </summary>
	public IList<str> InnerAdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
	/// <summary>
	/// 在CREATE TABLE() 塊外 如 `CREATE INDEX ...`
	/// </summary>
	public IList<str> OuterAdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
	public IDictionary<Type, IUpperTypeMapFn> UpperType_DfltMapper{get;set;}
#if Impl
	= new Dictionary<Type, IUpperTypeMapperFn>();
#endif
}

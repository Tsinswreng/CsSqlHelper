#define Impl
namespace Tsinswreng.CsSqlHelper;

using System.Linq.Expressions;
using Tsinswreng.CsCore;
using Tsinswreng.CsDictMapper;
using Tsinswreng.CsPage;
using Tsinswreng.CsTools;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;
using Str_Any = System.Collections.Generic.Dictionary<str, obj?>;


public partial class Table<T>: Table, ITable<T>{

}

public partial class Table:ITable{
	public ITblMgr? TblMgr{get;set;}
	public IDictMapperShallow DictMapper{get;set;}
	public Type CodeEntityType{get;set;}
	#pragma warning disable CS8618
	public Table(){}
	public Table(
		IDictMapperShallow DictMapper
		,str Name
		,IDictionary<str, Type> CodeCol_UpperType
	){
		this.DictMapper = DictMapper;
		this.DbTblName = Name;
		this.CodeCol_UpperType = CodeCol_UpperType;
	}

	bool _Inited = false;
	public ITable Init(){
		if(_Inited){
			return this;
		}
		foreach(var (Key,Type) in CodeCol_UpperType){
			var Col = new Column();
			Col.DbName = Key;
			Columns[Key] = Col;
			DbColName_CodeColName[Key] = Key;
			Col.RawCodeType = Type;
			Col.UpperCodeType = Type;
			// if(v != null){
			// 	Col.RawClrType = v.GetType();
			// 	Col.UpperClrType = v.GetType();
			// }

		}
		_Inited = true;
		return this;
	}

/// <summary>
/// 建構一個Table實體
/// </summary>
/// <param name="DictMapper"></param>
/// <param name="DbTblName">table name in db</param>
/// <param name="Key_Type"></param>
/// <returns></returns>
	public static ITable<TEntity> Mk<TEntity>(
		IDictMapperShallow DictMapper
		,str DbTblName
		,IDictionary<str, Type> Key_Type
	){
		return Mk<TEntity>(typeof(TEntity), DictMapper, DbTblName, Key_Type);
	}

	public static ITable<TEntity> Mk<TEntity>(
		Type EntityClrType
		,IDictMapperShallow DictMapper
		,str DbTblName
		,IDictionary<str, Type> Key_Type
	){
		var t = new Table<TEntity>{
			DictMapper = DictMapper
			,DbTblName = DbTblName
			,CodeCol_UpperType = Key_Type
			,CodeEntityType = EntityClrType
		};
		t.Init();
		return t;
	}

	public static Func<str, ITable<T>> FnMkTbl<T>(IDictMapperShallow DictMapper){
 		ITable<T2> Mk<T2>(str DbTblName){
			var TypeDict = DictMapper.GetTypeDictShallowT<T2>();
			return Table.Mk<T2>(
				DictMapper
				,DbTblName
				,TypeDict
			);
		}
		return Mk<T>;
	}

	[Impl]
	public str DbTblName{get;set;}
	#if Impl
	= "";
	#endif

	[Impl]
	public IDictionary<str, IColumn> Columns{get;set;}
	#if Impl
	= new Dictionary<str, IColumn>();
	#endif

	[Impl]
	public str CodeIdName{get;set;}
	#if Impl
	= "Id";
	#endif

	[Impl]
	public ISoftDeleteCol? SoftDelCol{get;set;}

	[Impl]
	public IDictionary<str, str> DbColName_CodeColName{get;set;}
	#if Impl
	= new Dictionary<str, str>();
	#endif

	[Impl]
	public IDictionary<str, Type> CodeCol_UpperType{get;set;}
	#if Impl
	= new Dictionary<str, Type>();
	#endif

	[Impl]
	public ISqlMkr SqlMkr{get;set;}
	/// <summary>
	/// 在CREATE TABLE() 塊內
	/// </summary>
	[Impl]
	public IList<str> InnerAdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
	/// <summary>
	/// 在CREATE TABLE() 塊外
	/// </summary>
	[Impl]
	public IList<str> OuterAdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
	public IDictionary<Type, IUpperTypeMapFn> UpperType_DfltMapper{get;set;}
#if Impl
	= new Dictionary<Type, IUpperTypeMapFn>();
#endif
}


#pragma warning disable CS8601

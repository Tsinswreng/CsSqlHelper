#define Impl
namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsCore;
using Tsinswreng.CsDictMapper;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;
using Str_Any = System.Collections.Generic.Dictionary<str, obj?>;


public partial class Table:ITable{
	public IDictMapperShallow DictMapper{get;set;}
	public Type CodeEntityType{get;set;}
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
	public static ITable Mk<TEntity>(
		IDictMapperShallow DictMapper
		,str DbTblName
		,IDictionary<str, Type> Key_Type
	){
		return Mk(typeof(TEntity), DictMapper, DbTblName, Key_Type);
	}

	public static ITable Mk(
		Type EntityClrType
		,IDictMapperShallow DictMapper
		,str DbTblName
		,IDictionary<str, Type> Key_Type
	){
		ITable ans = new Table{
			DictMapper = DictMapper
			,DbTblName = DbTblName
			,CodeCol_UpperType = Key_Type
			,CodeEntityType = EntityClrType
		}.Init();
		return ans;
	}

	public static Func<str, ITable> FnMkTbl<T>(IDictMapperShallow DictMapper){
 		ITable Mk<T2>(str DbTblName){
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
public static class ExtnITable{

/// <summary>
/// quote field name for SQL, e.g. in sqlite: "field_name"; in mysql: `field_name`;
/// </summary>
/// <param name="z"></param>
/// <param name="s"></param>
/// <returns></returns>
	public static str Qt(
		this ITable z
		,str s
	){
		return z.SqlMkr.Qt(s);
	}

/// <summary>
/// 映射到數據庫表ʹ字段名
/// </summary>
	public static str ToDbName(
		this ITable z
		,str CodeColName
	){
		var DbColName = z.Columns[CodeColName].DbName;
		return DbColName;
	}

/// <summary>
/// 映射到數據庫表ʹ字段名 並加引號/括號
/// </summary>
	public static str Fld(
		this ITable z
		,str CodeColName
	){
		var DbColName = z.Columns[CodeColName].DbName;
		return z.SqlMkr.Qt(DbColName);
	}

	public static IParam Prm(
		this ITable z
		,str Name
	){
		return z.SqlMkr.Prm(Name);
	}

	// [Obsolete]
	// public static str PrmStr(
	// 	this ITable z
	// 	,str Name
	// ){
	// 	return z.SqlMkr.PrmStr(Name);
	// }

	public static IStr_Any ToCodeDict(
		this ITable z
		,IStr_Any DbDict
	){
		var ans = new Str_Any();
		foreach(var (kDb, vDb) in DbDict){
			var kCode = z.DbColName_CodeColName[kDb];
			var colCode = z.Columns[kCode];
			var vCode = colCode.RawToUpper?.Invoke(vDb)??vDb;
			ans[kCode] = vCode;
		}
		return ans;
	}

	public static T AssignCodePo<T>(
		this ITable z
		,IStr_Any DbDict
		,T ToBeAssigned
	){
		var CodeDict = z.ToCodeDict(DbDict);
		z.DictMapper.AssignShallowT(ToBeAssigned, CodeDict);
		return ToBeAssigned;
	}

	public static TPo DbDictToPo<TPo>(
		this ITable z
		,IStr_Any DbDict
	)where TPo:new(){
		var CodeDict = z.ToCodeDict(DbDict);
		var ans = new TPo();
		z.DictMapper.AssignShallowT(ans, CodeDict);
		return ans;
	}

	//static i32 i = 0;

	public static IStr_Any ToDbDict(
		this ITable z
		,IStr_Any CodeDict
	){
		var ans = new Str_Any();
		foreach(var (kCode, vCode) in CodeDict){
			var Col = z.Columns[kCode];
			var vDb = Col.UpperToRaw?.Invoke(vCode)??vCode;
			ans[Col.DbName] = vDb;
		}
		return ans;

	}

	public static obj? UpperToRaw(
		this ITable z
		,obj? UpperValue
		,str CodeColName
	){
		return z.Columns[CodeColName].UpperToRaw?.Invoke(UpperValue)??UpperValue;
	}

	public static obj? UpperToRaw<T>(
		this ITable z
		,T UpperValue
		,str? CodeColName = null
	){
		if(CodeColName != null
			&& (z.Columns.TryGetValue(CodeColName, out var Col))
		){
			return Col.UpperToRaw?.Invoke(UpperValue)??UpperValue;
		}
		var Mapper = z.UpperType_DfltMapper[typeof(T)]
			?? throw new Exception("No UpperTypeMapperFn for type: "+ typeof(T))
		;
		return Mapper.UpperToRaw?.Invoke(UpperValue)??UpperValue;
	}

	public static obj? RawToUpper(
		this ITable z
		,obj? RawValue
		,str CodeColName
	){
		return z.Columns[CodeColName].RawToUpper?.Invoke(RawValue)??RawValue;
	}

	public static T RawToUpper<T>(
		this ITable z
		,obj? RawValue
		,str CodeColName
	){
		return (T)(z.Columns[CodeColName].RawToUpper?.Invoke(RawValue)??RawValue)!;
	}

	public static str UpdateClause(
		this ITable z
		,IEnumerable<str> RawFields
	){
		List<str> segs = [];
		foreach(var rawField in RawFields){
			var field = z.Fld(rawField);
			var param = z.Prm(rawField);
			segs.Add(field + " = " + param);
		}
		return string.Join(", ", segs);
	}

	public static str InsertClause(
		this ITable z
		,IEnumerable<str> RawFields
	){
		List<str> Fields = [];
		List<str> Params = [];
		foreach(var rawField in RawFields){
			var field = z.Fld(rawField);
			var param = z.Prm(rawField).ToString()??"";
			Fields.Add(field);
			Params.Add(param);
		}
		return "(" + string.Join(", ", Fields) + ") VALUES (" + string.Join(", ", Params) + ")";
	}


/// <summary>
/// (@0, @1, @2 ...)
/// </summary>
/// <param name="z"></param>
/// <param name="Count"></param>
/// <returns></returns>

	public static str NumParamClause(
		this ITable z
		,u64 EndPos
		,u64 StartPos = 0
	){
		List<str> R = [];
		R.Add("(");
		for(u64 i = StartPos; i <= EndPos; i++){
			var Param = z.Prm(i+"").ToString()??"";
			R.Add(Param);
			if(i == EndPos){
				R.Add(", ");
			}
		}
		R.Add(")");
		return string.Join("", R);
	}


/// <summary>
/// [@0, @1, @2 ...]
/// ,</summary>
/// <param name="z"></param>
/// <param name="Start">含</param>
/// <param name="End">含</param>
/// <returns></returns>
	public static IList<IParam> Prm(
		this ITable z
		,u64 Start
		,u64 End
	){
		var R = new List<IParam>();
		for(u64 i = Start; i <= End; i++){
			var Param = z.Prm(i+"");
			R.Add(Param);
		}
		return R;
	}

/// <summary>
/// [@0, @1, @2 ...]
/// ,</summary>
/// <param name="z"></param>
/// <param name="Start">含</param>
/// <param name="End">含</param>
/// <returns></returns>
	// [Obsolete]
	// public static IList<str> PrmStrArr(
	// 	this ITable z
	// 	,u64 Start
	// 	,u64 End
	// ){
	// 	var R = new List<str>();
	// 	for(u64 i = Start; i <= End; i++){
	// 		var Param = z.PrmStr(i+"");
	// 		R.Add(Param);
	// 	}
	// 	return R;
	// }

	public static str SqlMkTbl(
		this ITable z
	){
		str OneCol(ITable Tbl, IColumn Col){
			var R = new List<str>();
			R.Add(Tbl.Qt(Col.DbName));
			if(!str.IsNullOrEmpty(Col.DbType)){
				R.Add(Col.DbType);
			}else{
				if(Col.RawCodeType == null){
					var Msg = $"{Col.DbName}:\nCol.RawClrType == null";
					throw new Exception("Col.RawClrType == null");
				}
				try{
					var DbTypeName = z.SqlMkr.SqlTypeMapper.ToDbTypeName(Col.RawCodeType);
					R.Add(DbTypeName);
				}
				catch (System.Exception e){
					throw new Exception("Type Mapping Error for Colunm:"+ Col.DbName, e);
				}
			}
			R.AddRange(Col.AdditionalSqls??[]);
			if(Col.NotNull){
				R.Add("NOT NULL");
			}
			return string.Join(" ", R);
		}

		str FmtInnerSqls(IList<str>? Sqls){
			if(Sqls == null || Sqls.Count == 0){
				return "";
			}
			return $",\n\n{str.Join(",\n\t", z.InnerAdditionalSqls??[])}";
		}

		str FmtOuterSqls(IList<str>? Sqls){
			if(Sqls == null || Sqls.Count == 0){
				return "";
			}
			List<str> R = [];
			foreach(var sql in Sqls){
				R.Add(sql);
				R.Add(";\n");
			}
			return str.Join("",R);
		}

		var Lines = new List<str>();
		foreach(var (name, Col) in z.Columns){
			Lines.Add(OneCol(z, Col));
		}
var S =
$"""
CREATE TABLE IF NOT EXISTS {z.Qt(z.DbTblName)}(
	{string.Join(",\n\t", Lines)}{FmtInnerSqls(z.InnerAdditionalSqls)}
);
{FmtOuterSqls(z.OuterAdditionalSqls)}
""";
		return S;
	}


}

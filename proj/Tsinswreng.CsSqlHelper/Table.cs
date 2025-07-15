#define Impl
using System.Net;
using Tsinswreng.CsDictMapper;
using IStr_Any = System.Collections.Generic.IDictionary<string, object?>;
using Str_Any = System.Collections.Generic.Dictionary<string, object?>;
namespace Tsinswreng.CsSqlHelper;



public class Table:ITable{
	public IDictMapperShallow DictMapper{get;set;}
	public Type EntityClrType{get;set;}
	public Table(){}
	public Table(
		IDictMapperShallow DictMapper
		,str Name
		,IDictionary<str, Type> ExampleDict
	){
		this.DictMapper = DictMapper;
		this.DbTblName = Name;
		this.Key_Type = ExampleDict;
	}

	bool _Inited = false;
	public ITable Init(){
		if(_Inited){
			return this;
		}
		foreach(var (Key,Type) in Key_Type){
			var Col = new Column();
			Col.NameInDb = Key;
			Columns[Key] = Col;
			DbColName_CodeColName[Key] = Key;
			Col.RawClrType = Type;
			Col.UpperClrType = Type;
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
			,Key_Type = Key_Type
			,EntityClrType = EntityClrType
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

	public str DbTblName{get;set;}
	#if Impl
	= "";
	#endif
	public IDictionary<str, IColumn> Columns{get;set;}
	#if Impl
	= new Dictionary<str, IColumn>();
	#endif

	public str CodeIdName{get;set;}
	#if Impl
	= "Id";
	#endif

	public ISoftDeleteCol? SoftDelCol{get;set;}
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


#pragma warning disable CS8601
public static class ExtnITable{

/// <summary>
/// quote field name for SQL, e.g. in sqlite: "field_name"; in mysql: `field_name`;
/// </summary>
/// <param name="z"></param>
/// <param name="s"></param>
/// <returns></returns>
	public static str Quote(
		this ITable z
		,str s
	){
		return z.SqlMkr.Quote(s);
	}

/// <summary>
/// 映射到數據庫表ʹ字段名
/// </summary>
	public static str ToDbName(
		this ITable z
		,str CodeColName
	){
		var DbColName = z.Columns[CodeColName].NameInDb;
		return DbColName;
	}

/// <summary>
/// 映射到數據庫表ʹ字段名 並加引號/括號
/// </summary>
	public static str Field(
		this ITable z
		,str CodeColName
	){
		var DbColName = z.Columns[CodeColName].NameInDb;
		return z.SqlMkr.Quote(DbColName);
	}

/// <summary>
///
/// </summary>
	public static str Param(
		this ITable z
		,str CodeColName
	){
		return z.SqlMkr.Param(CodeColName);
	}


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
			var vDb = Col.UpperToRaw?.Invoke(vCode);//-
			ans[Col.NameInDb] = vDb;
		}
		return ans;

	}

	public static object? ToDbType(
		this ITable z
		,str CodeColName
		,object? CodeValue
	){
		return z.Columns[CodeColName].UpperToRaw?.Invoke(CodeValue);
	}

	public static str UpdateClause(
		this ITable z
		,IEnumerable<str> RawFields
	){
		List<str> segs = [];
		foreach(var rawField in RawFields){
			var field = z.Field(rawField);
			var param = z.Param(rawField);
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
			var field = z.Field(rawField);
			var param = z.Param(rawField);
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
[Obsolete]
	public static str NumParamClause(
		this ITable z
		,u64 EndPos
		,u64 StartPos = 0
	){
		List<str> R = [];
		R.Add("(");
		for(u64 i = StartPos; i <= EndPos; i++){
			var Param = z.Param(i+"");
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
/// </summary>
/// <param name="z"></param>
/// <param name="Start">含</param>
/// <param name="End">含</param>
/// <returns></returns>
	public static IList<str> MkParams(
		this ITable z
		,u64 Start
		,u64 End
	){
		var R = new List<str>();
		for(u64 i = Start; i <= End; i++){
			var Param = z.Param(i+"");
			R.Add(Param);
		}
		return R;
	}

	public static str SqlMkTbl(
		this ITable z
	){
		str OneCol(ITable Tbl, IColumn Col){
			var R = new List<str>();
			R.Add(Tbl.Quote(Col.NameInDb));
			if(!str.IsNullOrEmpty(Col.TypeInDb)){
				R.Add(Col.TypeInDb);
			}else{
				if(Col.RawClrType == null){
					var Msg = $"{Col.NameInDb}:\nCol.RawClrType == null";
					throw new Exception("Col.RawClrType == null");
				}
				try{
					var DbTypeName = z.SqlMkr.SqlTypeMapper.ToDbTypeName(Col.RawClrType);
					R.Add(DbTypeName);
				}
				catch (System.Exception e){
					throw new Exception("Type Mapping Error for Colunm:"+ Col.NameInDb, e);
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
CREATE TABLE IF NOT EXISTS {z.Quote(z.DbTblName)}(
	{string.Join(",\n\t", Lines)}{FmtInnerSqls(z.InnerAdditionalSqls)}
);
{FmtOuterSqls(z.OuterAdditionalSqls)}
""";
		return S;
	}


}

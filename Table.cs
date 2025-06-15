#define Impl
using System.Net;
using Ngaq.Core.Infra;
using Tsinswreng.CsSrcGen.DictMapper;
using IStr_Any = System.Collections.Generic.IDictionary<string, object?>;
using Str_Any = System.Collections.Generic.Dictionary<string, object?>;
namespace Tsinswreng.CsSqlHelper;



public class Table:ITable{
	public IDictMapper DictMapper{get;set;}
	public Type EntityType{get;set;}
	public Table(){}
	public Table(
		IDictMapper DictMapper
		,str Name
		,IDictionary<str, Type> ExampleDict
	){
		this.DictMapper = DictMapper;
		this.Name = Name;
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

	public static ITable Mk(
		IDictMapper DictMapper
		,str Name
		,IDictionary<str, Type> Key_Type
	){
		ITable ans = new Table{
			DictMapper = DictMapper
			,Name = Name
			,Key_Type = Key_Type
		}.Init();
		return ans;
	}

	// [Obsolete()]
	// public static I_Table Mk<TPo>(
	// 	IDictMapper DictMapper
	// 	,str Name, TPo ExamplePo
	// ){
	// 	I_Table ans = new Table{
	// 		Name = Name
	// 		,ExampleDict = DictMapper.ToDictT(ExamplePo)
	// 	}.Init();
	// 	return ans;
	// }

	public str Name{get;set;}
	#if Impl
	= "";
	#endif
	public IDictionary<str, IColumn> Columns{get;set;}
	#if Impl
	= new Dictionary<str, IColumn>();
	#endif
	/// <summary>
	/// 編程代碼中實體主鍵字段名
	/// </summary>
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


#pragma warning disable CS8601
public static class ExtnITable{

	[Obsolete("用ColBuilder")]
	public static IColumn OldSetCol(
		this ITable z
		,str NameInCode
		,str? NameInDb = null
		,Type? Type = null
	){
		// System.Console.WriteLine("一睡眠");
		// System.Console.WriteLine(
		// 	str.Join("\n", z.Columns.Keys)//t
		// );
		var col = z.Columns[NameInCode];
		if(NameInDb != null){
			col.NameInDb = NameInDb;
			z.DbColName_CodeColName[NameInDb] = NameInCode;
		}
		col.RawClrType = Type;
		return col;
	}



	[Obsolete("用泛型版本")]
	public static IColumn OldHasConversion(
		this IColumn z
		,Func<object?,object?> ToDbType
		,Func<object?,object?> ToCodeType
	){
		z.UpperToRaw = ToDbType;
		z.RawToUpper = ToCodeType;
		return z;
	}

	static i32 debug = 0;
	[Obsolete("用ColBuilder")]
	public static IColumn OldHasConversion<TCode,TDb>(
		this IColumn z
		,Func<TCode,TDb> ToDbType
		,Func<TDb,TCode> ToCodeType
	){

		z.UpperToRaw = (x)=>{
			try{
				return ToDbType((TCode)x!);
			}
			catch (System.Exception){
				System.Console.Error.WriteLine("Type Conversion Error for Colunm:"+ z.NameInDb);
				throw;
			}
		};
		z.RawToUpper = (x)=>{
			try{
				return ToCodeType((TDb)x!);
			}
			catch (System.Exception){
				System.Console.Error.WriteLine("Type Conversion Error for Colunm:"+ z.NameInDb);
				throw;
			}
		};

		// z.ToDbType = ToDbType;
		// z.ToCodeType = ToCodeType;
		return z;
	}

	public static str Quote(
		this ITable z
		,str s
	){
		return z.SqlMkr.Quote(s);
	}

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
/// <param name="z"></param>
/// <param name="CodeColName"></param>
/// <returns></returns>
	public static str Field(
		this ITable z
		,str CodeColName
	){
		var DbColName = z.Columns[CodeColName].NameInDb;
		return z.SqlMkr.Quote(DbColName);
	}

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
			var vCode = colCode.RawToUpper(vDb);
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
		z.DictMapper.AssignT(ToBeAssigned, CodeDict);
		return ToBeAssigned;
	}

	public static TPo DbDictToPo<TPo>(
		this ITable z
		,IStr_Any DbDict
	)where TPo:new(){
		var CodeDict = z.ToCodeDict(DbDict);
		var ans = new TPo();
		z.DictMapper.AssignT(ans, CodeDict);
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
			var vDb = Col.UpperToRaw(vCode);//-
			ans[Col.NameInDb] = vDb;
		}
		return ans;

	}

	public static object? ToDbType(
		this ITable z
		,str CodeColName
		,object? CodeValue
	){
		return z.Columns[CodeColName].UpperToRaw(CodeValue);
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
	public static IList<str> MkUnnamedParam(
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
CREATE TABLE IF NOT EXISTS {z.Quote(z.Name)}(
	{string.Join(",\n\t", Lines)}{FmtInnerSqls(z.InnerAdditionalSqls)}
);
{FmtOuterSqls(z.OuterAdditionalSqls)}
""";
		return S;
	}


}

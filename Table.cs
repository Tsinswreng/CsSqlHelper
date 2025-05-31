#define Impl
using Ngaq.Core.Infra;
using Tsinswreng.SrcGen.Dict;
using IStr_Any = System.Collections.Generic.IDictionary<string, object?>;
using Str_Any = System.Collections.Generic.Dictionary<string, object?>;
namespace Tsinswreng.SqlHelper;



public class Table:ITable{
	public IDictMapper DictMapper{get;set;}
	public Type EntityType{get;set;}
	public Table(){}
	public Table(
		IDictMapper DictMapper
		,str Name
		,IStr_Any ExampleDict
	){
		this.DictMapper = DictMapper;
		this.Name = Name;
		this.ExampleDict = ExampleDict;
	}

	bool _Inited = false;
	public ITable Init(){
		if(_Inited){
			return this;
		}
		foreach(var (k,v) in ExampleDict){
			var column = new Column();
			column.NameInDb = k;
			Columns[k] = column;
			DbColName__CodeColName[k] = k;
		}
		_Inited = true;
		return this;
	}

	public static ITable Mk(
		IDictMapper DictMapper
		,str Name
		,IStr_Any ExampleDict
	){
		ITable ans = new Table{
			DictMapper = DictMapper
			,Name = Name
			,ExampleDict = ExampleDict
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
	public str CodeIdName{get;set;}
	#if Impl
	= "Id";
	#endif
	public IDictionary<str, str> DbColName__CodeColName{get;set;}
	#if Impl
	= new Dictionary<str, str>();
	#endif
	public IStr_Any ExampleDict{get;set;}
	#if Impl
	= new Str_Any();
	#endif

	public ISqlMkr SqlMkr{get;set;}
}


#pragma warning disable CS8601
public static class Extn_I_Table{
	public static IColumn SetCol(
		this ITable z
		,str NameInCode
		,str? NameInDb = null
	){
		// System.Console.WriteLine("一睡眠");
		// System.Console.WriteLine(
		// 	str.Join("\n", z.Columns.Keys)//t
		// );
		var col = z.Columns[NameInCode];
		if(NameInDb != null){
			col.NameInDb = NameInDb;
			z.DbColName__CodeColName[NameInDb] = NameInCode;
		}
		return col;
	}

	public static IColumn HasConversion(
		this IColumn z
		,Func<object?,object?> ToDbType
		,Func<object?,object?> ToCodeType
	){
		z.ToDbType = ToDbType;
		z.ToCodeType = ToCodeType;
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
			var kCode = z.DbColName__CodeColName[kDb];
			var colCode = z.Columns[kCode];
			var vCode = colCode.ToCodeType(vDb);
			ans[kCode] = vCode;
		}
		return ans;
	}

	public static T_Po DbDictToPo<T_Po>(
		this ITable z
		,IStr_Any DbDict
	)where T_Po:new(){
		var CodeDict = z.ToCodeDict(DbDict);
		var ans = new T_Po();
		z.DictMapper.AssignT(ans, CodeDict);
		return ans;
	}

	public static IStr_Any ToDbDict(
		this ITable z
		,IStr_Any CodeDict
	){
		var ans = new Str_Any();
		foreach(var (kCode, vCode) in CodeDict){
			var Col = z.Columns[kCode];
			var vDb = Col.ToDbType(vCode);
			ans[Col.NameInDb] = vDb;
		}
		return ans;
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


}

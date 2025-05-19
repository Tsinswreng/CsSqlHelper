#define Impl
using Ngaq.Core.Infra;

namespace Tsinswreng.SqlHelper;



public class Table:I_Table{
	public Type EntityType{get;set;}
	public Table(){}
	public Table(str Name, IDictionary<str, object> ExampleDict){
		this.Name = Name;
		this.ExampleDict = ExampleDict;
	}

	bool _Inited = false;
	public I_Table Init(){
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
	public static I_Table Mk<T_Po>(str Name, T_Po ExamplePo){
		I_Table ans = new Table{
			Name = Name
			,ExampleDict = DictCtx.ToDictT(ExamplePo)
		}.Init();
		return ans;
	}

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


#pragma warning disable CS8601
public static class Extn_I_Table{
	public static I_Column SetCol(
		this I_Table z
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

	public static I_Column HasConversion(
		this I_Column z
		,Func<object?,object?> ToDbType
		,Func<object?,object?> ToCodeType
	){
		z.ToDbType = ToDbType;
		z.ToCodeType = ToCodeType;
		return z;
	}

	public static str Quote(
		this I_Table z
		,str s
	){
		return z.SqlMkr.Quote(s);
	}

	public static str ToDbName(
		this I_Table z
		,str CodeColName
	){
		var DbColName = z.Columns[CodeColName].NameInDb;
		return DbColName;
	}

	public static str Field(
		this I_Table z
		,str CodeColName
	){
		var DbColName = z.Columns[CodeColName].NameInDb;
		return z.SqlMkr.Quote(DbColName);
	}

	public static str Param(
		this I_Table z
		,str CodeColName
	){
		return z.SqlMkr.Param(CodeColName);
	}


	public static IDictionary<str, object> ToCodeDict(
		this I_Table z
		,IDictionary<str, object> DbDict
	){
		var ans = new Dictionary<str, object>();
		foreach(var (kDb, vDb) in DbDict){
			var kCode = z.DbColName__CodeColName[kDb];
			var colCode = z.Columns[kCode];
			var vCode = colCode.ToCodeType(vDb);
			ans[kCode] = vCode;
		}
		return ans;
	}

	public static T_Po DbDictToPo<T_Po>(
		this I_Table z
		,IDictionary<str, object> DbDict
	)where T_Po:new(){
		var CodeDict = z.ToCodeDict(DbDict);
		var ans = new T_Po();
		DictCtx.AssignT(ans, CodeDict);
		return ans;
	}

	public static IDictionary<str, object> ToDbDict(
		this I_Table z
		,IDictionary<str, object> CodeDict
	){
		var ans = new Dictionary<str, object>();
		foreach(var (kCode, vCode) in CodeDict){
			var Col = z.Columns[kCode];
			var vDb = Col.ToDbType(vCode);
			ans[Col.NameInDb] = vDb;
		}
		return ans;
	}




	public static str UpdateClause(
		this I_Table z
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
		this I_Table z
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

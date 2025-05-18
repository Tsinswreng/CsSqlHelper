#define Impl
using Ngaq.Core.Infra;

namespace Tsinswreng.SqlHelper;



public class Table:I_Table{
	public Type EntityType{get;set;}
	public Table(){}
	public Table(str Name, IDictionary<str, object> ExampleDict){
		this.Name = Name;
		this.ExampleDict = ExampleDict;
		foreach(var (k,v) in ExampleDict){
			var column = new Column();
			column.NameInDb = k;
			Columns[k] = column;
		}
	}
	public static I_Table Mk<T_Po>(str Name, T_Po ExamplePo){
		I_Table ans = new Table{
			Name = Name
			,ExampleDict = DictCtx.ToDict(ExamplePo)
		};
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
}


public static class Extn_I_Table{
	public static I_Column SetCol(
		this I_Table z
		,str NameInCode
		,str? NameInDb = null
	){
		var col = z.Columns[NameInCode];
		if(NameInDb != null){
			col.NameInDb = NameInDb;
		}
		return col;
	}

	public static I_Column HasConversion(
		this I_Column z
		,Func<object,object> ToDbType
		,Func<object,object> ToCodeType
	){
		z.ToDbType = ToDbType;
		z.ToCodeType = ToCodeType;
		return z;
	}
}

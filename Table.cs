// #define Impl
// using Ngaq.Core.Infra;

// namespace Tsinswreng.SqlHelper;



// public class Table:I_Table{
// 	public Table(){}
// 	public Table(str Name, IDictionary<str, object> ExampleDict){
// 		this.Name = Name;
// 		this.ExampleDict = ExampleDict;
// 		foreach(var (k,v) in ExampleDict){
// 			var column = new Column();
// 			column.NameInDb = k;
// 			Columns[k] = column;
// 		}
// 	}
// 	public static Table Mk<T_Po>(str Name, T_Po ExamplePo){
// 		var ans = new Table{
// 			Name = Name
// 			,ExampleDict = DictCtx.ToDict(ExamplePo)
// 		};
// 		return ans;
// 	}
// 	public str Name{get;set;}
// 	#if Impl
// 	= "";
// 	#endif
// 	public IDictionary<str, I_Column> Columns{get;set;}
// 	#if Impl
// 	= new Dictionary<str, I_Column>();
// 	#endif
// 	public IDictionary<str, object> ExampleDict{get;set;}
// 	#if Impl
// 	= new Dictionary<str, object>();
// 	#endif
// }



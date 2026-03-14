#define Impl
namespace Tsinswreng.CsSql;


//類型映射與字段映射
public partial class Column: IColumn{
	
	/// 在數據庫中 字段ʹ名
	
	public string DbName { get; set; } = "";
	public str DbType{get;set;} = "";
	public Type? RawClrType{get;set;}
	public Type? UpperClrType{get;set;}
	public IList<str> AdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
	public bool NotNull{get;set;}
	public IUpperTypeMapFn? UpperTypeMapper{get;set;}
	// public Func<object?,object?>? UpperToRaw{get;set;} = (x)=>x;
	// public Func<object?,object?>? RawToUpper{get;set;} = (x)=>x;
}


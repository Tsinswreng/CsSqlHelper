#define Impl
namespace Tsinswreng.CsSqlHelper;


//類型映射與字段映射
public partial class Column: IColumn{
	/// <summary>
	/// 在數據庫中 字段ʹ名
	/// </summary>
	public string DbName { get; set; } = "";
	public str DbType{get;set;} = "";
	public Type? RawCodeType{get;set;}
	public Type? UpperCodeType{get;set;}
	public IList<str> AdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif
	public bool NotNull{get;set;}
	public IUpperTypeMapFn? UpperTypeMapper{get;set;}
	// public Func<object?,object?>? UpperToRaw{get;set;} = (x)=>x;
	// public Func<object?,object?>? RawToUpper{get;set;} = (x)=>x;
}


public class Column<TTbl, TRaw, TUpper> : Column, IColumn<TTbl, TRaw, TUpper>{

}

// public class Column<T> : Column<T, T>, IColumn<T>{

// }

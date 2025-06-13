namespace Tsinswreng.CsSqlHelper;
//類型映射與字段映射
public interface IColumn{
	/// <summary>
	/// 在數據庫中 字段ʹ名
	/// </summary>
	public string NameInDb { get; set; }
	/// <summary>
	/// 完整ʹ類型 如 TEXT  VARCHAR(64)。 不當獨VARCHAR洏缺後ʹ(n)
	/// </summary>
	public str TypeInDb{get;set;}
	/// <summary>
	/// 有自封裝ʹ類型旹 即其內ʹ原始類型
	/// </summary>
	public Type? RawClrType{get;set;}
	/// <summary>
	/// 自封裝ʹ類型
	/// </summary>
	public Type? UpperClrType{get;set;}
	public IList<str>? AdditionalSqls{get;set;}
	public bool NotNull{get;set;}

	// public object ToDbType(object CodeType){
	// 	return CodeType;
	// }
	public Func<object?,object?> UpperToRaw{get;set;}
	#if Impl
	= (object? CodeType)=>{return CodeType;};
	#endif


	public Func<object?,object?> RawToUpper{get;set;}
	#if Impl
	= (object? DbType)=>{return DbType;};
	#endif

}


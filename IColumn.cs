namespace Tsinswreng.SqlHelper;
//類型映射與字段映射
public interface IColumn{
	/// <summary>
	/// 在數據庫中 字段ʹ名
	/// </summary>
	public string NameInDb { get; set; }
	public str TypeNameInDb{get;set;}
	public Type TypeInCode{get;set;}

	// public object ToDbType(object CodeType){
	// 	return CodeType;
	// }
	public Func<object?,object?> ToDbType{get;set;}
	#if Impl
	= (object? CodeType)=>{return CodeType;};
	#endif


	public Func<object?,object?> ToCodeType{get;set;}
	#if Impl
	= (object? DbType)=>{return DbType;};
	#endif

}


namespace Tsinswreng.SqlHelper;

//類型映射與字段映射
public interface I_Column{
	/// <summary>
	/// 在數據庫中 字段ʹ名
	/// </summary>
	public string NameInDb { get; set; }

	public str TypeInDb{get;set;}

	public object ToDbType(object CodeType){
		return CodeType;
	}

	public object ToCodeType(object DbType){
		return DbType;
	}
}



//類型映射與字段映射
public class Column: I_Column{
	/// <summary>
	/// 在數據庫中 字段ʹ名
	/// </summary>
	public string NameInDb { get; set; } = "";

	public str TypeInDb{get;set;} = "";


}

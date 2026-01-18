namespace Tsinswreng.CsSqlHelper;

public partial interface IArgDict{
	public IDictionary<str, obj?> ParamName_RawValue{get;set;}
	/// <summary>
	/// 裸ʹ參數名、不帶前ʹ修飾符如"@"
	/// </summary>
	/// <param name="ParamName"></param>
	/// <param name="Raw"></param>
	/// <returns></returns>
	public IArgDict AddRaw(str ParamName, obj? Raw);
	public IArgDict AddRaw(IParam Param, obj? Raw);
	public IArgDict AddT<T>(IParam Param, T Raw, str? CodeColName=null);
	/// <summary>
	/// 批量綁定多ʹ參數
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="Params"></param>
	/// <param name="Uppers"></param>
	/// <param name="CodeColName"></param>
	/// <param name="Alt">若<paramref name="Uppers"/>長度不等於<paramref name="Params"/>長度，則用此值替代缺失值</param>
	/// <returns></returns>
	public IArgDict AddManyT<T>(
		IList<IParam> Params, IList<T> Uppers
		,str? CodeColName=null, obj? Alt=null
	);
	public IDictionary<str, obj?> ToDict();
}


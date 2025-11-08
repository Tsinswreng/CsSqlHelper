using System.Collections;
using Tsinswreng.CsCore;
using Tsinswreng.CsPage;

namespace Tsinswreng.CsSqlHelper;

public partial class ArgDict: IArgDict{
	public static ArgDict Mk(){
		return new ArgDict();
	}
	public static ArgDict Mk(ITable? Tbl){
		return new ArgDict{Tbl = Tbl};
	}

	public IDictionary<str, obj?> ParamName_RawValue{get;set;} = new Dictionary<str, obj?>();
	public ITable? Tbl{get;set;}

	[Impl]
	public IArgDict AddRaw(str ParamName, obj? Raw){
		ParamName_RawValue.TryAdd(ParamName, Raw);
		return this;
	}

	[Impl]
	public IArgDict AddRaw(IParam Param, obj? Raw){
		ParamName_RawValue.TryAdd(Param.Name, Raw);
		return this;
	}

	/// <summary>
	/// 嘗試 Upper ->Raw轉換、轉不得則添 Upper
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="Param"></param>
	/// <param name="Upper"></param>
	/// <param name="CodeColName"></param>
	/// <returns></returns>
	public IArgDict AddT<T>(IParam Param, T Upper, str? CodeColName=null){
		if(Tbl is null){
			return AddRaw(Param, Upper);
		}
		var Raw = Tbl.UpperToRaw(Upper, CodeColName);
		ParamName_RawValue.TryAdd(Param.Name, Raw);
		return this;
	}

	[Impl]
	public IDictionary<str, obj?> ToDict(){
		return ParamName_RawValue;
	}
}


public static class ExtnArgDict{
	public static IArgDict AddPageQry(
		this IArgDict z
		,IPageQry PageQry
		,IParam PrmLmt
		,IParam PrmOfst
	){
		z.AddRaw(PrmLmt, PageQry.PageSize)
		.AddRaw(PrmOfst, PageQry.Offset_());
		return z;
	}

	// public static IArgDict Map(
	// 	this IArgDict z
	// 	,ITable Tbl
	// 	,str ParamName
	// ){

	// }
}

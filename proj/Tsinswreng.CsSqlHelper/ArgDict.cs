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
	public IArgDict Add(str ParamName, obj? Raw){
		ParamName_RawValue.TryAdd(ParamName, Raw);
		return this;
	}

	[Impl]
	public IArgDict Add(IParam Param, obj? Raw){
		ParamName_RawValue.TryAdd(Param.Name, Raw);
		return this;
	}

	public IArgDict AddConv<T>(IParam Param, T Upper, str? CodeColName=null){
		if(Tbl is null){
			throw new Exception("Tbl is null");
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
		z.Add(PrmLmt, PageQry.PageSize)
		.Add(PrmOfst, PageQry.Offset_());
		return z;
	}

	// public static IArgDict Map(
	// 	this IArgDict z
	// 	,ITable Tbl
	// 	,str ParamName
	// ){

	// }
}

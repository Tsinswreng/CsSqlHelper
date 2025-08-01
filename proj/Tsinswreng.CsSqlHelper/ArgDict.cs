using System.Collections;
using Tsinswreng.CsCore;
using Tsinswreng.CsPage;

namespace Tsinswreng.CsSqlHelper;

public partial class ArgDict: IArgDict{
	public static IArgDict Mk(){
		return new ArgDict();
	}
	public static IArgDict Mk(ITable? Tbl){
		return new ArgDict{Tbl = Tbl};
	}

	public IDictionary<str, obj?> Dict{get;set;} = new Dictionary<str, obj?>();
	public ITable? Tbl{get;set;}

	[Impl]
	public IArgDict Add(str ParamName, obj? Value){
		Dict.TryAdd(ParamName, Value);
		return this;
	}

	[Impl]
	public IDictionary<str, obj?> ToDict(){
		return Dict;
	}
}


public static class ExtnArgDict{
	public static IArgDict AddPageQry(
		this IArgDict z
		,IPageQuery PageQry
		,str PrmLmt
		,str PrmOfst
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

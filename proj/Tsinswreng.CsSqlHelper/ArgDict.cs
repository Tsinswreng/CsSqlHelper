using System.Collections;
using Tsinswreng.CsCore;
using Tsinswreng.CsPage;

namespace Tsinswreng.CsSqlHelper;

public class ArgDict: IArgDict{
	public static IArgDict Mk(){
		return new ArgDict();
	}
	public IDictionary<str, obj?> Dict{get;set;} = new Dictionary<str, obj?>();

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
}

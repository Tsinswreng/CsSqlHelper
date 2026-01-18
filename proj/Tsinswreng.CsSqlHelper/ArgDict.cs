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
	public IArgDict AddT<T>(IParam Param, T Upper, str? CodeColName=null){
		if(Tbl is null){
			return AddRaw(Param, Upper);
		}
		var Raw = Tbl.UpperToRaw(Upper, CodeColName);
		ParamName_RawValue.TryAdd(Param.Name, Raw);
		return this;
	}


	public IArgDict AddManyT<T>(
		IList<IParam> Params, IList<T> Uppers
		,str? CodeColName=null, obj? Alt=null
	){
		var z = this;
		if(z.Tbl is null){
			throw new NullReferenceException("Tbl is null");
		}
		for(var i = 0; i < Params.Count; i++){
			var Param = Params[i];

			if(i >= Uppers.Count){
				var Raw = z.Tbl.UpperToRaw(Alt, typeof(T), CodeColName);
				ParamName_RawValue.TryAdd(Param.Name, Raw);
			}else{
				var Raw = z.Tbl.UpperToRaw(Uppers[i], CodeColName);
				ParamName_RawValue.TryAdd(Param.Name, Raw);
			}
		}
		return this;
	}


	[Impl]
	public IDictionary<str, obj?> ToDict(){
		return ParamName_RawValue;
	}
}


public static class ExtnArgDict{
	extension(IArgDict z){
		public IArgDict AddPageQry(
			IPageQry PageQry
			,IParam PrmLmt
			,IParam PrmOfst
		){
			z.AddRaw(PrmLmt, PageQry.PageSize)
			.AddRaw(PrmOfst, PageQry.Offset_());
			return z;
		}

	}

	// public static IArgDict Map(
	// 	this IArgDict z
	// 	,ITable Tbl
	// 	,str ParamName
	// ){

	// }
}

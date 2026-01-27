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

	/// 嘗試 Upper ->Raw轉換、轉不得則添 Upper
	public IArgDict AddT<T>(IParam Param, T Upper, str? CodeColName=null){
		if(Tbl is null){
			return AddRaw(Param, Upper);
		}
		var Raw = Tbl.UpperToRaw(Upper, CodeColName);
		ParamName_RawValue.TryAdd(Param.Name, Raw);
		return this;
	}

	public IArgDict AddManyT<T>(
		IParam Param, IEnumerable<T> Uppers
		,str? CodeColName=null
	){
		var z = this;
		if(z.Tbl is null){
			throw new NullReferenceException("Tbl is null");
		}
		foreach(var (i, Upper) in Uppers.Index()){
			var numParam = Param.ToOfst((u64)i);
			var Raw = z.Tbl.UpperToRaw(Upper, CodeColName);
			ParamName_RawValue.TryAdd(numParam.Name, Raw);
		}
		return this;
	}

	public IArgDict AddManyT<T>(
		IEnumerable<IParam> Params, IEnumerable<T> Uppers
		,str? CodeColName=null, obj? Alt=null
	){
		var z = this;
		if(z.Tbl is null){
			throw new NullReferenceException("Tbl is null");
		}

		// 獲取枚舉器並確保釋放，完全匹配原代碼變量命名風格
		using var ParamEnum = Params.GetEnumerator();
		using var UpperEnum = Uppers.GetEnumerator();
		bool UppersExhausted = false;

		// 遍歷所有Param，Uppers用完後用Alt填充
		while(ParamEnum.MoveNext()){
			var CurrentParam = ParamEnum.Current;
			obj? Raw = null;

			if(!UppersExhausted){
				// 僅移動一次枚舉器，避免重複調用拋異常
				UppersExhausted = !UpperEnum.MoveNext();
			}

			if(!UppersExhausted){
				Raw = z.Tbl.UpperToRaw(UpperEnum.Current, CodeColName);
			}else{
				Raw = z.Tbl.UpperToRaw(Alt, typeof(T), CodeColName);
			}

			ParamName_RawValue.TryAdd(CurrentParam.Name, Raw);
		}

		return this;
	}

	// public IArgDict AddManyT<T>(
	// 	IList<IParam> Params, IList<T> Uppers
	// 	,str? CodeColName=null, obj? Alt=null
	// ){
	// 	var z = this;
	// 	if(z.Tbl is null){
	// 		throw new NullReferenceException("Tbl is null");
	// 	}
	// 	for(var i = 0; i < Params.Count; i++){
	// 		var Param = Params[i];

	// 		if(i >= Uppers.Count){
	// 			var Raw = z.Tbl.UpperToRaw(Alt, typeof(T), CodeColName);
	// 			ParamName_RawValue.TryAdd(Param.Name, Raw);
	// 		}else{
	// 			var Raw = z.Tbl.UpperToRaw(Uppers[i], CodeColName);
	// 			ParamName_RawValue.TryAdd(Param.Name, Raw);
	// 		}
	// 	}
	// 	return this;
	// }


	[Impl]
	public IDictionary<str, obj?> ToDict(){
		return ParamName_RawValue;
	}
}


public static class ExtnArgDict{
	extension(IArgDict z){
		[Doc(@$"
		#Params([Page Query], [Param for Limit], [Param for Offset])
		")]
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

}

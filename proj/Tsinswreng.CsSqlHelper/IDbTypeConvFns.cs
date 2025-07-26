namespace Tsinswreng.CsSqlHelper;

public interface IDbTypeConvFns<TRaw, TUpper>{
	public Func<TUpper,TRaw>? UpperToRaw{get;set;}
	public Func<TRaw,TUpper>? RawToUpper{get;set;}
	public Func<object?, TRaw>? ObjToRaw{get;set;}
	public Func<object?, TUpper>? ObjToUpper{get;set;}
}

public  partial class DbTypeConvFns<TRaw, TUpper>
	:IDbTypeConvFns<TRaw, TUpper>
{
	public Func<TUpper,TRaw>? UpperToRaw{get;set;}
	public Func<TRaw,TUpper>? RawToUpper{get;set;}
	public Func<object?, TRaw>? ObjToRaw{get;set;}
	public Func<object?, TUpper>? ObjToUpper{get;set;}
	public static IDbTypeConvFns<TRaw2, TUpper2> Mk<TRaw2, TUpper2>(
		Func<TUpper2,TRaw2> UpperToRaw
		,Func<TRaw2,TUpper2> RawToUpper
		,Func<object?, TRaw2>? ObjToRaw = null
		,Func<object?, TUpper2>? ObjToUpper = null
	){
		var R = new DbTypeConvFns<TRaw2, TUpper2>{
			UpperToRaw = UpperToRaw
			,RawToUpper = RawToUpper
			,ObjToRaw = ObjToRaw
			,ObjToUpper = ObjToUpper
		};
		return R;
	}

	public static IDbTypeConvFns<TRaw, TUpper> Mk(
		Func<TUpper,TRaw> UpperToRaw
		,Func<TRaw,TUpper> RawToUpper
		,Func<object?, TRaw>? ObjToRaw = null
		,Func<object?, TUpper>? ObjToUpper = null
	){
		var R = new DbTypeConvFns<TRaw, TUpper>{
			UpperToRaw = UpperToRaw
			,RawToUpper = RawToUpper
			,ObjToRaw = ObjToRaw
			,ObjToUpper = ObjToUpper
		};
		return R;
	}
}

namespace Tsinswreng.CsSqlHelper;

public partial interface IDbTypeConvFns<TRaw, TUpper>{
	public Func<TUpper,TRaw>? UpperToRaw{get;set;}
	public Func<TRaw,TUpper>? RawToUpper{get;set;}
	public Func<object?, TRaw>? ObjToRaw{get;set;}
	public Func<object?, TUpper>? ObjToUpper{get;set;}
}

public partial class DbTypeConvFns<TRaw, TUpper>
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


public static partial class DbTypeConvFns{
	public static IUpperTypeMapFn ToNonGeneric<TRaw, TUpper>(
		this IDbTypeConvFns<TRaw, TUpper> z
	){
		var UpperToRaw = z.UpperToRaw;
		var RawToUpper = z.RawToUpper;
		var ObjToRaw = z.ObjToRaw;
		var ObjToUpper = z.ObjToUpper;
		var R = new UpperTypeMapFn();
		R.UpperToRaw = (x)=>{
			try{
				if(UpperToRaw == null){return x;}
				if(ObjToUpper != null){
					return UpperToRaw(ObjToUpper(x));
				}
				return UpperToRaw((TUpper)x!);
			}
			catch (System.Exception){
				System.Console.Error.WriteLine("Type Conversion Error. Raw:"+typeof(TRaw)+", Upper:"+typeof(TUpper));
				throw;
			}
		};
		R.RawToUpper = (x)=>{
			try{
				if(RawToUpper == null){return x;}
				if(ObjToRaw != null){
					return RawToUpper(ObjToRaw(x));
				}
				return RawToUpper((TRaw)x!);
			}
			catch (System.Exception){
				System.Console.Error.WriteLine("Type Conversion Error. Raw:"+typeof(TRaw)+", Upper:"+typeof(TUpper));
				throw;
			}
		};
		return R;
	}
}

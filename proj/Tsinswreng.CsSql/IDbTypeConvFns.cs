namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// 帶泛型
/// </summary>
/// <typeparam name="TRaw"></typeparam>
/// <typeparam name="TUpper"></typeparam>
public partial interface IUpperTypeMapFnT<TRaw, TUpper>{
	public Func<TUpper,TRaw>? UpperToRaw{get;set;}
	public Func<TRaw,TUpper>? RawToUpper{get;set;}
	public Func<obj?, TRaw>? ObjToRaw{get;set;}
	public Func<obj?, TUpper>? ObjToUpper{get;set;}
}

public partial class UpperTypeMapFnT<TRaw, TUpper>
	:IUpperTypeMapFnT<TRaw, TUpper>

{
	public Func<TRaw,TUpper>? RawToUpper{get;set;}
	public Func<TUpper,TRaw>? UpperToRaw{get;set;}
	public Func<obj?, TRaw>? ObjToRaw{get;set;}
	public Func<obj?, TUpper>? ObjToUpper{get;set;}

	public static IUpperTypeMapFnT<TRaw2, TUpper2> Mk<TRaw2, TUpper2>(
		Func<TRaw2,TUpper2> RawToUpper
		,Func<TUpper2,TRaw2> UpperToRaw
		,Func<obj?, TUpper2>? ObjToUpper = null
		,Func<obj?, TRaw2>? ObjToRaw = null
	){
		var R = new UpperTypeMapFnT<TRaw2, TUpper2>{
			UpperToRaw = UpperToRaw
			,RawToUpper = RawToUpper
			,ObjToRaw = ObjToRaw
			,ObjToUpper = ObjToUpper
		};
		return R;
	}

	[Obsolete]
	public static IUpperTypeMapFnT<TRaw2, TUpper2> MkOld<TRaw2, TUpper2>(
		Func<TUpper2,TRaw2> UpperToRaw
		,Func<TRaw2,TUpper2> RawToUpper
		,Func<obj?, TRaw2>? ObjToRaw = null
		,Func<obj?, TUpper2>? ObjToUpper = null
	){
		var R = new UpperTypeMapFnT<TRaw2, TUpper2>{
			UpperToRaw = UpperToRaw
			,RawToUpper = RawToUpper
			,ObjToRaw = ObjToRaw
			,ObjToUpper = ObjToUpper
		};
		return R;
	}

	public static IUpperTypeMapFnT<TRaw, TUpper> Mk(
		Func<TRaw,TUpper> RawToUpper
		,Func<TUpper,TRaw> UpperToRaw
		,Func<obj?, TUpper>? ObjToUpper = null
		,Func<obj?, TRaw>? ObjToRaw = null
	){
		var R = new UpperTypeMapFnT<TRaw, TUpper>{
			UpperToRaw = UpperToRaw
			,RawToUpper = RawToUpper
			,ObjToRaw = ObjToRaw
			,ObjToUpper = ObjToUpper
		};
		return R;
	}

	[Obsolete]
	public static IUpperTypeMapFnT<TRaw, TUpper> MkOld(
		Func<TUpper,TRaw> UpperToRaw
		,Func<TRaw,TUpper> RawToUpper
		,Func<obj?, TRaw>? ObjToRaw = null
		,Func<obj?, TUpper>? ObjToUpper = null
	){
		var R = new UpperTypeMapFnT<TRaw, TUpper>{
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
		this IUpperTypeMapFnT<TRaw, TUpper> z
	){
		var UpperToRaw = z.UpperToRaw;
		var RawToUpper = z.RawToUpper;
		var ObjToRaw = z.ObjToRaw;
		var ObjToUpper = z.ObjToUpper;
		var R = new UpperTypeMapFn();
		R.UpperToRaw = (x)=>{
			try{
				if(x is null){
					return null; // 2026_0117_100146
				}
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
				if(x is null){
					return null; // 2026_0117_100146
				}
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

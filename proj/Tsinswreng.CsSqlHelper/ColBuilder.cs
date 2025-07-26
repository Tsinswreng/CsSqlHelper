namespace Tsinswreng.CsSqlHelper;

using Self = ColBuilder;
/// <summary>
/// A helper class to build a `IColumn` object.
/// </summary>
public  partial class ColBuilder{
	public ITable Table{get;set;}
	public IColumn Column { get; set; }
	public ColBuilder(
		ITable Table
		,IColumn Column
	){
		this.Table = Table;
		this.Column = Column;
	}

}


public static class ExtnColBuilder{

	public static IColumn Build(
		this Self z
	){
		return z.Column;
	}

	public static Self SetCol(
		this ITable z
		,str NameInCode
	){
		var col = z.Columns[NameInCode];
		var R = new ColBuilder(z, col);
		return R;
	}

	public static Self Type<TRaw, TUpper>(
		this Self z
		,str? TypeNameInDb = null
	){
		if(TypeNameInDb != null){
			z.Column.TypeInDb = TypeNameInDb;
		}
		z.Column.RawClrType = typeof(TRaw);
		z.Column.UpperClrType = typeof(TUpper);
		return z;
	}

	public static Self Type<TRaw>(
		this Self z
		,str? TypeInDb = null
	){
		return Type<TRaw,TRaw>(z, TypeInDb);
	}


	public static Self NotNull(
		this Self z
	){
		z.Column.NotNull = true;
		return z;
	}


/// <summary>
/// 見 HasConversionʹ註
/// 用強轉 轉作Func<object?, object?>。
/// 需保證入參強轉不報錯
/// </summary>
/// <typeparam name="TRaw"></typeparam>
/// <typeparam name="TUpper"></typeparam>
/// <param name="z"></param>
/// <param name="UpperToRaw"></param>
/// <param name="RawToUpper"></param>
/// <returns></returns>
	public static Self HasConversionEtMapType<TRaw, TUpper>(
		this Self z
		,Func<TUpper,TRaw> UpperToRaw
		,Func<TRaw,TUpper> RawToUpper
		,Func<object?, TRaw>? ObjToRaw = null
		,Func<object?, TUpper>? ObjToUpper = null
	){
		var Fns = DbTypeConvFns<nil, nil>.Mk<TRaw, TUpper>(UpperToRaw, RawToUpper, ObjToRaw, ObjToUpper);
		return HasConversionEtMapType(z, Fns);
	}

	public static Self HasConversionEtMapType<TRaw, TUpper>(
		this Self z
		,IDbTypeConvFns<TRaw, TUpper> Fns
	){
		var col = z.Column;
		col.RawClrType = typeof(TRaw);
		col.UpperClrType = typeof(TUpper);
		return HasConversion(z, Fns);
	}

	public static Self HasConversion<TRaw, TUpper>(
		this Self z
		,IDbTypeConvFns<TRaw, TUpper> Fns
	){
		var col = z.Column;
		var UpperToRaw = Fns.UpperToRaw;
		var RawToUpper = Fns.RawToUpper;
		var ObjToRaw = Fns.ObjToRaw;
		var ObjToUpper = Fns.ObjToUpper;
		col.UpperToRaw = (x)=>{
			try{
				if(UpperToRaw == null){return x;}
				if(ObjToUpper != null){
					return UpperToRaw(ObjToUpper(x));
				}
				return UpperToRaw((TUpper)x!);
			}
			catch (System.Exception){
				System.Console.Error.WriteLine("Type Conversion Error for Colunm:"+ col.NameInDb);
				throw;
			}
		};
		col.RawToUpper = (x)=>{
			try{
				if(RawToUpper == null){return x;}
				if(ObjToRaw != null){
					return RawToUpper(ObjToRaw(x));
				}
				return RawToUpper((TRaw)x!);
			}
			catch (System.Exception){
				System.Console.Error.WriteLine("Type Conversion Error for Colunm:"+ col.NameInDb);
				throw;
			}
		};
		return z;
	}

/// <summary>
/// 用強轉 轉作Func<object?, object?>。
/// 需保證入參強轉不報錯
/// 例
/// o.SetCol(nameof(IPoBase.Status)).HasConversion<i32, PoStatus>(
/// 	s=>Convert.ToInt32(s.Value),
/// 	val=>new PoStatus(val)
/// );
/// 用sqlite旹 緣sqlite中 只有i64洏無i32、故實際取出之原始類型潙i64。return ToUpperType((TRaw)x!);則報錯
/// </summary>
/// <typeparam name="TRaw"></typeparam>
/// <typeparam name="TUpper"></typeparam>
/// <param name="z"></param>
/// <param name="UpperToRaw"></param>
/// <param name="RawToUpper"></param>
/// <returns></returns>
	public static Self HasConversion<TRaw, TUpper>(
		this Self z
		,Func<TUpper,TRaw> UpperToRaw
		,Func<TRaw,TUpper> RawToUpper
		,Func<object?, TRaw>? ObjToRaw = null
		,Func<object?, TUpper>? ObjToUpper = null
	){
		var Fns = DbTypeConvFns<nil, nil>.Mk<TRaw, TUpper>(UpperToRaw, RawToUpper, ObjToRaw, ObjToUpper);
		return HasConversion(z, Fns);
	}

	public static Self AdditionalSqls(
		this Self z
		,IEnumerable<str> Sqls
	){
		foreach(var sql in Sqls){
			z.Column.AdditionalSqls.Add(sql);
		}
		return z;
	}

}

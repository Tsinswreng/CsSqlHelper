namespace Tsinswreng.CsSqlHelper;

using Self = ColBuilder;
/// <summary>
/// A helper class to build a `IColumn` object.
/// </summary>
public partial class ColBuilder{
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
			z.Column.DbType = TypeNameInDb;
		}
		z.Column.RawCodeType = typeof(TRaw);
		z.Column.UpperCodeType = typeof(TUpper);
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
	public static Self MapType<TRaw, TUpper>(
		this Self z
		,Func<TUpper,TRaw> UpperToRaw
		,Func<TRaw,TUpper> RawToUpper
		,Func<object?, TRaw>? ObjToRaw = null
		,Func<object?, TUpper>? ObjToUpper = null
	){
		var Fns = UpperTypeMapFnT<nil, nil>.Mk<TRaw, TUpper>(UpperToRaw, RawToUpper, ObjToRaw, ObjToUpper);
		return MapType(z, Fns);
	}

	public static Self MapType<TRaw, TUpper>(
		this Self z
		,IUpperTypeMapFnT<TRaw, TUpper> Fns
	){
		var col = z.Column;
		col.RawCodeType = typeof(TRaw);
		col.UpperCodeType = typeof(TUpper);
		return HasConv(z, Fns);
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
	public static Self HasConv<TRaw, TUpper>(
		this Self z
		,Func<TUpper,TRaw> UpperToRaw
		,Func<TRaw,TUpper> RawToUpper
		,Func<object?, TRaw>? ObjToRaw = null
		,Func<object?, TUpper>? ObjToUpper = null
	){
		var Fns = UpperTypeMapFnT<nil, nil>.Mk<TRaw, TUpper>(UpperToRaw, RawToUpper, ObjToRaw, ObjToUpper);
		return HasConv(z, Fns);
	}

	public static Self HasConv<TRaw, TUpper>(
		this Self z
		,IUpperTypeMapFnT<TRaw, TUpper> Fns
	){
		z.Column.UpperTypeMapper = Fns.ToNonGeneric();
		z.Table.UpperType_DfltMapper.TryAdd(
			typeof(TUpper)
			,z.Column.UpperTypeMapper
		);
		return z;
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

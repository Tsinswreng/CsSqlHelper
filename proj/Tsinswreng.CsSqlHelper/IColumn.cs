namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// Stands for a column in a table or a property in an entity
/// </summary>
public partial interface IColumn{
	/// <summary>
	/// Column name in database table
	/// </summary>
	public str DbName { get; set; }
	/// <summary>
	/// Should be full type name like `TEXT`, `VARCHAR(64)`; only `VARCHAR` is unsupported
	/// </summary>
	public str DbType{get;set;}
	/// <summary>
	/// 有自封裝ʹ類型旹 即其內ʹ原始類型
	/// Type of the data that is retrieved from the database
	/// </summary>
	public Type? RawCodeType{get;set;}
	/// <summary>
	/// 自封裝ʹ類型
	/// Type defined in entity
	/// e.g, when you use strongly typed id struct encapsulating an int64,
	/// in this way `RawClrType` is `long` and `UpperClrType` is your custom struct
	///
	/// </summary>
	public Type? UpperCodeType{get;set;}
	/// <summary>
	/// Additional SQL statements to be executed when creating the column
	/// e.g `UNIQUE(Email)`
	/// </summary>
	public IList<str> AdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif

	/// <summary>
	/// if set true, the column will be marked as `NOT NULL` in the database
	/// </summary>
	public bool NotNull{get;set;}

	public IUpperTypeMapFn? UpperTypeMapper{get;set;}

	/// <summary>
	/// Convert from `UpperClrType` to `RawClrType`
	/// better not to be null. when use, better do like var Tar = Fn?.Invoke(Src)??Src
	/// </summary>
	public Func<obj?,obj?>? UpperToRaw{
		get{return UpperTypeMapper?.UpperToRaw;}
		set{
			UpperTypeMapper??=new UpperTypeMapFn();
			UpperTypeMapper.UpperToRaw=value;
		}
	}
#if Impl
	= (x)=>x;
#endif

	/// <summary>
	/// Convert from `RawClrType` to `UpperClrType`
	/// better not to be null. when use, better do like var Tar = Fn?.Invoke(Src)??Src
	/// </summary>
	public Func<obj?,obj?>? RawToUpper{
		get{return UpperTypeMapper?.RawToUpper;}
		set{
			UpperTypeMapper??=new UpperTypeMapFn();
			UpperTypeMapper.RawToUpper=value;
		}
	}
#if Impl
	= (x)=>x;
#endif

}

public interface IColumn<TTbl, TRaw, TUpper>:IColumn{

}

// public interface IColumn<T>:IColumn<T,T>{

// }

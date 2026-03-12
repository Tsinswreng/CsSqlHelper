namespace Tsinswreng.CsSqlHelper;

public partial interface IColumn{
	[Doc(@$"
	#Sum[Column name in database table. aka DbColName.
	Entity field name in C# code aka CodeColName.
	]
	")]
	public str DbName { get; set; }
	[Doc(@$"
	#Sum[Should be full type name like `TEXT`, `VARCHAR(64)`; only `VARCHAR` is unsupported]
	")]
	public str DbType{get;set;}
	[Doc(@$"
	#Sum[Type of the data that is retrieved from the database]
	#See[{nameof(UpperCodeType)}]
	")]
	public Type? RawCodeType{get;set;}
	/// 自封裝ʹ類型
	[Doc(@$"
	Type defined in entity class.
	e.g, when you use strongly typed id struct encapsulating an int64,
	in this way {nameof(RawCodeType)} is `long` and {nameof(UpperCodeType)} is your custom struct
	")]
	public Type? UpperCodeType{get;set;}
	[Doc(@$"Additional SQL statements to be executed when creating the column
	e.g `UNIQUE(Email)`")]
	public IList<str> AdditionalSqls{get;set;}
#if Impl
	= new List<str>();
#endif


	[Doc("if set true, the column will be marked as `NOT NULL` in the database")]
	public bool NotNull{get;set;}

	public IUpperTypeMapFn? UpperTypeMapper{get;set;}

	[Doc(@$"
	Convert from {nameof(UpperCodeType)} to {nameof(RawCodeType)}
	better not to be null. when use, better do like var Tar = Fn?.Invoke(Src)??Src
	#See[{nameof(RawToUpper)}]
	")]
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

	[Doc(@$"#See[{nameof(UpperToRaw)}]")]
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

namespace Tsinswreng.CsSqlHelper;

public interface IRawUpperTypeMapperFn{
	/// <summary>
	/// Convert from `UpperClrType` to `RawClrType`
	/// better not to be null. when use, better do like var Tar = Fn?.Invoke(Src)??Src
	/// </summary>
	public Func<obj?,obj?>? UpperToRaw{get;set;}
#if Impl
	= (x)=>x;
#endif

	/// <summary>
	/// Convert from `RawClrType` to `UpperClrType`
	/// better not to be null. when use, better do like var Tar = Fn?.Invoke(Src)??Src
	/// </summary>
	public Func<obj?,obj?>? RawToUpper{get;set;}
#if Impl
	= (x)=>x;
#endif
}

#define Impl
using Tsinswreng.CsCore;

namespace Tsinswreng.CsSql;

public class UpperTypeMapFn
	:IUpperTypeMapFn
{
	
	/// Convert from `UpperClrType` to `RawClrType`
	/// better not to be null. when use, better do like var Tar = Fn?.Invoke(Src)??Src
	

	[Impl]
	public Func<obj?,obj?>? UpperToRaw{get;set;}
#if Impl
	= (x)=>x;
#endif

	
	/// Convert from `RawClrType` to `UpperClrType`
	/// better not to be null. when use, better do like var Tar = Fn?.Invoke(Src)??Src
	

	[Impl]
	public Func<obj?,obj?>? RawToUpper{get;set;}
#if Impl
	= (x)=>x;
#endif
}

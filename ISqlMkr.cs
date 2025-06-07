namespace Tsinswreng.CsSqlHelper;

public interface ISqlMkr{
	/// <summary>
	/// 字段加引號 如Name -> "Name"或`Name`或[Name]等
	/// </summary>
	/// <param name="Name"></param>
	/// <returns></returns>
	public str Quote(str Name);
	/// <summary>
	/// 如Name -> "@Name" 等
	/// </summary>
	/// <param name="Name"></param>
	/// <returns></returns>
	public str Param(str Name);

/// <summary>
///
/// </summary>
/// <param name="Limit">參數名</param>
/// <param name="Offset">參數名</param>
/// <returns></returns>
	public str LimitOffset(str Limit, str Offset);
}

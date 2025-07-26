namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// A struct that contains an Id and a dictionary of key-value pairs.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Id"></param>
/// <param name="Dict"></param>
public  partial struct Id_Dict<T>(
	T Id
	, IDictionary<string, object?> Dict
){
	public T Id{get;set;} = Id;
	public IDictionary<string, object?> Dict{get;set;} = Dict;
}

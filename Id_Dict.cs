namespace Tsinswreng.CsSqlHelper;
public struct Id_Dict<T>(
	T Id
	, IDictionary<string, object?> Dict
){
	public T Id{get;set;} = Id;
	public IDictionary<string, object?> Dict{get;set;} = Dict;
}

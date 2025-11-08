namespace Tsinswreng.CsSqlHelper;

using TStruct = Sql;
using TPrimitive = str;

public interface ISql{
	public str RawStr{get;set;}
}

public struct Sql:ISql{
	public Sql(str RawStr){
		this.RawStr = RawStr;
	}
	public str RawStr{get;set;} = "";

	public static implicit operator TPrimitive(TStruct e){
		return e.RawStr;
	}
	public static implicit operator TStruct(TPrimitive s){
		return new TStruct(s);
	}

}

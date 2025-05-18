namespace Tsinswreng.SqlHelper;

public class SqliteFormatter{
	protected static SqliteFormatter? _Inst = null;
	public static SqliteFormatter Inst => _Inst??= new SqliteFormatter();

	public str Field(str Name){
		return "[" + Name + "]";
	}

	public str Param(str Name){
		return "@" + Name;
	}

	public str UpdateClause(IEnumerable<str> RawFields){
		List<str> segs = [];
		foreach(var rawField in RawFields){
			var field = Field(rawField);
			var param = Param(rawField);
			segs.Add(field + " = " + param);
		}
		return string.Join(", ", segs);
	}

	public str InsertClause(IEnumerable<str> RawFields){
		List<str> Fields = [];
		List<str> Params = [];
		foreach(var rawField in RawFields){
			var field = Field(rawField);
			var param = Param(rawField);
			Fields.Add(field);
			Params.Add(param);
		}
		return "(" + string.Join(", ", Fields) + ") VALUES (" + string.Join(", ", Params) + ")";
	}

}

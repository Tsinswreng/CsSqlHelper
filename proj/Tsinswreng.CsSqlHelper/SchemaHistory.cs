namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsDictMapper;


/// <summary>
/// 遷移表實體類
/// </summary>
public partial class SchemaHistory{
	public static SchemaHistory Sample = new();
	/// <summary>
	/// 主鍵。用插入旹之毫秒時間戳
	/// </summary>
	public i64 Id{get;set;} = DateTimeOffset.Now.ToUnixTimeMilliseconds();
	/// <summary>
	/// 非 被應用之時
	/// </summary>
	public i64 CreatedMs{get;set;} = DateTimeOffset.Now.ToUnixTimeMilliseconds();
	public str? Name{get;set;}
	public str? Descr{get;set;}
	public i64 ProductVersionTime{get;set;} = Version.Time;

}



[DictType(typeof(SchemaHistory))]
public partial class SqlHelperDictMapper{
	protected static SqlHelperDictMapper? _Inst = null;
	public static SqlHelperDictMapper Inst => _Inst??= new SqlHelperDictMapper();
}

public partial class SchemaHistoryTblMkr{
	public str TblName = "__TsinswrengSchemaHistory";
	public ITable MkTbl(){
		var Key_Type = SqlHelperDictMapper.Inst.GetTypeDictShallowT<SchemaHistory>();
		ITable R = Table.Mk<SchemaHistory>(SqlHelperDictMapper.Inst, TblName, Key_Type);
		R.SetCol(nameof(SchemaHistory.Id)).AdditionalSqls(["PRIMARY KEY"]);
		return R;
	}

}

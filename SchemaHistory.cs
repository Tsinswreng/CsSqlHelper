using System.Data;
using Tsinswreng.CsSrcGen.DictMapper;
using Tsinswreng.CsSrcGen.DictMapper.Attributes;
namespace Tsinswreng.CsSqlHelper;

/// <summary>
/// 遷移表實體類
/// </summary>
public class SchemaHistory{
	public static SchemaHistory Sample = new();
	/// <summary>
	/// 主鍵。用插入旹之毫秒時間戳
	/// </summary>
	public i64 Id{get;set;} = DateTimeOffset.Now.ToUnixTimeMilliseconds();
	public i64 CreatedAt{get;set;} = DateTimeOffset.Now.ToUnixTimeMilliseconds();
	public str? Name{get;set;}
	public str? Descr{get;set;}
	public i64 ProductVersionTime{get;set;} = Version.Time;
}

[DictType(typeof(SchemaHistory))]
public partial class SqlHelperDictCtx{
	protected static SqlHelperDictCtx? _Inst = null;
	public static SqlHelperDictCtx Inst => _Inst??= new SqlHelperDictCtx();
}

public class SchemaHistoryTblMkr{
	public str TblName = "__TsinswrengSchemaHistory";
	public ITable MkTbl(){
		var Key_Type = SqlHelperDictCtx.Inst.GetTypeDictT<SchemaHistory>();
		ITable R = Table.Mk(SqlHelperDictCtx.Inst, TblName, Key_Type);
		R.SetCol(nameof(SchemaHistory.Id)).AdditionalSqls(["PRIMARY KEY"]);
		return R;
	}
}

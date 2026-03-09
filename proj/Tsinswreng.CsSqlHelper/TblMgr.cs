using Tsinswreng.CsCore;

namespace Tsinswreng.CsSqlHelper;

//TODO配置忽略之字段
public partial class TblMgr:ITblMgr{
	public virtual IDbStuff DbStuff{
		get{
			if(field  is null){
				throw new Exception($"You must set {nameof(DbStuff)}");
			}
			return field;
		}
		set;
	}
	public IDictionary<Type, ITable> EntityType_Tbl{get;set;} = new Dictionary<Type, ITable>();
	public IDictionary<Type, IAggReg> AggType_Reg{get;set;} = new Dictionary<Type, IAggReg>();
}


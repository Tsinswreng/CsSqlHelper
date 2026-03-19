namespace Tsinswreng.CsSql;
/// SQL duplicator with table context and auto-binder metadata.
public interface IAutoBindSqlDuplicator: ISqlDuplicator{
	[Doc($@"Auto binders captured during SQL construction")]
	public IList<IParamAutoBinder> ParamAutoBinders { get; set; }
}

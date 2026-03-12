using Tsinswreng.CsDictMapper;

namespace Tsinswreng.CsSqlHelper.Test.Domains;

[DictType(typeof(PoAllBasicTypes))]
public partial class TestDictMapper {
	protected static TestDictMapper? _Inst = null;
	public static TestDictMapper Inst => _Inst ??= new TestDictMapper();
}

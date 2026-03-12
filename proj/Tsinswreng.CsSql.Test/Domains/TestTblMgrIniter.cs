using Tsinswreng.CsSqlHelper.Postgres;
using Tsinswreng.CsSqlHelper.Sqlite;

namespace Tsinswreng.CsSqlHelper.Test.Domains;

public static class TestTblMgrIniter {
	public static ITblMgr MkSqliteMgr() {
		var mgr = new SqliteTblMgr {};
		Init(mgr);
		return mgr;
	}

	public static ITblMgr MkPostgresMgr() {
		var mgr = new PostgresTblMgr {};
		Init(mgr);
		return mgr;
	}

	public static ITblMgr Init(ITblMgr mgr) {
		var mapper = TestDictMapper.Inst;

		var tbl = Table.FnSetTbl<PoAllBasicTypes>(mapper)("AllBasicTypes");
		tbl.Tbl.CodeIdName = nameof(PoAllBasicTypes.Id);

		// 主鍵：u8[] / blob
		tbl.Col(nameof(PoAllBasicTypes.Id))
			.Type<byte[], byte[]>("BLOB")
			.NotNull()
			.AdditionalSqls(["PRIMARY KEY"]);

		// 整數類
		tbl.Col(nameof(PoAllBasicTypes.U8Val)).Type<byte, byte>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.I8Val)).Type<sbyte, sbyte>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.U16Val)).Type<ushort, ushort>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.I16Val)).Type<short, short>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.U32Val)).Type<uint, uint>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.I32Val)).Type<int, int>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.U64Val)).Type<ulong, ulong>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.I64Val)).Type<long, long>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.I32Nullable)).Type<int?, int?>("INTEGER");
		tbl.Col(nameof(PoAllBasicTypes.I64Nullable)).Type<long?, long?>("INTEGER");

		// 浮點類
		tbl.Col(nameof(PoAllBasicTypes.F32Val)).Type<float, float>("REAL");
		tbl.Col(nameof(PoAllBasicTypes.F64Val)).Type<double, double>("REAL");
		tbl.Col(nameof(PoAllBasicTypes.F64Nullable)).Type<double?, double?>("REAL");

		// 字符串
		tbl.Col(nameof(PoAllBasicTypes.StrVal)).Type<string, string>("TEXT").NotNull();
		tbl.Col(nameof(PoAllBasicTypes.StrNullable)).Type<string?, string?>("TEXT");

		// 二進制
		tbl.Col(nameof(PoAllBasicTypes.BlobVal)).Type<byte[], byte[]>("BLOB").NotNull();
		tbl.Col(nameof(PoAllBasicTypes.BlobNullable)).Type<byte[]?, byte[]?>("BLOB");

		mgr.AddTbl(tbl);
		return mgr;
	}
}

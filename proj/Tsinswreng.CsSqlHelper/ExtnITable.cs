namespace Tsinswreng.CsSqlHelper;

using System.Linq.Expressions;
using Tsinswreng.CsCore;
using Tsinswreng.CsDictMapper;
using Tsinswreng.CsPage;
using Tsinswreng.CsTools;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;
using Str_Any = System.Collections.Generic.Dictionary<str, obj?>;
public static class ExtnITable{

	extension(ITable z){
		/// <summary>
		/// quote field name for SQL, e.g. in sqlite: "field_name"; in mysql: `field_name`;
		/// </summary>
		/// <param name="z"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public str Qt(
			str s
		){
			return z.SqlMkr.Quote(s);
		}

		/// 宜用此㕥取列 無旹自有報錯ʹ訊
		public IColumn GetCol(str CodeColName){
			if(!z.Columns.TryGetValue(CodeColName, out var Col)){
				throw new Exception($"In Table {z.DbTblName}, No such column: {CodeColName}\nAvailable columns: {string.Join(", ", z.Columns.Keys)}");
			}
			return Col;
		}


		/// <summary>
		/// 映射到數據庫表ʹ字段名
		/// </summary>
		public str ColNameToDb(
			str CodeColName
		){
			var Col = z.GetCol(CodeColName);
			var DbColName = Col.DbName;
			return DbColName;
		}

		/// <summary>
		/// 映射到數據庫表ʹ字段名 並加引號/括號
		/// </summary>
		public str Fld(
			str CodeColName
		){
			var dbColName = z.ColNameToDb(CodeColName);
			return z.SqlMkr.Quote(dbColName);
		}

		//"db_name"  不帶表名前綴
		public IField Fld<T>(Expression<Func<T, obj?>> ExprMemb){
			var memberName = ToolExpr.GetMemberName<T>(ExprMemb);
			var R = new Field(z, memberName);
			return R;
		}
		public ISqlSplicer<T> SqlSplicer<T>(){
			var R = new SqlSplicer<T>();
			R.Tbl = z;
			return R;
		}

		public ISqlSplicer SqlSplicer(){
			var R = new SqlSplicer();
			R.Tbl = z;
			return R;
		}


	/// <summary>
	/// 映射到數據庫表ʹ字段名 並加引號/括號
	/// IParam CodeColNameParam 其Name須同於數據庫字段名
	/// </summary>
		public str Fld(
			IParam CodeColNameParam
		){
			var CodeColName = CodeColNameParam.Name;
			var DbColName = z.ColNameToDb(CodeColName);
			return z.SqlMkr.Quote(DbColName);
		}

		public IParam Prm(
			str Name
		){
			return z.SqlMkr.Param(Name);
		}

		// _ + 64進制 ULID 如 "@_1ccGi7C87H-LETKfaB_JX"
		public IParam Prm(){
			//TODO 抽作獨ʹ工具
			var bytes = Ulid.NewUlid().ToByteArray();
			var id = ToolUInt128.ByteArrToUInt128(bytes);
			var Name = ToolUInt128.ToLow64Base(id);
			return z.SqlMkr.Param("_"+Name);
		}



		// [Obsolete]
		// public str PrmStr(
		//
		// 	,str Name
		// ){
		// 	return z.SqlMkr.PrmStr(Name);
		// }

		public IStr_Any ToCodeDict(
			IStr_Any DbDict
		){
			var ans = new Str_Any();
			foreach(var (kDb, vDb) in DbDict){
				//TODO 製函數㕥統一字典取值、取不到則友好ᵈ報錯並列有效ʹ鍵
				var kCode = z.DbColName_CodeColName[kDb];
				var colCode = z.Columns[kCode];
				var vCode = colCode.RawToUpper?.Invoke(vDb)??vDb;
				ans[kCode] = vCode;
			}
			return ans;
		}

		public T AssignEntity<T>(
			IStr_Any DbDict
			,T ToBeAssigned
		){
			var CodeDict = z.ToCodeDict(DbDict);
			z.DictMapper.AssignShallowT(ToBeAssigned, CodeDict);
			return ToBeAssigned;
		}

		public TPo DbDictToEntity<TPo>(
			IStr_Any DbDict
			,ref TPo R
		)where TPo:new(){
			var CodeDict = z.ToCodeDict(DbDict);
			R ??= new TPo();
			z.DictMapper.AssignShallowT(R, CodeDict);
			return R;
		}

		public TPo DbDictToEntity<TPo>(
			IStr_Any DbDict
		)where TPo:new(){
			var R = new TPo();
			z.DbDictToEntity(DbDict, ref R);
			return R;
		}

		//static i32 i = 0;

		public IStr_Any ToDbDict(
			IStr_Any CodeDict
		){
			var ans = new Str_Any();
			foreach(var (kCode, vCode) in CodeDict){
				var Col = z.Columns[kCode];
				var vDb = Col.UpperToRaw?.Invoke(vCode)??vCode;
				ans[Col.DbName] = vDb;
			}
			return ans;

		}

		public obj? UpperToRaw(
			obj? UpperValue
			,str CodeColName
		){
			var Col = z.GetCol(CodeColName);
			return Col.UpperToRaw?.Invoke(UpperValue)??UpperValue;
		}

		public obj? UpperToRaw(
			obj? UpperValue
			,Type UpperType
			,str? CodeColName = null
		){
			if(CodeColName != null
				&& (z.Columns.TryGetValue(CodeColName, out var Col))
			){
				return Col.UpperToRaw?.Invoke(UpperValue)??UpperValue;
			}
			if(z.UpperType_DfltMapper.TryGetValue(UpperType, out var Mapper)){
				return Mapper.UpperToRaw?.Invoke(UpperValue)??UpperValue;
			}
			return UpperValue;
			//throw new Exception("No UpperTypeMapperFn for type: "+ typeof(T));
		}


		public obj? UpperToRaw<T>(
			T UpperValue
			,str? CodeColName = null
		){
			return z.UpperToRaw(UpperValue, typeof(T), CodeColName);
		}

		public obj? RawToUpper(
			obj? RawValue
			,str CodeColName
		){
			var Col = z.GetCol(CodeColName);
			return Col.RawToUpper?.Invoke(RawValue)??RawValue;
		}

		// public T RawToUpper<T>(//此處T非實體類 洏是列ʹ類型
		// 	obj? RawValue
		// 	,Expression<Func<T,obj?>> ExprMemb
		// ){
		// 	var CodeColName = ToolExpr.GetMemberName(ExprMemb);
		// 	return z.RawToUpper<T>(RawValue, CodeColName);
		// }

		public T RawToUpper<T>(
			obj? RawValue
			,str CodeColName
		){
			var Col = z.GetCol(CodeColName);
			return (T)(Col.RawToUpper?.Invoke(RawValue)??RawValue)!;
		}

		public str UpdateClause(
			IEnumerable<str> UpperFields
		){
			List<str> segs = [];
			foreach(var rawField in UpperFields){
				var field = z.Fld(rawField);
				var param = z.Prm(rawField);
				segs.Add(field + " = " + param);
			}
			return string.Join(", ", segs);
		}

		public str InsertClause(
			IEnumerable<str> RawFields
		){
			List<str> Fields = [];
			List<IParam> Params = [];
			foreach(var rawField in RawFields){
				var field = z.Fld(rawField);
				var param = z.Prm(rawField);
				Fields.Add(field);
				Params.Add(param);
			}
			return "(" + string.Join(", ", Fields) + ") VALUES (" + string.Join(", ", Params) + ")";
		}

		public str InsertManyClause(
			IEnumerable<str> RawFields
			,u64 GroupCnt = 1000
		){
			IList<str> Fields = [];
			foreach(var rawField in RawFields){
				var field = z.Fld(rawField);
				var param = z.Prm(rawField);
				Fields.Add(field);
			}


			IList<IList<IParam>> ParamLists = [];
			for(u64 i = 0; i < GroupCnt; i++){
				IList<IParam> Params = [];
				foreach(var rawField in RawFields){
					var param = z.NumFieldParam(rawField, i);
					Params.Add(param);
				}
				ParamLists.Add(Params);
			}
			var sqlFieldsValus = "(" + string.Join(", ", Fields) + ") VALUES";

			List<str> R = [];
			//R.Add(sqlFieldsValus);
			for(u64 i = 0; i < GroupCnt; i++){
				var ParamList = ParamLists[i.AsI32()];
				R.Add(" (" + string.Join(", ", ParamList) + ")");
			}

			return sqlFieldsValus+" "+string.Join(", ", R);
		}

		public IParam NumFieldParam(
			str Field, u64 Num
		){
			return z.Prm(Field+"__"+Num);
		}

		/// Num:0 -> @_0
		public IParam NumParam(
			u64 Num
		){
			return z.Prm("_"+Num);
		}


		/// (EndPos: 2, StartPos: 0) -> [@_0, @_1, @_2]
		/// StartPos<=x<=EndPos
		public IList<IParam> NumParamsEndStart(
			u64 EndPos, u64 StartPos = 0
		){
			var R = new List<IParam>();
			for(u64 i = StartPos; i <= EndPos; i++){
				var Param = z.NumParam(i);
				R.Add(Param);
			}
			return R;
		}

		/// (StartPos: 0, EndPos: 2) -> [@_0, @_1, @_2]
		/// StartPos<=x<=EndPos
		public IList<IParam> NumParams(
			u64 StartPos, u64 EndPos
		){
			var R = new List<IParam>();
			for(u64 i = StartPos; i <= EndPos; i++){
				var Param = z.NumParam(i);
				R.Add(Param);
			}
			return R;
		}

		public IList<IParam> NumParams(
			u64 Cnt
		){
			return z.NumParams(0, Cnt-1);
		}


		/// (@0, @1, @2 ...)
		public str NumParamClause(
			u64 EndPos
			,u64 StartPos = 0
		){
			List<str> R = [];
			R.Add("(");
			for(u64 i = StartPos; i <= EndPos; i++){
				var Param = z.Prm(i+"").ToString()??"";
				R.Add(Param);
				if(i == EndPos){
					R.Add(", ");
				}
			}
			R.Add(")");
			return string.Join("", R);
		}


	/// <summary>
	/// [@0, @1, @2 ...]
	/// ,</summary>
	/// <param name="z"></param>
	/// <param name="Start">含</param>
	/// <param name="End">含</param>
	/// <returns></returns>
		public IList<IParam> Prm(
			u64 Start
			,u64 End
		){
			var R = new List<IParam>();
			for(u64 i = Start; i <= End; i++){
				var Param = z.Prm(i+"");
				R.Add(Param);
			}
			return R;
		}


		public str Eq(
			str DbColName, IParam Param
		){
			return z.SqlMkr.Eq(DbColName, Param);
		}

		public str Eq(IParam Param){
			return z.Eq(Param.Name, Param);
		}


	/// <summary>
	/// [@0, @1, @2 ...]
	/// ,</summary>
	/// <param name="z"></param>
	/// <param name="Start">含</param>
	/// <param name="End">含</param>
	/// <returns></returns>
		// [Obsolete]
		// public IList<str> PrmStrArr(
		//
		// 	,u64 Start
		// 	,u64 End
		// ){
		// 	var R = new List<str>();
		// 	for(u64 i = Start; i <= End; i++){
		// 		var Param = z.PrmStr(i+"");
		// 		R.Add(Param);
		// 	}
		// 	return R;
		// }

		public str SqlMkTbl(

		){
			str OneCol(ITable Tbl, IColumn Col){
				var R = new List<str>();
				R.Add(Tbl.Qt(Col.DbName));
				if(!str.IsNullOrEmpty(Col.DbType)){
					R.Add(Col.DbType);
				}else{
					if(Col.RawCodeType == null){
						var Msg = $"{Col.DbName}:\nCol.RawClrType == null";
						throw new Exception("Col.RawClrType == null");
					}
					try{
						var DbTypeName = z.SqlMkr.SqlTypeMapper.ToDbTypeName(Col.RawCodeType);
						R.Add(DbTypeName);
					}
					catch (System.Exception e){
						throw new Exception($"Type Mapping Error:\n{Tbl.DbTblName}.{Col.DbName}\n{e}");
					}
				}
				R.AddRange(Col.AdditionalSqls??[]);
				if(Col.NotNull){
					R.Add("NOT NULL");
				}
				return string.Join(" ", R);
			}

			str FmtInnerSqls(IList<str>? Sqls){
				if(Sqls == null || Sqls.Count == 0){
					return "";
				}
				return $",\n\n{str.Join(",\n\t", z.InnerAdditionalSqls??[])}";
			}

			str FmtOuterSqls(IList<str>? Sqls){
				if(Sqls == null || Sqls.Count == 0){
					return "";
				}
				List<str> R = [];
				foreach(var sql in Sqls){
					R.Add(sql);
					R.Add(";\n");
				}
				return str.Join("",R);
			}

			var Lines = new List<str>();
			foreach(var (name, Col) in z.Columns){
				Lines.Add(OneCol(z, Col));
			}
	var S =
	$"""
	CREATE TABLE IF NOT EXISTS {z.Qt(z.DbTblName)}(
		{string.Join(",\n\t", Lines)}{FmtInnerSqls(z.InnerAdditionalSqls)}
	);
	{FmtOuterSqls(z.OuterAdditionalSqls)}
	""";
			return S;
		}


		public IPageQry PageSlctAll(){
			var R = new PageQry();
			if(z.TblMgr?.DbSrcType == ConstDbSrcType.Postgres){
				R.PageSize = i64.MaxValue;
			}else{
				R.PageSize = u64.MaxValue;
			}
			return R;
		}
	}
}

#if DEBUG
public class Dbg{
	public static i64 Cnt = 0;
}
#endif

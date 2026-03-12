using System.Linq.Expressions;
using System.Collections;
using Tsinswreng.CsTools;
namespace Tsinswreng.CsSql;

public class ISqlSplicer<E>: IAutoBindSqlDuplicator{
	public ITable Tbl{get;set;}
	public IList<obj> Segs{get;set;} = [];
	public IList<IParamAutoBinder> ParamAutoBinders { get; set; } = [];


	//變 實體
	public ISqlSplicer<T2> T<T2>(ITable<T2> Tbl2){
		return new SqlSplicer<T2>(){Tbl=Tbl2};
	}

	public ISqlSplicer<T2> T<T2>(ITable Tbl2){
		return new SqlSplicer<T2>(){Tbl=Tbl2};
	}

	ISqlSplicer<E> AddSeg(str Seg){
		Seg = " "+Seg+" ";
		Segs.Add(Seg);
		return this;
	}

	ISqlSplicer<E> AddSeg(IParam P){
		Segs.Add(" ");
		Segs.Add(P);
		Segs.Add(" ");
		return this;
	}

	/// s -> "s"
	str Qt(str s){
		return Tbl.Qt(s);
	}

	// CodeCol -> "DbCol"
	str QtCol(str s){
		return Tbl.QtCol(s);
		//return Tbl.Qt(Tbl.DbTblName+"."+Tbl.ColNameToDb(s));
	}
	// CodeCol -> "Tbl"."DbCol"
	str QtTblCol(str s){
		return Tbl.Qt(Tbl.DbTblName)+"."+QtCol(s);
	}

	/// p -> @p
	IParam Prm(str s){
		//TODO 若褈則改名併返改後者
		return Tbl.Prm(s);
	}

	/// x=>x.Memb -> Memb (string)
	str Memb<T2>(Expression<Func<T2, obj?>> Memb){
		return ToolExpr.GetMemberName(Memb);
	}

	/// x=>x.Memb -> Tbl.Memb
	str TblWithMemb<T2>(Expression<Func<T2, obj?>> ExprMemb){
		return Tbl.DbTblName+"."+Memb(ExprMemb);
	}

	/// x=>x.Memb -> "Tbl"."Memb"
	str QtTblWithMemb<T2>(Expression<Func<T2, obj?>> ExprMemb){
		//Tbl.Qt(TblWithMemb(ExprMemb)); 此謬。如"Tbl.Field"實示 完整ʹ字段。璫用"Tbl"."Field"
		return Tbl.Qt(Tbl.DbTblName)+"."+Tbl.Qt(Memb(ExprMemb));
	}

	public ISqlSplicer<E> Raw(str Raw){
		return AddSeg(Raw);
	}
	public ISqlSplicer<E> Select(str Raw){
		return AddSeg($"SELECT {Raw}");
	}

	public ISqlSplicer<E> Select(Expression<Func<E, obj?>> GetMember){
		var memb = Memb(GetMember);
		//return AddSeg($"SELECT {Qt(Tbl.DbTblName+"."+Tbl.ColNameToDb(memb))} AS {memb}");
		var seg = "SELECT"
		+Qt(Tbl.DbTblName)
		+"."
		+Qt(Tbl.DbCol(memb))
		+" AS "
		+Qt(memb);
		return AddSeg(seg);
	}

	public ISqlSplicer<E> From(){
		AddSeg($"FROM {Qt(Tbl.DbTblName)}");
		return this;
	}

	public ISqlSplicer<E> From(str Raw){
		AddSeg($"FROM {Raw}");
		return this;
	}

	public ISqlSplicer<E> Where1(){
		return AddSeg($"WHERE 1=1");
	}
	public ISqlSplicer<E> Where(str Raw){
		return AddSeg($"WHERE {Raw}");
	}

	public ISqlSplicer<E> And(){
		return AddSeg("AND");

	}

	public ISqlSplicer<E> Or(){
		return AddSeg("OR");
	}

	public ISqlSplicer<E> And(str Raw){
		return AddSeg($"AND {Raw}");
	}
	public ISqlSplicer<E> Or(str Raw){
		return AddSeg($"OR {Raw}");
	}
	public ISqlSplicer<E> Not(str Raw){
		return AddSeg($"NOT ({Raw})");

	}

	public ISqlSplicer<E> Paren(str Raw){
		return AddSeg($"({Raw})");
	}

	public ISqlSplicer<E> Paren(Func<ISqlSplicer<E>, obj?> FnBlock){
		AddSeg($"\n(\n");
		FnBlock(this);
		AddSeg($"\n)\n");
		return this;
	}


	public ISqlSplicer<E> Bool(Expression<Func<E, obj?>> GetMember, str Op, IParam Param){
		var memb = Memb(GetMember);
		return AddSeg(QtCol(memb)).AddSeg(Op).AddSeg(Param);
	}
	public ISqlSplicer<E> Bool(str Memb, str Op, out IParam Param){
		Param = Prm(Memb);
		return AddSeg(QtTblCol(Memb)).AddSeg(Op).AddSeg(Param);
	}
	public ISqlSplicer<E> Bool(Expression<Func<E, obj?>> GetMember, str Op, out IParam Param){
		var memb = Memb(GetMember);
		Param = Prm(memb);
		return Bool(GetMember, Op, Param);
	}


	public ISqlSplicer<E> Bool(
		str BoolOp
		,Expression<Func<E, obj?>> GetMember, str Op, out IParam Param
	){
		AddSeg(BoolOp);
		return Bool(GetMember, Op, out Param);
	}
	public ISqlSplicer<E> And(Expression<Func<E, obj?>> GetMember, str Op, out IParam Param){
		Bool("AND", GetMember, Op, out Param);
		return this;
	}

	public ISqlSplicer<E> AndEq(Expression<Func<E, obj?>> GetMember, out IParam Param){
		return And(GetMember, "=", out Param);
	}

	public ISqlSplicer<E> AndEq(
		Expression<Func<E, obj?>> GetMember,
		Func<SqlArgBinderFactory, IParamAutoBinder> Bind
	){
		var memb = Memb(GetMember);
		AndEq(GetMember, out var param);
		var binder = Bind(new SqlArgBinderFactory(param, Tbl, memb));
		ParamAutoBinders.Add(binder);
		return this;
	}
	
	public ISqlSplicer<E> AndEq(
		string CodeCol,
		Func<SqlArgBinderFactory, IParamAutoBinder> Bind
	){
		var r = And().Bool(CodeCol, "=", out var param);
		var binder = Bind(new SqlArgBinderFactory(param, Tbl, CodeCol));
		ParamAutoBinders.Add(binder);
		return r;
	}



	public ISqlSplicer<E> OrderBy(str Raw){
		AddSeg($"ORDER BY {Raw}");
		return this;
	}

	public ISqlSplicer<E> OrderByDesc(str Raw){
		AddSeg($"ORDER BY {Raw} DESC");
		return this;
	}
	public ISqlSplicer<E> OrderByDesc(Expression<Func<E, obj?>> GetMember){
		OrderByDesc(QtCol(Memb(GetMember)));
		return this;
	}
	public ISqlSplicer<E> LimOfst(out IParam Lim, out IParam Ofst){
		var seg = Tbl.SqlMkr.ParamLimOfst(out Lim, out Ofst);
		AddSeg(seg);
		return this;
	}

	public ISqlSplicer<E> Set(){
		return AddSeg("SET");
	}


	public ISqlSplicer<E> Eq(str Left, str Right){
		return AddSeg(Left).AddSeg("=").AddSeg(Right);
	}

	public ISqlSplicer<E> Eq(Expression<Func<E, obj?>> ExprMemb, str Right){
		return Eq(QtTblWithMemb(ExprMemb), Right);
	}

	public ISqlSplicer<E> Eq(Expression<Func<E, obj?>> ExprMemb, IParam Right){
		return AddSeg(QtTblWithMemb(ExprMemb)).AddSeg("=").AddSeg(Right);
	}

	public ISqlSplicer<E> Eq(Expression<Func<E, obj?>> ExprMemb, out IParam Right){
		var memb = Memb(ExprMemb);
		Right = Prm(memb);
		return AddSeg(QtTblWithMemb(ExprMemb)).AddSeg("=").AddSeg(Right);
	}

	public ISqlSplicer<E> UpdateSet(){
		return AddSeg($"UPDATE {Qt(Tbl.DbTblName)} SET");
	}

	public ISqlSplicer<E> With(str Raw){
		return AddSeg("WITH").AddSeg(Raw);
	}
	public ISqlSplicer<E> As(){
		return AddSeg("AS");
	}

	public ISqlSplicer<E> C(){
		return AddSeg(", ");
	}
	public ISqlSplicer<E> PL(){
		return AddSeg("(");
	}

	public ISqlSplicer<E> PR(){
		return AddSeg(")");
	}

	[Impl]
	[Doc($@"
#Sum[Generate repeated SQL statements]
#Params([Repeat count])
#Rtn[String containing multiple SQL statements, each ending with a semicolon]
#See([{nameof(ToSqlStrAtOfst)}],[{nameof(IParam.ToOfst)}])
")]
	public str DuplicateSql(u64 Cnt){
		var R = new List<str>();
		for(u64 i=0;i<Cnt;i++){
			R.Add(ToSqlStrAtOfst(i));
			R.Add(";");
		}
		return string.Join("", R);
	}

	[Impl]
	[Doc($@"
#Sum[Convert SQL segments to SQL string with offset]
#Params([Parameter offset])
#Rtn[Complete SQL string where all parameters are adjusted to the specified offset]
#See([{nameof(DuplicateSql)}],[{nameof(IParam.ToOfst)}])
")]
	public str ToSqlStrAtOfst(u64 Ofst){
		var L = new List<str>();
		foreach(var seg in Segs){
			if(seg is IParam p){
				L.Add(p.ToOfst(Ofst)+"");
			}else if(seg is str s){
				L.Add(s);
			}
		}
		return string.Join("", L);
	}


	public str ToSqlStr(u64 RepeatCnt = 1){
		return DuplicateSql(RepeatCnt);
	}

	// public str ToSqlStr(IDbFnCtx Ctx){
	// 	return DuplicateSql(Ctx.BatchSize);
	// }

}
public class ISqlSplicer: ISqlSplicer<obj>{}
public class SqlSplicer<T>:ISqlSplicer<T>{}

public class SqlSplicer:ISqlSplicer{

}

/// SQL duplicator with table context and auto-binder metadata.
public interface IAutoBindSqlDuplicator: ISqlDuplicator{
	[Doc($@"Auto binders captured during SQL construction")]
	public IList<IParamAutoBinder> ParamAutoBinders { get; set; }
}

/// Bind values into <see cref="IArgDict"/> for one execution.
public interface IParamAutoBinder{
	[Doc(@$"
#Sum[Bind values into argument dictionary for current execution]
#Params([Argument dictionary],[Current batch items])
#Rtn[Void]
")]
	public void Bind(IArgDict Args, IList Items);
}

/// Binder for "Many(values)" that can stream values by batch size.
public interface IParamAutoBinderMulti: IParamAutoBinder{
	[Doc(@$"
#Sum[Take next arguments batch from internal sequence]
#Params([Expected batch size],[Output batch list])
#Rtn[True if at least one value is available]
")]
	public bool TryTakeBatchArgs(u64 BatchSize, out IList Args);
	[Doc(@$"
#Sum[Bind a taken batch into arguments]
#Params([Argument dictionary],[Batch values])
#Rtn[Void]
")]
	public void BindBatch(IArgDict Args, IList Batch);
}


/// Factory that creates auto-binders bound to one SQL parameter.

public class SqlArgBinderFactory{
	public IParam Param { get; set; }
	public ITable? Tbl{get;set;}
	//TODO 處理CodeColo
	public str? CodeCol{get;set;}
	public SqlArgBinderFactory(
		IParam Param
		,ITable? Tbl=null
		,str? CodeCol = null
	){
		this.Param = Param;
		this.Tbl = Tbl;
		this.CodeCol = CodeCol;
	}
	[Doc(@$"
#Sum[Create binder for one fixed value]
#Params([Value bound to parameter])
#TParams([Value type])
#Rtn[Auto binder instance]
")]
	public IParamAutoBinder One<TVal>(TVal Value){
		return new ParamAutoBinderOne<TVal>(Param, Value){Tbl=Tbl};
	}
	[Doc(@$"
#Sum[Create binder for a value sequence]
#Params([Sequence to bind as numbered parameters])
#TParams([Element type])
#Rtn[Auto binder instance]
")]
	public IParamAutoBinder Many<TVal>(IEnumerable<TVal> Values){
		return new ParamAutoBinderManyValues<TVal>(Param, Values){Tbl=Tbl};
	}

}


/// Binder for one fixed value.
public class ParamAutoBinderOne<TVal>: IParamAutoBinder{
	public IParam Param { get; set; }
	public TVal Value { get; set; }
	public ITable? Tbl { get; set; }

	public ParamAutoBinderOne(IParam Param, TVal Value){
		this.Param = Param;
		this.Value = Value;
	}

	[Doc(@$"
#Sum[Bind one fixed value]
#Params([Argument dictionary],[Ignored batch items])
#Rtn[Void]
")]
	public void Bind(IArgDict Args, IList Items){
		if(Tbl != null){
			Args.AddRaw(Param, Tbl.UpperToRaw(Value));
			return;
		}
		Args.AddRaw(Param, Value);
	}
}

/// Binder for prebuilt value sequence; supports incremental batch consumption.
public class ParamAutoBinderManyValues<TVal>: IParamAutoBinderMulti{
	[Doc(@$"Declared Parameter")]
	public IParam Param { get; set; }
	[Doc(@$"Received Arguments")]
	public IEnumerable<TVal> Args { get; set; }
	protected IEnumerator<TVal> ArgsItor{
		get{
			field ??= Args.GetEnumerator();
			return field;
		}
	}
	public ITable? Tbl { get; set; }
	
	
	public ParamAutoBinderManyValues(IParam Param, IEnumerable<TVal> Args){
		this.Param = Param;
		this.Args = Args;
	}

	[Doc(@$"
#Sum[Bind all values in sequence]
#Params([Argument dictionary],[Ignored batch items])
#Rtn[Void]
")]
	public void Bind(IArgDict Args, IList Items){
		foreach(var (i, value) in this.Args.Index()){
			var p = Param.ToOfst((u64)i);
			if(Tbl != null){
				Args.AddRaw(p, Tbl.UpperToRaw(value));
			}else{
				Args.AddRaw(p, value);
			}
		}
	}



	[Doc(@$"
#Sum[Take next N values from sequence]
#Params([Maximum items to take],[Taken values])
#Rtn[True when at least one value is taken]
")]
	public bool TryTakeBatchArgs(u64 BatchSize, out IList Batch){
		var args = new List<TVal>();
		var argsItor = ArgsItor;
		// Pull values lazily; do not materialize full source sequence.
		for(u64 i = 0; i < BatchSize; i++){
			if(!argsItor.MoveNext()){
				break;
			}
			args.Add(argsItor.Current);
		}
		Batch = args;
		return args.Count > 0;
	}

	[Doc(@$"
#Sum[Bind a pre-taken value batch]
#Params([Argument dictionary],[Batch values])
#Rtn[Void]
#Throw[{nameof(InvalidCastException)}][When batch element type does not match {nameof(TVal)}]
")]
	public void BindBatch(IArgDict Args, IList Batch){
		var list = new List<TVal>(Batch.Count);
		foreach(var item in Batch){
			if(item is not TVal typed){
				throw new InvalidCastException($"Expected batch item type {typeof(TVal).Name}, got {item?.GetType().Name ?? "null"}.");
			}
			list.Add(typed);
		}
		foreach(var (i, value) in list.Index()){
			var p = Param.ToOfst((u64)i);
			if(Tbl != null){
				Args.AddRaw(p, Tbl.UpperToRaw(value));
			}else{
				Args.AddRaw(p, value);
			}
		}
	}
}

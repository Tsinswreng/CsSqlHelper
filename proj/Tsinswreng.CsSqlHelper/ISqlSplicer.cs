using System.Linq.Expressions;
using System.Collections;
using Tsinswreng.CsTools;
namespace Tsinswreng.CsSqlHelper;

public class ISqlSplicer<E>:ISqlDuplicator{
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
	str Fld(str s){
		return Tbl.QtCol(s);
		//return Tbl.Qt(Tbl.DbTblName+"."+Tbl.ColNameToDb(s));
	}

	/// p -> @p
	IParam Prm(str s){
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
		return AddSeg(Fld(memb)).AddSeg(Op).AddSeg(Param);
	}

	public ISqlSplicer<E> Bool(Expression<Func<E, obj?>> GetMember, str Op, out IParam Param){
		var memb = Memb(GetMember);
		Param = Prm(memb);//TODO 維護dict參數表㕥判褈名
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
		AndEq(GetMember, out var param);
		var binder = Bind(new SqlArgBinderFactory(param));
		ParamAutoBinders.Add(binder);
		return this;
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
		OrderByDesc(Fld(Memb(GetMember)));
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




public interface IParamAutoBinder{
	public void Bind(ITable Tbl, IArgDict Args, IList Items);
}

public interface IParamAutoBinderManyValuesBatch: IParamAutoBinder{
	public bool TryTakeBatch(u64 BatchSize, out IList Batch);
	public void BindBatch(ITable Tbl, IArgDict Args, IList Batch);
}

public class SqlArgBinderFactory{
	public IParam Param { get; set; }
	public SqlArgBinderFactory(IParam Param){
		this.Param = Param;
	}
	public IParamAutoBinder One<TVal>(TVal Value){
		return new ParamAutoBinderOne<TVal>(Param, Value);
	}
	public IParamAutoBinder Many<TVal>(IEnumerable<TVal> Values){
		return new ParamAutoBinderManyValues<TVal>(Param, Values);
	}
	public IParamAutoBinder Many<TItem, TVal>(Func<TItem, TVal> Selector){
		return new ParamAutoBinderMany<TItem, TVal>(Param, Selector);
	}
}

public class ParamAutoBinderOne<TVal>: IParamAutoBinder{
	public IParam Param { get; set; }
	public TVal Value { get; set; }

	public ParamAutoBinderOne(IParam Param, TVal Value){
		this.Param = Param;
		this.Value = Value;
	}

	public void Bind(ITable Tbl, IArgDict Args, IList Items){
		Args.AddT(Param, Value);
	}
}

public class ParamAutoBinderMany<TItem, TVal>: IParamAutoBinder{
	public IParam Param { get; set; }
	public Func<TItem, TVal> Selector { get; set; }

	public ParamAutoBinderMany(IParam Param, Func<TItem, TVal> Selector){
		this.Param = Param;
		this.Selector = Selector;
	}

	public void Bind(ITable Tbl, IArgDict Args, IList Items){
		var vals = new List<TVal>();
		foreach(var item in Items){
			if(item is not TItem typed){
				throw new InvalidCastException($"Expected batch item type {typeof(TItem).Name}, got {item?.GetType().Name ?? "null"}.");
			}
			vals.Add(Selector(typed));
		}
		Args.AddManyT(Param, vals);
	}
}

public class ParamAutoBinderManyValues<TVal>: IParamAutoBinderManyValuesBatch{
	public IParam Param { get; set; }
	public IEnumerable<TVal> Values { get; set; }
	protected IEnumerator<TVal>? ValueEnum { get; set; }

	public ParamAutoBinderManyValues(IParam Param, IEnumerable<TVal> Values){
		this.Param = Param;
		this.Values = Values;
	}

	public void Bind(ITable Tbl, IArgDict Args, IList Items){
		Args.AddManyT(Param, Values);
	}

	protected IEnumerator<TVal> GetOrMkEnum(){
		ValueEnum ??= Values.GetEnumerator();
		return ValueEnum;
	}

	public bool TryTakeBatch(u64 BatchSize, out IList Batch){
		var list = new List<TVal>();
		var e = GetOrMkEnum();
		for(u64 i = 0; i < BatchSize; i++){
			if(!e.MoveNext()){
				break;
			}
			list.Add(e.Current);
		}
		Batch = list;
		return list.Count > 0;
	}

	public void BindBatch(ITable Tbl, IArgDict Args, IList Batch){
		var list = new List<TVal>(Batch.Count);
		foreach(var item in Batch){
			if(item is not TVal typed){
				throw new InvalidCastException($"Expected batch item type {typeof(TVal).Name}, got {item?.GetType().Name ?? "null"}.");
			}
			list.Add(typed);
		}
		Args.AddManyT(Param, list);
	}
}

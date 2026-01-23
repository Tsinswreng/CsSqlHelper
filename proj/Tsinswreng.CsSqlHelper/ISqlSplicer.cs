using System.Linq.Expressions;
using Tsinswreng.CsTools;
namespace Tsinswreng.CsSqlHelper;


public class ISqlSplicer<E>{
	public ITable Tbl{get;set;}
	public IList<str> Segs{get;set;} = [];

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

	/// s -> "s"
	str Qt(str s){
		return Tbl.Qt(s);
	}

	// CodeCol -> "DbCol"
	str Fld(str s){
		return Tbl.Fld(s);
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
		return AddSeg($"SELECT {Qt(Tbl.DbTblName+"."+Tbl.ColNameToDb(memb))} AS {memb}");
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
		AddSeg($"{Fld(memb)} {Op} {Param}");
		return this;
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
		return Eq(QtTblWithMemb(ExprMemb), Right.ToString()??"");
	}

	public ISqlSplicer<E> Eq(Expression<Func<E, obj?>> ExprMemb, out IParam Right){
		var memb = Memb(ExprMemb);
		Right = Prm(memb);
		return Eq(QtTblWithMemb(ExprMemb), Right.ToString()??"");
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


	public str ToSqlStr(){
		return string.Join("\n", Segs);
	}
}
public class ISqlSplicer: ISqlSplicer<obj>{}
public class SqlSplicer<T>:ISqlSplicer<T>{}

public class SqlSplicer:ISqlSplicer{

}

using System.Linq.Expressions;
using Tsinswreng.CsTools;
namespace Tsinswreng.CsSqlHelper;

public class ISqlSplicer<T>{
	public ITable<T> Tbl{get;set;}
	public IList<str> Segs{get;set;} = [];
	ISqlSplicer<T> AddSeg(str Seg){
		Seg = " "+Seg+" ";
		Segs.Add(Seg);
		return this;
	}
	str Qt(str s){
		return Tbl.Qt(s);
	}
	str Fld(str s){
		//return Tbl.Fld(s);
		return Tbl.Qt(Tbl.DbTblName+"."+Tbl.ColNameToDb(s));
	}
	IParam Prm(str s){
		return Tbl.Prm(s);
	}
	str Memb<T2>(Expression<Func<T2, obj?>> Memb){
		return ToolExpr.GetMemberName(Memb);
	}
	str TblWithMemb<T2>(Expression<Func<T2, obj?>> ExprMemb){
		return Tbl.DbTblName+"."+Memb(ExprMemb);
	}
	str QtTblWithMemb<T2>(Expression<Func<T2, obj?>> ExprMemb){
		return Tbl.Qt(TblWithMemb(ExprMemb));
	}

	public ISqlSplicer<T> Raw(str Raw){
		return AddSeg(Raw);
	}
	public ISqlSplicer<T> Select(str Raw){
		return AddSeg($"SELECT {Raw}");
	}

	public ISqlSplicer<T> Select(Expression<Func<T, obj?>> GetMember){
		var memb = Memb(GetMember);
		return AddSeg($"SELECT {Qt(Tbl.DbTblName+"."+Tbl.ColNameToDb(memb))} AS {memb}");
	}

	public ISqlSplicer<T> From(){
		AddSeg($"FROM {Qt(Tbl.DbTblName)}");
		return this;
	}
	public ISqlSplicer<T> WhereT(){
		return AddSeg($"WHERE 1=1");
	}
	public ISqlSplicer<T> Where(str Raw){
		return AddSeg($"WHERE {Raw}");
	}

	public ISqlSplicer<T> And(){
		return AddSeg("AND");

	}

	public ISqlSplicer<T> Or(){
		return AddSeg("OR");
	}

	public ISqlSplicer<T> And(str Raw){
		return AddSeg($"AND {Raw}");
	}
	public ISqlSplicer<T> Or(str Raw){
		return AddSeg($"OR {Raw}");
	}
	public ISqlSplicer<T> Not(str Raw){
		return AddSeg($"NOT ({Raw})");

	}

	public ISqlSplicer<T> Paren(str Raw){
		return AddSeg($"({Raw})");
	}

	public ISqlSplicer<T> Paren(Func<ISqlSplicer<T>, obj?> FnBlock){
		AddSeg($"\n(\n");
		FnBlock(this);
		AddSeg($"\n)\n");
		return this;
	}


	public ISqlSplicer<T> Bool(Expression<Func<T, obj?>> GetMember, str Op, IParam Param){
		if(Dbg.Cnt == 18){
			System.Console.WriteLine();
		}
		var memb = Memb(GetMember);
		AddSeg($"{Fld(memb)} {Op} {Param}");
		return this;
	}

	public ISqlSplicer<T> Bool(Expression<Func<T, obj?>> GetMember, str Op, out IParam Param){
		var memb = Memb(GetMember);
		Param = Prm(memb);//TODO 維護dict參數表㕥判褈名
		return Bool(GetMember, Op, Param);
	}



	public ISqlSplicer<T> BoolOp(
		str BoolOp
		,Expression<Func<T, obj?>> GetMember, str Op, out IParam Param
	){
		AddSeg(BoolOp);
		return Bool(GetMember, Op, out Param);
	}
	public ISqlSplicer<T> And(Expression<Func<T, obj?>> GetMember, str Op, out IParam Param){
		BoolOp("AND", GetMember, Op, out Param);
		return this;
	}

	public ISqlSplicer<T> AndEq(Expression<Func<T, obj?>> GetMember, out IParam Param){
		return And(GetMember, "=", out Param);
	}


	public ISqlSplicer<T> OrderBy(str Raw){
		AddSeg($"ORDER BY {Raw}");
		return this;
	}

	public ISqlSplicer<T> OrderByDesc(str Raw){
		AddSeg($"ORDER BY {Raw} DESC");
		return this;
	}
	public ISqlSplicer<T> OrderByDesc(Expression<Func<T, obj?>> GetMember){
		OrderByDesc(Fld(Memb(GetMember)));
		return this;
	}
	public ISqlSplicer<T> LimOfst(out IParam Lim, out IParam Ofst){
		var seg = Tbl.SqlMkr.ParamLimOfst(out Lim, out Ofst);
		AddSeg(seg);
		return this;
	}

	public ISqlSplicer<T> Set(){
		return AddSeg("SET");
	}


	public ISqlSplicer<T> Eq(str Left, str Right){
		return AddSeg(Left).AddSeg("=").AddSeg(Right);
	}

	public ISqlSplicer<T> Eq(Expression<Func<T, obj?>> ExprMemb, str Right){
		return Eq(QtTblWithMemb(ExprMemb), Right);
	}

	public ISqlSplicer<T> Eq(Expression<Func<T, obj?>> ExprMemb, IParam Right){
		return Eq(QtTblWithMemb(ExprMemb), Right.ToString()??"");
	}

	public ISqlSplicer<T> Eq(Expression<Func<T, obj?>> ExprMemb, out IParam Right){
		var memb = Memb(ExprMemb);
		Right = Prm(memb);
		return Eq(QtTblWithMemb(ExprMemb), Right.ToString()??"");
	}

	public ISqlSplicer<T> UpdateSet(){
		return AddSeg($"UPDATE {Qt(Tbl.DbTblName)} SET");
	}

	public ISqlSplicer<T> C(){
		return AddSeg(", ");
	}
	public ISqlSplicer<T> PL(){
		return AddSeg("(");
	}

	public ISqlSplicer<T> PR(){
		return AddSeg(")");
	}


	public str ToSqlStr(){
		return string.Join("\n", Segs);
	}
}

public class SqlSplicer<T>:ISqlSplicer<T>{}


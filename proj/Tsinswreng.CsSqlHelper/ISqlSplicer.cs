using System.Linq.Expressions;
using Tsinswreng.CsTools;
namespace Tsinswreng.CsSqlHelper;
public class ISqlSplicer<T>{
	public ITable<T> Tbl{get;set;}
	public IList<str> Segs{get;set;} = [];
	void AddSeg(str Seg){
		Seg = " "+Seg+" ";
		Segs.Add(Seg);
	}
	str Qt(str s){
		return Tbl.Qt(s);
	}
	str Fld(str s){
		return Tbl.Fld(s);
	}
	IParam Prm(str s){
		return Tbl.Prm(s);
	}
	str Memb<T2>(Expression<Func<T2, obj?>> Memb){
		return Tbl.Fld(ToolExpr.GetMemberName(Memb));
	}
	public ISqlSplicer<T> Raw(str Raw){
		AddSeg(Raw);
		return this;
	}
	public ISqlSplicer<T> Select(str Raw){
		AddSeg($"SELECT {Raw}");
		return this;
	}
	public ISqlSplicer<T> From(){
		AddSeg($"FROM {Qt(Tbl.DbTblName)}");
		return this;
	}
	public ISqlSplicer<T> Where(){
		AddSeg($"WHERE 1=1");
		return this;
	}
	public ISqlSplicer<T> Where(str Raw){
		AddSeg($"WHERE {Raw}");
		return this;
	}
	public ISqlSplicer<T> And(str Raw){
		AddSeg($"AND {Raw}");
		return this;
	}
	ISqlSplicer<T> BoolOp(
		str BoolOp
		,Expression<Func<T, obj?>> GetMember, str Op, out IParam Param
	){
		var memb = Memb(GetMember);
		Param = Prm(memb);//TODO 維護dict參數表㕥判褈名
		AddSeg($"{BoolOp} {Fld(memb)} {Op} {Param}");
		return this;
	}
	public ISqlSplicer<T> And(Expression<Func<T, obj?>> GetMember, str Op, out IParam Param){
		BoolOp("AND", GetMember, Op, out Param);
		return this;
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

	public str ToSqlStr(){
		return string.Join("\n", Segs);
	}
}

public class SqlSplicer<T>:ISqlSplicer<T>{}


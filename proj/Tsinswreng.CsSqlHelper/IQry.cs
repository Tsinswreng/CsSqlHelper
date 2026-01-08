#if false
public class IQry<T>{
	public ITable<T> Tbl;

	public IQry<T> LeftJoin<T2>(
		ITable<T2> Tbl2
		,Expression<Func<T,T2, obj?>> GetMember
	){

	}

	public IQry<T> Select(Expression<Func<T, obj?>> GetMember){

	}

	public IQry<T> Select(Expression<Func<T, obj?>> GetMember, str Alias){

	}

	public IQry<T> And(Expression<Func<T, obj?>> GetMember, str Right){

	}
	public IQry<T> And(Expression<Func<T, obj?>> GetMember, str Op, out IParam Param){

	}

	public IQry<T> Or(Expression<Func<T, obj?>> GetMember, str Op, IParam Param){

	}

	public IQry<T> Or(Expression<Func<T, obj?>> GetMember, str Op, out IParam Param){

	}


	public IQry<T> AndBlock(Func<IQry<T>, obj?> GetMember){

	}

	public IQry<T> OrderBy(Expression<Func<T, obj?>> GetMember){

	}
	public IQry<T> LimOfst(out IParam Lim, out IParam Ofst){

	}

}

public class Qry<T> :IQry<T>{}

#endif

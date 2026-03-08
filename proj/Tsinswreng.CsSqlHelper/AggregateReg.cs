namespace Tsinswreng.CsSqlHelper;

public enum EAggRelKind{
	OneToOne = 1,
	OneToMany = 2,
}

public partial interface IAggIncludeReg{
	public Type EntityType{get;}
	public str CodeCol{get;}
	public ITable Tbl{get;}
	public EAggRelKind RelKind{get;}
	public Func<object, object?> FnMembObj{get;}
}

public partial class AggIncludeReg<TPo, TKey>
	: IAggIncludeReg
	where TPo : new()
{
	public Type EntityType => typeof(TPo);
	public str CodeCol{get;set;}
	public ITable Tbl{get;set;}
	public EAggRelKind RelKind{get;set;} = EAggRelKind.OneToMany;
	public Func<TPo, TKey> FnMemb{get;set;}
	public Func<object, object?> FnMembObj => x=>FnMemb((TPo)x);

	public AggIncludeReg(
		ITable<TPo> Tbl
		,str CodeCol
		,EAggRelKind RelKind
		,Func<TPo, TKey> FnMemb
	){
		this.Tbl = Tbl;
		this.CodeCol = CodeCol;
		this.RelKind = RelKind;
		this.FnMemb = FnMemb;
	}
}

public partial interface IAggReg{
	public Type AggType{get;}
	public Type RootEntityType{get;}
	public Type RootIdType{get;}
	public ITable RootTbl{get;}
	public Func<object, object?> FnRootIdObj{get;}
	public IReadOnlyList<IAggIncludeReg> Includes{get;}
}

public partial class AggReg<TAgg, TRoot, TRootId>
	: IAggReg
	where TRoot : class, new()
{
	public Type AggType => typeof(TAgg);
	public Type RootEntityType => typeof(TRoot);
	public Type RootIdType => typeof(TRootId);
	public ITable RootTbl{get;set;}
	public Func<TRoot, TRootId> FnRootId{get;set;}
	public Func<object, object?> FnRootIdObj => x=>FnRootId((TRoot)x);
	public IList<IAggIncludeReg> _Includes{get;set;} = [];
	public IReadOnlyList<IAggIncludeReg> Includes => _Includes.AsReadOnly();

	public AggReg(
		ITable<TRoot> RootTbl
		,Func<TRoot, TRootId> FnRootId
	){
		this.RootTbl = RootTbl;
		this.FnRootId = FnRootId;
	}

	public static AggReg<TAgg, TRoot, TRootId> Mk(
		ITable<TRoot> RootTbl
		,Func<TRoot, TRootId> FnRootId
	){
		return new AggReg<TAgg, TRoot, TRootId>(RootTbl, FnRootId);
	}

	public AggReg<TAgg, TRoot, TRootId> AddInclude<TPo, TKey>(
		ITable<TPo> Tbl
		,str CodeCol
		,EAggRelKind RelKind
		,Func<TPo, TKey> FnMemb
	)
		where TPo : new()
	{
		_Includes.Add(new AggIncludeReg<TPo, TKey>(Tbl, CodeCol, RelKind, FnMemb));
		return this;
	}

	public AggReg<TAgg, TRoot, TRootId> AddOneToOne<TPo, TKey>(
		ITable<TPo> Tbl
		,str CodeCol
		,Func<TPo, TKey> FnMemb
	)
		where TPo : new()
	{
		return AddInclude(Tbl, CodeCol, EAggRelKind.OneToOne, FnMemb);
	}

	public AggReg<TAgg, TRoot, TRootId> AddOneToMany<TPo, TKey>(
		ITable<TPo> Tbl
		,str CodeCol
		,Func<TPo, TKey> FnMemb
	)
		where TPo : new()
	{
		return AddInclude(Tbl, CodeCol, EAggRelKind.OneToMany, FnMemb);
	}
}

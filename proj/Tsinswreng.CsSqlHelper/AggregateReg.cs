namespace Tsinswreng.CsSqlHelper;

public enum EAggRelKind {
	OneToOne = 1,
	OneToMany = 2,
}

public partial interface IAggBuildCtx {
	public nil Add(Type EntityType, object Key, object Entity);
	public IList<object> GetMany(Type EntityType, object Key);
	public object? GetOne(Type EntityType, object Key);
}

public partial class AggBuildCtx
	: IAggBuildCtx {
	public IDictionary<Type, IDictionary<object, IList<object>>> EntityType_Key_Items { get; set; }
		= new Dictionary<Type, IDictionary<object, IList<object>>>();

	public nil Add(Type EntityType, object Key, object Entity) {
		if (!EntityType_Key_Items.TryGetValue(EntityType, out var key_Items)) {
			key_Items = new Dictionary<object, IList<object>>();
			EntityType_Key_Items[EntityType] = key_Items;
		}
		if (!key_Items.TryGetValue(Key, out var items)) {
			items = [];
			key_Items[Key] = items;
		}
		items.Add(Entity);
		return NIL;
	}

	public IList<object> GetMany(Type EntityType, object Key) {
		if (!EntityType_Key_Items.TryGetValue(EntityType, out var key_Items)) {
			return [];
		}
		if (!key_Items.TryGetValue(Key, out var items)) {
			return [];
		}
		return items;
	}

	public object? GetOne(Type EntityType, object Key) {
		return GetMany(EntityType, Key).FirstOrDefault();
	}
}

public static partial class ExtnAggBuildCtx {
	extension(IAggBuildCtx z) {
		public IList<TPo> GetMany<TPo, TKey>(TKey Key) {
			var items = z.GetMany(typeof(TPo), Key!);
			return items.Select(x => (TPo)x).ToList();
		}

		public TPo? GetOne<TPo, TKey>(TKey Key) {
			var one = z.GetOne(typeof(TPo), Key!);
			if (one is null) {
				return default;
			}
			return (TPo)one;
		}
	}
}

public partial interface IAggIncludeReg {
	public Type EntityType { get; }
	public str CodeCol { get; }
	public ITable Tbl { get; }
	public EAggRelKind RelKind { get; }
	public Func<object, object?> FnMembObj { get; }
}

public partial class AggIncludeReg<TPo, TKey>
	: IAggIncludeReg
	where TPo : new() {
	public Type EntityType => typeof(TPo);
	public str CodeCol { get; set; }
	public ITable Tbl { get; set; }
	public EAggRelKind RelKind { get; set; } = EAggRelKind.OneToMany;
	public Func<TPo, TKey> FnMemb { get; set; }
	public Func<object, object?> FnMembObj => x => FnMemb((TPo)x);

	public AggIncludeReg(
		ITable<TPo> Tbl
		, str CodeCol
		, EAggRelKind RelKind
		, Func<TPo, TKey> FnMemb
	) {
		this.Tbl = Tbl;
		this.CodeCol = CodeCol;
		this.RelKind = RelKind;
		this.FnMemb = FnMemb;
	}
}

public partial interface IAggReg {
	public Type AggType { get; }
	public Type RootEntityType { get; }
	public Type RootIdType { get; }
	public ITable RootTbl { get; }
	public Func<object, object?> FnRootIdObj { get; }
	public Func<object, IAggBuildCtx, object> FnAssembleObj { get; }
	public IReadOnlyList<IAggIncludeReg> Includes { get; }
}

public partial class AggReg<TAgg, TRoot, TRootId>
	: IAggReg
	where TRoot : class, new() {
	public Type AggType => typeof(TAgg);
	public Type RootEntityType => typeof(TRoot);
	public Type RootIdType => typeof(TRootId);
	public ITable RootTbl { get; set; }
	public Func<TRoot, TRootId> FnRootId { get; set; }
	public Func<object, object?> FnRootIdObj => x => FnRootId((TRoot)x);
	public Func<TRoot, IAggBuildCtx, TAgg> FnAssemble { get; set; }
	public Func<object, IAggBuildCtx, object> FnAssembleObj => (root, ctx) => FnAssemble((TRoot)root, ctx)!;
	public IList<IAggIncludeReg> _Includes { get; set; } = [];
	public IReadOnlyList<IAggIncludeReg> Includes => _Includes.AsReadOnly();

	public AggReg(
		ITable<TRoot> RootTbl
		, Func<TRoot, TRootId> FnRootId
		, Func<TRoot, IAggBuildCtx, TAgg> FnAssemble
	) {
		this.RootTbl = RootTbl;
		this.FnRootId = FnRootId;
		this.FnAssemble = FnAssemble;
	}

	public static AggReg<TAgg, TRoot, TRootId> Mk(
		ITable<TRoot> RootTbl
		, Func<TRoot, TRootId> FnRootId
		, Func<TRoot, IAggBuildCtx, TAgg> FnAssemble
	) {
		return new AggReg<TAgg, TRoot, TRootId>(RootTbl, FnRootId, FnAssemble);
	}

	public AggReg<TAgg, TRoot, TRootId> AddInclude<TPo, TKey>(
		ITable<TPo> Tbl
		, str CodeCol
		, EAggRelKind RelKind
		, Func<TPo, TKey> FnMemb
	)
		where TPo : new() {
		_Includes.Add(new AggIncludeReg<TPo, TKey>(Tbl, CodeCol, RelKind, FnMemb));
		return this;
	}

	public AggReg<TAgg, TRoot, TRootId> AddOneToOne<TPo, TKey>(
		ITable<TPo> Tbl
		, str CodeCol
		, Func<TPo, TKey> FnMemb
	)
		where TPo : new() {
		return AddInclude(Tbl, CodeCol, EAggRelKind.OneToOne, FnMemb);
	}

	public AggReg<TAgg, TRoot, TRootId> AddOneToMany<TPo, TKey>(
		ITable<TPo> Tbl
		, str CodeCol
		, Func<TPo, TKey> FnMemb
	)
		where TPo : new() {
		return AddInclude(Tbl, CodeCol, EAggRelKind.OneToMany, FnMemb);
	}
}

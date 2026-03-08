namespace Tsinswreng.CsSqlHelper;

file class __FileDoc{
	
}

/// <summary>
/// Defines the relationship kind between a root aggregate entity and included entities.
/// Used to distinguish how related entities should be loaded and assembled.
/// Example: A Word (root) can have multiple WordProps (OneToMany) or a single User (OneToOne)
/// </summary>
public enum EAggRelKind {
	/// <summary>One-to-one relationship: Each root entity has exactly one related entity</summary>
	OneToOne = 1,
	/// <summary>One-to-many relationship: Each root entity can have zero or more related entities</summary>
	OneToMany = 2,
}

/// <summary>
/// Context for building/assembling aggregates during data retrieval.
/// Acts as a temporary cache that holds related entities grouped by type and key,
/// enabling the assembly phase to fetch pre-loaded related data.
/// Example: When loading a Word aggregate with its Props and Learn records,
/// the context stores lists of Props and Learn records keyed by WordId,
/// so the FnAssemble function can retrieve them without additional queries.
/// </summary>
public partial interface IAggBuildCtx {
	/// <summary>Add a related entity to the context under its type and key</summary>
	/// <param name="EntityType">The CLR type of the entity (e.g., typeof(PoWordProp))</param>
	/// <param name="Key">The grouping key (e.g., WordId value)</param>
	/// <param name="Entity">The entity instance to add</param>
	public nil Add(Type EntityType, object Key, object Entity);

	/// <summary>Retrieve all related entities of a given type and key</summary>
	/// <returns>A list of entities matching the type and key; empty list if not found</returns>
	public IList<object> GetMany(Type EntityType, object Key);

	/// <summary>Retrieve the first related entity of a given type and key</summary>
	/// <returns>The first entity, or null if not found</returns>
	public object? GetOne(Type EntityType, object Key);
}

/// <summary>
/// Default implementation of IAggBuildCtx that stores entities in nested dictionaries.
/// The structure is: Dictionary[EntityType] -> Dictionary[Key] -> List[Entities]
/// </summary>
public partial class AggBuildCtx
	: IAggBuildCtx {
	/// <summary>
	/// Three-level storage structure for aggregated entities.
	/// Level 1: EntityType (e.g., typeof(PoWordProp))
	/// Level 2: Key value (e.g., WordId: 123)
	/// Level 3: List of all entities matching that type and key
	/// </summary>
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

/// <summary>
/// Strongly-typed extension methods for IAggBuildCtx that avoid boxing/unboxing.
/// These provide generic versions of GetMany and GetOne.
/// Example: Instead of ctx.GetOne(typeof(User), userId) and casting,
/// use ctx.GetOne<User, IdUser>(userId) for type safety.
/// </summary>
public static partial class ExtnAggBuildCtx {
	extension(IAggBuildCtx z) {
		/// <summary>
		/// Retrieve all related entities of type TPo with key TKey.
		/// Internally uses GetMany(Type, object) but returns a strongly-typed list.
		/// </summary>
		/// <typeparam name="TPo">The persistent object type to retrieve</typeparam>
		/// <typeparam name="TKey">The key type used for grouping</typeparam>
		/// <param name="Key">The key value to lookup (e.g., WordId: 123)</param>
		/// <returns>List of TPo instances; empty if not found. Never null.</returns>
		public IList<TPo> GetMany<TPo, TKey>(TKey Key) {
			var items = z.GetMany(typeof(TPo), Key!);
			return items.Select(x => (TPo)x).ToList();
		}

		/// <summary>
		/// Retrieve the first related entity of type TPo with key TKey.
		/// Useful for one-to-one relationships.
		/// Example: var user = ctx.GetOne<User, IdAccount>(accountId);
		/// </summary>
		/// <typeparam name="TPo">The persistent object type to retrieve</typeparam>
		/// <typeparam name="TKey">The key type used for grouping</typeparam>
		/// <param name="Key">The key value to lookup</param>
		/// <returns>The first TPo instance or null if not found</returns>
		public TPo? GetOne<TPo, TKey>(TKey Key) {
			var one = z.GetOne(typeof(TPo), Key!);
			if (one is null) {
				return default;
			}
			return (TPo)one;
		}
	}
}

/// <summary>
/// Metadata about a single related entity set in an aggregate.
/// Stores information about how to load and retrieve related entities of a specific type.
/// Example: An aggregate may have multiple includes:
///   - PoWordProp items joined on WordId (OneToMany)
///   - PoWordLearn items joined on WordId (OneToMany)
///   - PoWordMeaning item joined on WordId (OneToOne)
/// </summary>
public partial interface IAggIncludeReg {
	/// <summary>The CLR type of the related entity (e.g., typeof(PoWordProp))</summary>
	public Type EntityType { get; }

	/// <summary>
	/// The column name in the related table used as the join key.
	/// Example: "WordId" in the WordProp table
	/// </summary>
	public str CodeCol { get; }

	/// <summary>The table definition for the related entity type</summary>
	public ITable Tbl { get; }

	/// <summary>Whether this is a one-to-one or one-to-many relationship</summary>
	public EAggRelKind RelKind { get; }

	/// <summary>
	/// Function that extracts the join key from a root entity.
	/// Used to determine which related entities belong to a given root.
	/// Example: For WordProp items, this extracts the WordId from a Word root entity.
	/// </summary>
	public Func<object, object?> FnMembObj { get; }
}

/// <summary>
/// Strongly-typed registration of a single related entity set in an aggregate.
/// Stores configuration for loading related entities of type TPo, grouped by key TKey from a root entity.
/// 
/// Example: In a Word aggregate:
///   new AggIncludeReg<PoWordProp, IdWord>(
///       Tbl_Prop,
///       "WordId",
///       EAggRelKind.OneToMany,
///       x => x.WordId  // Extract WordId from each PoWordProp
///   )
/// </summary>
/// <typeparam name="TPo">The type of related entities (e.g., PoWordProp)</typeparam>
/// <typeparam name="TKey">The key type used to group related entities (e.g., IdWord)</typeparam>
public partial class AggIncludeReg<TPo, TKey>
	: IAggIncludeReg
	where TPo : new() {
	public Type EntityType => typeof(TPo);

	/// <summary>The column name in this entity's table used for joining to the root</summary>
	public str CodeCol { get; set; }

	/// <summary>The table definition containing entities of type TPo</summary>
	public ITable Tbl { get; set; }

	/// <summary>The relationship kind (OneToOne or OneToMany)</summary>
	public EAggRelKind RelKind { get; set; } = EAggRelKind.OneToMany;

	/// <summary>
	/// Strongly-typed function to extract the join key from a TPo instance.
	/// Example: x => x.WordId
	/// </summary>
	public Func<TPo, TKey> FnMemb { get; set; }

	/// <summary>Object-typed wrapper of FnMemb for runtime use</summary>
	public Func<object, object?> FnMembObj => x => FnMemb((TPo)x);

	/// <summary>
	/// Create a new strongly-typed include registration.
	/// 
	/// Example:
	///   var reg = new AggIncludeReg<PoWordProp, IdWord>(
	///       Tbl_Prop.Tbl,
	///       "WordId",
	///       EAggRelKind.OneToMany,
	///       x => x.WordId
	///   );
	/// </summary>
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

/// <summary>
/// Metadata describing how to load and assemble a complete aggregate from its root and related entities.
/// An aggregate is a graph of related entities centered around a root entity.
/// 
/// Example: A JnWord aggregate consists of:
///   - Root: PoWord (the main entity)
///   - Includes: PoWordProp and PoWordLearn (loaded separately and attached to the root)
///   - Assembly: Creates a JnWord(root, props, learns) from the root and loaded related entities.
/// 
/// The registration system works in 2 phases:
/// 1. LOAD: Load root entities by their IDs, then load all related entities
/// 2. ASSEMBLE: For each root, invoke FnAssemble with the root and a context containing pre-loaded related entities
/// </summary>
public partial interface IAggReg {
	/// <summary>The aggregate type being registered (e.g., typeof(JnWord))</summary>
	public Type AggType { get; }

	/// <summary>The type of the root entity (e.g., typeof(PoWord))</summary>
	public Type RootEntityType { get; }

	/// <summary>The type of the root entity's ID (e.g., typeof(IdWord))</summary>
	public Type RootIdType { get; }

	/// <summary>The table definition for root entities</summary>
	public ITable RootTbl { get; }

	/// <summary>
	/// Function that extracts the ID from a root entity object.
	/// Example: x => x.Id where x is a PoWord
	/// </summary>
	public Func<object, object?> FnRootIdObj { get; }

	/// <summary>
	/// Function that assembles a complete aggregate from a root and pre-loaded related entities.
	/// The context parameter contains all related entities grouped by type and key.
	/// Example: (root, ctx) => new JnWord(root, ctx.GetMany<PoWordProp, IdWord>(root.Id), ...)
	/// </summary>
	public Func<object, IAggBuildCtx, object> FnAssembleObj { get; }

	/// <summary>
	/// List of all included related entity sets.
	/// Each entry describes a one-to-many or one-to-one relationship.
	/// </summary>
	public IReadOnlyList<IAggIncludeReg> Includes { get; }
}

/// <summary>
/// Strongly-typed registration of a complete aggregate.
/// Defines how to load and assemble a TAgg aggregate from its TRoot root entity and related entities identified by TRootId.
/// 
/// Example:
///   var agg = AggReg<JnWord, PoWord, IdWord>.Mk(
///       Tbl_Word.Tbl,
///       x => x.Id,
///       (root, ctx) => new JnWord(
///           root,
///           ctx.GetMany<PoWordProp, IdWord>(root.Id),
///           ctx.GetMany<PoWordLearn, IdWord>(root.Id)
///       )
///   )
///   .AddOneToMany(Tbl_Prop.Tbl, "WordId", x => x.WordId)
///   .AddOneToMany(Tbl_Learn.Tbl, "WordId", x => x.WordId);
/// 
/// When BatSlctAggById is called with Word IDs:
/// 1. Loads PoWord rows for each ID
/// 2. Loads all PoWordProp rows where WordId matches
/// 3. Loads all PoWordLearn rows where WordId matches
/// 4. For each root PoWord, calls FnAssemble with the root and a context containing the matched related entities
/// 5. Returns the assembled JnWord aggregates
/// </summary>
/// <typeparam name="TAgg">The aggregate root type (e.g., JnWord)</typeparam>
/// <typeparam name="TRoot">The persistent root entity type (e.g., PoWord)</typeparam>
/// <typeparam name="TRootId">The ID type of the root entity (e.g., IdWord)</typeparam>
public partial class AggReg<TAgg, TRoot, TRootId>
	: IAggReg
	where TRoot : class, new() {
	public Type AggType => typeof(TAgg);
	public Type RootEntityType => typeof(TRoot);
	public Type RootIdType => typeof(TRootId);

	/// <summary>The table definition for root entities</summary>
	public ITable RootTbl { get; set; }

	/// <summary>Strongly-typed function to extract the ID from a root entity</summary>
	public Func<TRoot, TRootId> FnRootId { get; set; }

	/// <summary>Object-typed wrapper of FnRootId</summary>
	public Func<object, object?> FnRootIdObj => x => FnRootId((TRoot)x);

	/// <summary>
	/// Strongly-typed function that assembles the complete aggregate.
	/// Parameters:
	///   - TRoot: The root entity instance
	///   - IAggBuildCtx: Context containing pre-loaded related entities
	/// Returns: The assembled TAgg instance
	/// 
	/// Example: (root, ctx) => new JnWord(root, ctx.GetMany<PoWordProp, IdWord>(root.Id), ...)
	/// </summary>
	public Func<TRoot, IAggBuildCtx, TAgg> FnAssemble { get; set; }

	/// <summary>Object-typed wrapper of FnAssemble</summary>
	public Func<object, IAggBuildCtx, object> FnAssembleObj => (root, ctx) => FnAssemble((TRoot)root, ctx)!;

	/// <summary>List of included related entity sets (stored as mutable list internally)</summary>
	public IList<IAggIncludeReg> _Includes { get; set; } = [];

	/// <summary>Read-only list of included related entity sets</summary>
	public IReadOnlyList<IAggIncludeReg> Includes => _Includes.AsReadOnly();

	/// <summary>
	/// Create a new aggregate registration.
	/// 
	/// Example:
	///   new AggReg<JnWord, PoWord, IdWord>(
	///       Tbl_Word.Tbl,
	///       x => x.Id,
	///       (root, ctx) => new JnWord(root, ctx.GetMany<PoWordProp, IdWord>(root.Id), ...)
	///   )
	/// </summary>
	/// <param name="RootTbl">The table containing root entities</param>
	/// <param name="FnRootId">Function to extract the ID from a root entity</param>
	/// <param name="FnAssemble">Function to assemble the aggregate from root and context</param>
	public AggReg(
		ITable<TRoot> RootTbl
		, Func<TRoot, TRootId> FnRootId
		, Func<TRoot, IAggBuildCtx, TAgg> FnAssemble
	) {
		this.RootTbl = RootTbl;
		this.FnRootId = FnRootId;
		this.FnAssemble = FnAssemble;
	}

	/// <summary>
	/// Static factory method to create an aggregate registration.
	/// Equivalent to calling the constructor directly.
	/// </summary>
	public static AggReg<TAgg, TRoot, TRootId> Mk(
		ITable<TRoot> RootTbl
		, Func<TRoot, TRootId> FnRootId
		, Func<TRoot, IAggBuildCtx, TAgg> FnAssemble
	) {
		return new AggReg<TAgg, TRoot, TRootId>(RootTbl, FnRootId, FnAssemble);
	}

	/// <summary>
	/// Add a related entity set to this aggregate with unspecified relationship kind.
	/// Use AddOneToOne or AddOneToMany for convenience methods instead.
	/// 
	/// Example:
	///   .AddInclude(
	///       Tbl_Prop.Tbl,
	///       "WordId",
	///       EAggRelKind.OneToMany,
	///       x => x.WordId
	///   )
	/// </summary>
	/// <param name="Tbl">The table containing related entities</param>
	/// <param name="CodeCol">The column name to join on (e.g., "WordId")</param>
	/// <param name="RelKind">The relationship kind (OneToOne or OneToMany)</param>
	/// <param name="FnMemb">Function to extract the join key from a related entity</param>
	/// <returns>This registration for method chaining</returns>
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

	/// <summary>
	/// Add a one-to-one related entity set to this aggregate.
	/// Each root entity will have at most one related entity of this type.
	/// 
	/// Example: Adding a single User for each Account
	///   .AddOneToOne(
	///       Tbl_User.Tbl,
	///       "AccountId",
	///       x => x.AccountId
	///   )
	/// </summary>
	/// <param name="Tbl">The table containing related entities</param>
	/// <param name="CodeCol">The join column name</param>
	/// <param name="FnMemb">Function to extract the join key from a related entity</param>
	/// <returns>This registration for method chaining</returns>
	public AggReg<TAgg, TRoot, TRootId> AddOneToOne<TPo, TKey>(
		ITable<TPo> Tbl
		, str CodeCol
		, Func<TPo, TKey> FnMemb
	)
		where TPo : new() {
		return AddInclude(Tbl, CodeCol, EAggRelKind.OneToOne, FnMemb);
	}

	/// <summary>
	/// Add a one-to-many related entity set to this aggregate.
	/// Each root entity can have zero or more related entities of this type.
	/// 
	/// Example: Adding multiple properties and learning records to a Word
	///   .AddOneToMany(
	///       Tbl_Prop.Tbl,
	///       "WordId",
	///       x => x.WordId
	///   )
	///   .AddOneToMany(
	///       Tbl_Learn.Tbl,
	///       "WordId",
	///       x => x.WordId
	///   )
	/// </summary>
	/// <param name="Tbl">The table containing related entities</param>
	/// <param name="CodeCol">The join column name</param>
	/// <param name="FnMemb">Function to extract the join key from a related entity</param>
	/// <returns>This registration for method chaining</returns>
	public AggReg<TAgg, TRoot, TRootId> AddOneToMany<TPo, TKey>(
		ITable<TPo> Tbl
		, str CodeCol
		, Func<TPo, TKey> FnMemb
	)
		where TPo : new() {
		return AddInclude(Tbl, CodeCol, EAggRelKind.OneToMany, FnMemb);
	}
}

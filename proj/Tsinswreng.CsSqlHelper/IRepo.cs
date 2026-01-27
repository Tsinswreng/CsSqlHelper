namespace Tsinswreng.CsSqlHelper;

using Tsinswreng.CsPage;
using IStr_Any = System.Collections.Generic.IDictionary<str, obj?>;

[Doc($@"
#Sum[Repository interface for database common operations on entities]
#TParams([Entity type],[Entity ID type])
")]
public partial interface IRepo<TEntity, TId>{

	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertMany(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		IAsyncEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertAsyE(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		TEntity
		,CT
		,Task<nil>
	>> FnInsertOne(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		CT
		,Task<u64>
	>> FnCount(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		TId
		,CT
		,Task<TEntity?>
	>> FnSlctOneById(
		IDbFnCtx Ctx
		,CT Ct
	);


	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<IDictionary<str, obj?>>>
	>> FnPageAllDict(IDbFnCtx Ctx, CT Ct);


	public Task<Func<
		IPageQry
		,CT, Task<IPageAsyE<TEntity>>
	>> FnPageAll(IDbFnCtx Ctx, CT Ct);


	[Obsolete]
	public Task<Func<
		TId
		,TEntity
		,CT, Task<nil>
	>> FnUpdByIdOld(
		IDbFnCtx Ctx
		,IEnumerable<str>? FieldsToUpdate
		,CT Ct
	);


	[Obsolete]
	public Task<Func<
		IEnumerable<Id_Dict<TId>>
		,CT
		,Task<nil>
	>> FnUpdManyByIdOld(
		IDbFnCtx Ctx
		,IEnumerable<str> FieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		TEntity
		,CT, Task<nil>
	>> FnUpdOneById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnUpdManyById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		IAsyncEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnAsyEUpdManyById(
		IDbFnCtx Ctx
		,IEnumerable<str>? UpperFieldsToUpdate
		,CT Ct
	);


	public Task<Func<
		IEnumerable<obj?>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys(
		IDbFnCtx Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	);


	public Task<Func<
		IEnumerable<TKey>
		,CT
		,Task<nil>
	>> FnSoftDelManyByKeys<TKey>(
		IDbFnCtx Ctx
		,str KeyNameInCode
		,u64 CountPerBatch
		,CT Ct
	);


	/// 不預編譯。適用于況芝 在事務中 初建表後即添數據
	public Task<Func<
		IEnumerable<TEntity>
		,CT
		,Task<nil>
	>> FnInsertManyNoPrepare(
		IDbFnCtx Ctx
		,CT ct
	);


	public Task<Func<
		TId
		,obj?
		,CT
		,Task<nil>
	>> FnUpdOneColById(
		IDbFnCtx Ctx
		,str Col
		,CT Ct
	);


	public Task<Func<
		IList<TVal>
		,CT
		,Task<IAsyncEnumerable<IStr_Any>>
	>> FnScltAllByColInVals<TVal>(
		IDbFnCtx Ctx
		,ITable Tbl
		,str CodeCol
		,OptQry? OptQry
		,CT Ct
	);


	public Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey> Keys
		,Func<TPo, TKey> FnMemb
		,ITable Tbl
		,CT Ct
	)where TPo: new();


	public Task<IDictionary<TKey, IList<TPo>>> IncludeEntitysByKeys<TPo, TKey>(
		IDbFnCtx Ctx
		,str CodeCol
		,OptQry? OptQry
		,IEnumerable<TKey> Keys
		,Func<TPo, TKey> FnMemb
		,ITable<TPo> Tbl
		,CT Ct
	)where TPo: new();


	public Task<Func<
		IList<TCol>
		,CT
		,Task<IAsyncEnumerable<TEntity2>>
	>> FnScltAllByColInVals<TEntity2, TCol>(
		IDbFnCtx Ctx
		,ITable Tbl
		,str CodeCol
		,OptQry? OptQry
		,CT Ct
	)
		where TEntity2 : class, new();

}

/*

幾種批量方案:
總ʹ思路: 同結構多Sql拼接、以分號分隔、讀多個結果集
1: 不直接構建Sql字符串、內ʸ用現成ʹ庫㕥建Sql AST。末把.ToSqlStr() 改成 .ToSqlStrMany(Opt)時 依OptʹBatchSize以生成多組sql。
弊: 參數綁定麻煩。需改out var IParam
2. 把out var IParam 全改潙 out var IList<IParam>。 內ʸAddSeg時勿直ᵈ拼sql片段 而是 拼 自定義對象。自定義對象中 區分 參數與 sql片段字符串。
需批量拼sql旹 傳入批次大小、IList<IParam>.Count即其批次大小。不需批量旹 綁參數可用 Params[0]、只多寫一個[0]、亦不麻煩。
再使ArgDict.AddT適配IList<IParam>、不批量時自動取IParam[0]等 則更不需變寫法。
 */

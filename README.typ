//pandoc .\README.typ -o README.md
= CsSqlHelper//https://github.com/Tsinswreng/CsSqlHelper

*⚠️This project is only for my personal user now. It has no release version yet and everything may change. If you really want to try it, we recommand you to clone the code and reference the source code directly (including some of my dependencies in my other repos).⚠️*

//TODO page

CsSqlHelper is a tiny ORM
- AOT-compatible using #link("https://github.com/Tsinswreng/CsDictMapper")[CsDictMapper]
- Functional Crud
- Make your sql string portable(database independent)
- Abstact interfaces to adapt to different database or ORM
- Adapted Sqlite, PostgreSql, EFCore by now
- Field and Type mapping
- Soft delete
- Transaction support
- #link("https://github.com/Tsinswreng/CsPage")[Page query]
- Especially suitable for client app with sqlite


== Config Entities

=== Define Entities
suppose you use strongly typed Id type like this:
```cs
public struct UserId{
	public long Value;
	//......
}

```

```cs
public class User{
	public UserId Id{get;set;}
	public string Email{get;set;}
	public long CreatedAt{get;set;}
	public int Status{get;set;}
}
```

=== Config DictMapper
config DictMapper to convert between dictionary and entity:

for more infomation about the DictMapper, see `https://github.com/Tsinswreng/CsDictMapper`
```cs
using Tsinswreng.CsDictMapper;
namespace MyApp;
[DictType(typeof(User))]
public partial class AppDictMapper{

}
```

=== Config the entity
We use sqlite as an example:
```cs
public class AppTblMgr:SqliteTblMgr{
	protected static LocalTblMgr Inst = new();
}

public class AppSchemaCfg{
	protected ITblMgr Mgr{get;set;} //should be injected with `AppTblMgr`

	/// define a helper function for simplify the code to create table object
	protected ITable Mk<T>(str DbTblName){
		return Table.FnMkTbl<T>(AppDictMapper.Inst)(DbTblName);
	}

	public AppSchemaCfg(){
		// create table object for entity `User`
		var TblUser = Mk<User>("User")
		// Add in App Tables Manager
		Mgr.AddTable(TblUser);
		{
			var o = TblUser;
			// set the primary key column
			o.CodeIdName = nameof(User.Id);
			// config type mapping and conversion for Id column
			o.SetCol("id")
			.AddtitionalSqls(["PRIMARY KEY"])
			.HasConversionEtMapType(
				DbTypeConvFns<long, IdUser>.Mk(
					(id)=>id.Value
					,(val)=>IdUser.FromRaw(val)
				);
			)
			#if false
			//you can also directly pass lambdas:
			o.SetCol("id")
			.AddtitionalSqls(["PRIMARY KEY"])
			.HasConversionEtMapType<long, IdUser>(
				(id)=>id.Value
				,(val)=>IdUser.FromRaw(val)
			)
			#endif

			// add addtional sql which will be placed inside the `CREATE TABLE()` statement, e.g. add constrain
			o.InnerAdditionalSqls.AddRange([
				$"UNIQUE({o.Field(nameof(User.Email))})"
			]);

			// add addtional sql which will be placed outside the `CREATE TABLE()` statement, e.g create index
			o.OuterAdditionalSqls.AddRange([
				$"CREATE INDEX {0.Quote("Idx_Email")} ON {o.Quote(o.DbTblName) ({o.Field(nameof(User.Email))}) }"
			])

			// config the column for logic delete
			o.SoftDelCol = new SoftDelCol{
				CodeColName = nameof(User.Status)
				,FnDelete = (statusObj)=>Status.Deleted
				,FnRestore = (statusObj)=>Status.Normal
			}
		}
	}
}
```

== Config Dependency injection
```cs
//database connection
z.AddSingleton(LocalDb.Inst.DbConnection);
//sql command maker
z.AddScoped<ISqlCmdMkr, SqliteCmdMkr>();
//app tables manager
z.AddSingleton<ITblMgr>(LocalTblMgr.Inst);
//transaction maker
z.AddScoped<I_GetTxnAsy, SqliteCmdMkr>();
//transaction runner
z.AddScoped<ITxnRunner, AdoTxnRunner>();
//transaction function wrapper
z.AddScoped<TxnWrapper<DbFnCtx>>();
```

== Generate the sql to initiate your database schema
```cs
new AppTblMgr().SqlMkSchema();
```

= Query

== Run Custom Sql
This project offers Repository class which encapsulates some basic curd operations.
to learn how to run custom sql, let's see `FnSelectById` from the Reposity class

We use inner function and closure. It has the following advantages:
- pre-compile sql command
- run the same sql function for many times and pass different parameters.
- easy to combine

```cs
using CT = CancellationToken;
/// TId: The type of the primary key of the entity. It can be any type, including self-encapsulated strongly typed Id type
public class Repo<TEntity, TId>:IRepo<TEntity, TId>
	where TEntity: class, new()
{
	// the below properties will be injected by DI container

	/// Tables manager of your App
	public ITblMgr TblMgr{get;set;}
	/// Sql Command Maker
	public ISqlCmdMkr SqlCmdMkr{get;set;}
	/// use source generator to convert between dictionary and entity
	public IDictMapperShallow DictMapper{get;set;}

/// IDbFnCtx: the context of the database function, including the transaction object, etc.
	public async Task<Func<
		TId ,CT ,Task<TEntity?>
	>> FnSelectById(IDbFnCtx? Ctx ,CT Ct){
// in the outer function, we usually do some preparation work, e.g. create sql string, pre-compile sql command
// or prepare other functions that returns an inner function
		var T = TblMgr.GetTable<TEntity>(); // Get table object
		var Params = T.MkParams(0,0); // create sql parameter strings
		var Sql = $"SELECT * FROM {T.Quote(T.DbTblName)} WHERE {T.Field(T.CodeIdName)} = {Params[0]}" ;
// `Sql` distincts according to the database type, e.g. if db is sqlite, Sql will be "SELECT * FROM "User" WHERE "Id" = @0"
// we recommand to use `T.Quote` `T.Field` to make your sql string more portable.

		var Cmd = await SqlCmdMkr.Prepare(Ctx, Sql, Ct);
	// Pre-compile the SQL command

		var Fn = async(TId Id ,CT Ct)=>{
// in the inner function, we execute the SQL command with the given parameters
			var IdCol = T.Columns[T.CodeIdName];
			// from stronly typed Id to raw value. If you do not use strongly typed id, it will return as-is
			var ConvertedId = IdCol.UpperToRaw(Id);

			var RawDict = await Cmd
				.Args([ConvertedId]) // pass number parameters
				// .Args(dictionary) // we also support pass parameters by dictionary
				.Run(Ct).FirstOrDefaultAsync(Ct)
			;
			if(RawDict == null){
				return null;
			}
			var CodeDict = T.ToCodeDict(RawDict); // type mapping, from raw to upper(user self-encapsulated)
			var R = new TEntity();
			this.DictMapper.AssignShallowT(R, CodeDict); // assign from dictionary to entity object using CsDictMapper, which is AOT-compatible
			return R;
		};
		return Fn;
	}
}
```

== Combine Db Functions and run in transaction

```cs
public class UserService(
	TxnWrapper<DbFnCtx> TxnWrapper
	,IRepo<PoUser, IdUser> UserRepo
){

	public async Task<Func<
		UserId, string, CT, Task<nil>
	>> FnUpdateEmailById(IDbFnCtx? Ctx, CT Ct){

		//combine db functions
		var SelectUserById = await UserRepo.FnSelectById(Ctx, Ct);
		var UpdateUsersById = await UserRepo.FnUpdateManyById(Ctx, Ct);

		var Fn = (UserId userId, string NewEmail, CT Ct){
			var User = await SelectUserById(userId, Ct);
			User.Email = NewEmail
			var NewUserDict = AppDictMapper.Inst.ToDictShallowT(User);
			await UpdateUsersById([new Id_Dict(UserId, NewUserDict)], Ct);
			return NIL;
		}
	}

	/// run in transaction
	public async Task<nil> UpdateEmailById(UserId userId, string NewEmail, CT Ct){
		return await TxnWrapper.Wrap(FnUpdateEmailById, userId, NewEmail, Ct);
	}

}
```

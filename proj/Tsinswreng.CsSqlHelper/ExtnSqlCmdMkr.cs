// using System.Collections;
// using Tsinswreng.CsTools;

// namespace Tsinswreng.CsSqlHelper;

// public interface IBatchSqlOfSize{
// 	public u64 Size{get;set;}
// 	public ISqlCmd Cmd{get;set;}
// 	//public ISql SqlSingle{get;set;}
// 	public ISql JoinedSql{get;set;}
// }

// public class BatchSqlOfSize: IBatchSqlOfSize{
// 	public u64 Size{get;set;}
// 	public ISqlCmd Cmd{get;set;}
// 	public ISql SqlSingle{get;set;}
// 	public ISql JoinedSql{get;set;}

// }


// public class Batch{
// 	public ISqlCmdMkr SqlCmdMkr;
// 	public IList<u64> BatchDesk = [];
// 	public IList<u64> SizeDesc = [500, 20, 1];
// 	public IList<IBatchSqlOfSize> CmdsDesc = [];
// 	public Func<u64, ISql> FnMkSql;
// 	public u64 BatchCollectorSize{get=>SizeDesc[0];}
// 	public str ToJoin = "";
// 	public ISql MkSqlNo(u64 No){
// 		return FnMkSql(No);
// 	}

// 	public ISql MkMultiSqlInOne(u64 Cnt){
// 		var R = new List<str>();
// 		for(u64 i = 0; i < Cnt; i++){
// 			var ua = MkSqlNo(i);
// 			R.Add(ua.RawStr);
// 		}
// 		var JoinedStr = str.Join(ToJoin, R);
// 		return new Sql(JoinedStr);
// 	}

// 	public async Task<IBatchSqlOfSize> MkOneBatchAsy(u64 Size, CT Ct){
// 		var Sql = MkMultiSqlInOne(Size);
// 		var Cmd = await SqlCmdMkr.Prepare(null, Sql, Ct);
// 		var R = new BatchSqlOfSize{
// 			Size = Size,
// 			Cmd = Cmd,
// 			JoinedSql = Sql,
// 		};
// 		return R;
// 	}

// 	public async Task<nil> InitCmdsAsy(CT Ct){
// 		CmdsDesc = [];
// 		for(var i = 0; i < SizeDesc.Count; i++){
// 			var Batch = await MkOneBatchAsy(SizeDesc[i], Ct);
// 			CmdsDesc.Add(Batch);
// 		}
// 		return NIL;
// 	}

// 	public IDictionary<str, obj?> Flatten(IList<IDictionary<str, obj?>> ArgDicts){
// 		var R = new Dictionary<str, obj?>();
// 		foreach(var ArgDict in ArgDicts){
// 			foreach(var (k,v) in ArgDict){
// 				ArgDict[k] = v;
// 			}
// 		}
// 		return R;
// 	}


// 	public async Task RunAsy(IList<IArgDict> Args, CT Ct){
// 		await using BatchList = new BatchCollector<IDictionary<str, obj?>,nil>(
// 			async(ArgDicts, Ct)=>{
// 				u64 BatchSize = 1;
// 				var index = 0;
// 				for(index = 0; index < SizeDesc.Count; index++){
// 					BatchSize = SizeDesc[index];
// 					if((u64)ArgDicts.Count > BatchSize){
// 						break;
// 					}
// 				}

// 				var CmdCtx = CmdsDesc[index];
// 				CmdCtx.Cmd.RawArgs(Flatten(ArgDicts))



// 			}
// 			,BatchCollectorSize
// 		);
// 	}

// 	public static (u64 Quotient, u64 Remainder) DivideWithRemainder(
// 		u64 Dividend, u64 Divisor
// 	){
// 		if (Divisor == 0)
// 			throw new ArgumentException("除數不可為 0", nameof(Divisor));

// 		u64 quotient = Dividend / Divisor;
// 		u64 remainder = Dividend % Divisor;

// 		return (quotient, remainder);
// 	}


// }

// public static class ExtnSqlCmdMkr{
// 	// public static Task<ISqlCmd> MkCmd(
// 	// 	this ISqlCmdMkr z
// 	// 	,IBaseDbFnCtx? DbFnCtx
// 	// 	,Func<>
// 	// 	,CT ct
// 	// ){

// 	// }
// }


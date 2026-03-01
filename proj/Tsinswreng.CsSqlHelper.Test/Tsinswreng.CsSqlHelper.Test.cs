using Tsinswreng.CsSqlHelper.Test.Domains;

var sqliteSql = TestTblMgrIniter.MkSqliteMgr().SqlMkSchema();
var postgresSql = TestTblMgrIniter.MkPostgresMgr().SqlMkSchema();

Console.WriteLine("===== SQLITE =====");
Console.WriteLine(sqliteSql);
Console.WriteLine();

Console.WriteLine("===== POSTGRES =====");
Console.WriteLine(postgresSql);

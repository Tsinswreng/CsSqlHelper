namespace Tsinswreng.CsSqlHelper.Postgres;

using System.Data;
using Npgsql;

public class PostgresConnPool : I_GetDbConnAsy{
	NpgsqlDataSource DataSource;
	PostgresConnPool(
		NpgsqlDataSource DataSource
	){
		this.DataSource = DataSource;
	}
	public async Task<IDbConnection> GetConnAsy(CT Ct){
		return await DataSource.OpenConnectionAsync(Ct);
	}
}

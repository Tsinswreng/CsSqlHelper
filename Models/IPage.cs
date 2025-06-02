namespace Tsinswreng.SqlHelper.Models;

public interface IPageInfo{
	/// <summary>
	/// from 0
	/// </summary>
	public u64 PageIndex{get;set;}
	public u64 PageSize{get;set;}
}


public interface I_HasTotalCount{
	public bool HasTotalCount{get;set;}
}

public interface ITotalCount{
	public u64 TotalCount{get;set;}
}

public interface IPageQuery
	:IPageInfo
	,I_HasTotalCount
{

}

public interface IPageAsy<T>
	:ITotalCount
	,IPageInfo
	,I_HasTotalCount
{
	public IAsyncEnumerable<T>? DataAsy{get;set;}
}


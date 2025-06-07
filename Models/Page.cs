namespace Tsinswreng.CsSqlHelper.Models;

public class PageQuery
	:IPageQuery
{
	/// <summary>
	/// from 0
	/// </summary>
	public u64 PageIndex{get;set;}
	public u64 PageSize{get;set;}
	public bool HasTotalCount{get;set;}
	public static IPageQuery SelectAll(){
		var R = new PageQuery();
		R.PageIndex = 0;
		R.PageSize = u64.MaxValue;
		R.HasTotalCount = true;
		return R;
	}
}

public class PageAsy<T>
	:ITotalCount
	,IPageQuery
	,IPageAsy<T>
{
	public PageAsy(){}
	public static IPageAsy<T> Mk(
		IPageQuery Qry
		,u64 TotalCount
		,IAsyncEnumerable<T>? DataAsy
	){
		return new PageAsy<T>(Qry,TotalCount,DataAsy);
	}
	public PageAsy(
		IPageQuery Qry
		,u64 TotalCount
		,IAsyncEnumerable<T>? DataAsy
	){
		this.TotalCount = TotalCount;
		this.PageIndex = Qry.PageIndex;
		this.PageSize = Qry.PageSize;
		this.DataAsy = DataAsy;
		this.HasTotalCount = Qry.HasTotalCount;
	}
	public bool HasTotalCount{get;set;}
	public u64 TotalCount{get;set;}
	public u64 PageIndex{get;set;}
	public u64 PageSize{get;set;}
	public IAsyncEnumerable<T>? DataAsy{get;set;}

}


public static class ExtnPage{
	public static u64 Offset(
		this IPageInfo z
	){
		return z.PageIndex * z.PageSize;
	}
}

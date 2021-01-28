using System.Collections;
using System.Collections.Generic;

namespace EI.RP.CoreServices.Cqrs.Queries
{
	/// <summary>
	/// used to avoid reprocessing each individual reqeust of the items in a collection result
	/// </summary>
	/// <typeparam name="TQueryModel"></typeparam>
	/// <typeparam name="TQueryResult"></typeparam>
	public interface ICacheSingleItemsInCollection<TQueryModel,TQueryResult>
		where TQueryResult : class, IQueryResult
		where TQueryModel : IQueryModel
	{
		IEnumerable<CacheSingleItemsInCollectionResult<TQueryModel, TQueryResult>> Resolve(TQueryModel query,
			IEnumerable<TQueryResult> resultsInCollection);
	}
	public class CacheSingleItemsInCollectionResult<TQueryModel,TQueryResult>
		where TQueryResult : class, IQueryResult
		where TQueryModel : IQueryModel
	{
		public CacheSingleItemsInCollectionResult(TQueryModel query, TQueryResult value)
		{
			Query = query;
			Value = value;
		}

		public TQueryModel Query
		{
			get;
		}

		public TQueryResult Value
		{
			get;
		}
	}
}
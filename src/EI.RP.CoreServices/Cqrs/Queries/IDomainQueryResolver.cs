using System.Collections.Generic;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Queries
{
	public interface IDomainQueryResolver
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TQueryModel"></typeparam>
		/// <typeparam name="TQueryResult"></typeparam>
		/// <param name="query"></param>
		/// <param name="byPassPipeline">when true it bypasses validations,events,...  it usually means that is called from the very same domain</param>
		/// <returns></returns>
		Task<IEnumerable<TQueryResult>> FetchAsync<TQueryModel, TQueryResult>(TQueryModel query,bool byPassPipeline=false)
			where TQueryResult : class, IQueryResult
			where TQueryModel : IQueryModel;
	}

	
}

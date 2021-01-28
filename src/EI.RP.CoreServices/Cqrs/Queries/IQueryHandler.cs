using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Queries
{
    public interface IQueryHandler<in TQueryModel> where TQueryModel: IQueryModel
	{
        Task<IEnumerable<TQueryResult>> ExecuteQueryAsync<TQueryResult>(TQueryModel query) where TQueryResult :  class,IQueryResult;
    }
}

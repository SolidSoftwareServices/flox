using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Queries
{
	public abstract class QueryHandler<TQueryModel> : IQueryHandler<TQueryModel> where TQueryModel : IQueryModel
	{
		public async Task<IEnumerable<TQueryResult>> ExecuteQueryAsync<TQueryResult>(TQueryModel query) where TQueryResult :class, IQueryResult

		{
			if (!ValidQueryResultTypes.Contains(typeof(TQueryResult)))
			{
				throw new InvalidOperationException($"Requested {typeof(TQueryResult).FullName} is not currently supported as a result");
			}

			if (!query.IsValidQuery(out string [] notValidReasons))
			{
				if(notValidReasons==null || !notValidReasons.Any(x=>!string.IsNullOrWhiteSpace(x)))
				{
					throw new NotImplementedException("Must specify at least one reason why the query is not valid.");
				}
				throw new InvalidOperationException($"Query {typeof(TQueryModel).FullName} is not valid: {string.Join(Environment.NewLine,notValidReasons)}");
			}
            return (await _ExecuteQueryAsync<TQueryResult>(query)).ToArray();
        }

		protected abstract Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(TQueryModel query)
			where TQueryResult : class;
		protected abstract Type[] ValidQueryResultTypes { get; }
	}
}
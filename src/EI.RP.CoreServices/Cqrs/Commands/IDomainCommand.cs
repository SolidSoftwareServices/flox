using System.Collections.Generic;
using EI.RP.CoreServices.Cqrs.Queries;
using NotImplementedException = System.NotImplementedException;

namespace EI.RP.CoreServices.Cqrs.Commands
{
    public interface IDomainCommand
	{
		/// <summary>
		/// It provides the the queries to invalidate ONLY if the commands executes successfully
		/// </summary>
		/// <remarks>To invalidate a query, means that the next time this query will be resolved fresh and not from the cache</remarks>
		/// <remarks>Errored commands invalidate all the user's cache</remarks>
		IEnumerable<IQueryModel> InvalidateQueriesOnSuccess { get; }
		bool InvalidatesCache { get; }
    }

    public abstract class DomainCommand : IDomainCommand
    {
	    public virtual IEnumerable<IQueryModel> InvalidateQueriesOnSuccess { get; } = new IQueryModel[0];

	    public virtual bool InvalidatesCache { get; } = true;
    }
}
using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.CoreServices.Cqrs.Events
{
    public interface IQueryEventBuilder<TQuery> where TQuery : IQueryModel

    {
        Task<IEventApiMessage> BuildEventOnSuccess(TQuery query);
        Task<IEventApiMessage> BuildEventOnError(TQuery query, AggregateException exceptions);
    }
}
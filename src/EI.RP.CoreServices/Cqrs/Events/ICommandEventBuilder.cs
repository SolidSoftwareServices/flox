using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.CoreServices.Cqrs.Events
{
    /// <summary>
    /// This class defines the events to publish when processing a command or a query
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <remarks>implementing this class for a command is not mandatory</remarks>
    public interface ICommandEventBuilder<TCommand> where TCommand:IDomainCommand
    {
        Task<IEventApiMessage[]> BuildEventsOnSuccess(TCommand command);
        Task<IEventApiMessage[]> BuildEventsOnError(TCommand command, AggregateException exceptions);
    }
}
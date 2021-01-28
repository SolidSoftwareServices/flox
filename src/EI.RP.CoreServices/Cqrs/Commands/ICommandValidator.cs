using System;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Commands
{
    /// <summary>
    /// It validates the command before the dispatcher executes it. Throwing exceptions
    /// </summary>
    /// <remarks>Since they are not always needed validators are optional</remarks>
    public interface ICommandValidator<in TCommand> where TCommand : IDomainCommand
    {
        /// <summary>
        /// It validates a command and throws if validation errors
        /// </summary>
        /// <param name="command"></param>
        /// <remarks>It is expected that EVERY validation HAPPENS HERE</remarks>
        /// <returns>The Errors or an empty list</returns>
        /// <exception cref="AggregateException"></exception>
        Task ValidateAsync(TCommand command);

    }
}
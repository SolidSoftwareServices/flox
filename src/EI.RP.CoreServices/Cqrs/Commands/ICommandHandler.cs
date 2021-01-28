using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Commands
{
    public interface ICommandHandler<in TCommand>
        where TCommand : IDomainCommand
    {
        /// <summary>
        /// it executes the desired command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task ExecuteAsync(TCommand command);
    }
}
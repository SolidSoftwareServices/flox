using System;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Cqrs.Commands
{
    public interface IDomainCommandDispatcher
    {
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TCommand"></typeparam>
		/// <param name="command"></param>
		/// /// <param name="byPassPipeline">when true it bypasses validations,events,... it usually means that is called from the very same domain</param>
		/// <returns></returns>
		/// <exception cref="AggregateException">Something happened when proccessing the command see inner errors</exception>
		Task ExecuteAsync<TCommand>(TCommand command, bool byPassPipeline = false) where TCommand : IDomainCommand;
    }
}

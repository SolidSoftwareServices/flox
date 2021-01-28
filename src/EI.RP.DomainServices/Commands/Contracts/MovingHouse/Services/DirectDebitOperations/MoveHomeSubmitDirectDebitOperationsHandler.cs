using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.DirectDebitOperations
{
	class MoveHomeSubmitDirectDebitOperationsHandler : IMoveHomeSubmitDirectDebitOperationsHandler
	{
		private readonly IDomainCommandDispatcher _commandsDispatcher;

		public MoveHomeSubmitDirectDebitOperationsHandler(IDomainCommandDispatcher commandsDispatcher)
		{
			_commandsDispatcher = commandsDispatcher;
		}

		public async Task Submit(MoveHouse commandData)
		{
			var tasks = commandData.CommandsToExecute
				.Select(x => _commandsDispatcher.ExecuteAsync(x, true))
				.ToArray();

			if (tasks.Any())
			{
				await Task.WhenAll(tasks);
			}

			commandData.Context.CheckPoints.SetUpNewDirectDebitsCompleted_4 = true;
		}
	}
}

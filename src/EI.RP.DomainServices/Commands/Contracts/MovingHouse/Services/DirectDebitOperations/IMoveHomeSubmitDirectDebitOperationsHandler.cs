using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.DirectDebitOperations
{
	interface IMoveHomeSubmitDirectDebitOperationsHandler
	{
		Task Submit(MoveHouse commandData);
	}
}
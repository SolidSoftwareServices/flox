using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ActivityOperations
{
	interface IMoveHomeActivityPublisher
	{
		Task SubmitActivityError(MoveHouse commandData, DomainException ex);
	}
}

using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context
{
	internal interface ICompleteMovingHouseContextFactory
	{
		Task<CompleteMoveHouseContext> Resolve(MoveHouse commandData);

	}
}
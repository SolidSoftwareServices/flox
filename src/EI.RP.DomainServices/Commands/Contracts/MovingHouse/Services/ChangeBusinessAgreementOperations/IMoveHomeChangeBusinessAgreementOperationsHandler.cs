using System.Threading.Tasks;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ChangeBusinessAgreementOperations
{
	interface IMoveHomeChangeBusinessAgreementOperationsHandler
	{
		Task SetNewAddressAndBusinessAgreementChanges(MoveHouse commandData);
	}
}
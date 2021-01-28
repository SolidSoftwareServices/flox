using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.MoveInOperationsHandler
{
	interface IMoveHomeInOperationsHandler
	{
		Task SubmitMoveInMeterReadings(MoveHouse commandData, ContractSaleDto contractSale);
		
	}
}
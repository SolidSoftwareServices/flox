using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;


namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.MoveOutOperationsHandler
{
	interface IMoveHomeOutOperationsHandler
	{
		Task SubmitMoveOutMeterReadings(MoveHouse commandData, ContractSaleDto contractSale);
		Task StoreIncommingOccupant(MoveHouse commandData);
	}
}
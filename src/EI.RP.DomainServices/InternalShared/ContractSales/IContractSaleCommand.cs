using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;

namespace EI.RP.DomainServices.InternalShared.ContractSales
{
	internal interface IContractSaleCommand
	{
		Task<ContractSaleDto> ExecuteAddGasContractSale(string electricityAccountNumber, PaymentSetUpType paymentSetup,
			GasPointReferenceNumber prn, decimal meterReading,
			Premise electricityPremise, 
			string iban, string nameOnBankAccount, PointOfDeliveryInfo pointOfDelivery);

		Task<ContractSaleDto> ResolveContractSaleChecks(MoveHouse command, bool autobatch = true);
		Task<ContractSaleDto> ExecuteContractSale(MoveHouse commandData);
		Task<ContractSaleDto> ExecuteChangeSmartPlanToStandardContractSale(string electricityAccountNumber);

		Task<ContractSaleDto> ActivateSmartMeter(ActivateSmartMeterCommand commandData);
	}
}
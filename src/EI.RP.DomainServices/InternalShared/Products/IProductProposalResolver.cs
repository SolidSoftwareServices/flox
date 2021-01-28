using System;
using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;

namespace EI.RP.DomainServices.InternalShared.Products
{	
	internal interface IProductProposalResolver
	{
		[Obsolete("TODO: unify calls")]
		Task<ProductProposalResultDto> GetAddGasProductProposal(string electricityAccountNumber,
			PaymentSetUpType paymentSetup,
			PointOfDeliveryInfo pointOfDelivery,
			GasPointReferenceNumber newGprn,
			Premise electricityPremise);

		Task<ProductProposalResultDto> GetMoveHouseProductProposal(MoveHouse command);

		Task<ProductProposalResultDto> ChangeSmartToStandardProductProposal(string electricityAccountNumber, string gasAccountNumber);

		Task<ProductProposalResultDto> GetActivateSmartProductProposal(ActivateSmartMeterCommand command, string gasAccountNumber);
	}
}
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.InternalShared.ContractSales;
using EI.RP.DomainServices.InternalShared.MeterReading;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition;
using EI.RP.DomainServices.InternalShared.PointOfDelivery;

namespace EI.RP.DomainServices.Commands.Contracts.AddAdditionalAccount.Gas
{
	internal class AddGasAccountCommandHandler : ICommandHandler<AddGasAccountCommand>
	{
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private readonly IContractSaleCommand _contractSaleBuilder;
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;
		private readonly ISubmitMeterReadings _meterReadingsSubmitter;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IPointOfDeliveryCommand _pointOfDeliveryBuilder;

		public AddGasAccountCommandHandler(ISapRepositoryOfCrmUmc crmUmcRepository, IDomainQueryResolver queryResolver,
			IDomainCommandDispatcher commandDispatcher, ISubmitMeterReadings meterReadingsSubmitter,
			IContractSaleCommand contractSaleBuilder, IPointOfDeliveryCommand pointOfDeliveryBuilder)
		{
			_crmUmcRepository = crmUmcRepository;
			_queryResolver = queryResolver;
			_commandDispatcher = commandDispatcher;
			_meterReadingsSubmitter = meterReadingsSubmitter;
			_contractSaleBuilder = contractSaleBuilder;
			_pointOfDeliveryBuilder = pointOfDeliveryBuilder;
		}

		public async Task ExecuteAsync(AddGasAccountCommand command)
		{
			var getElectricityContract = _crmUmcRepository.NewQuery<BusinessAgreementDto>()
				.Key(command.BaseElectricityAccountNumber)
				.NavigateTo<ContractItemDto>()
				.Expand(x => x.Premise.PointOfDeliveries)
				.Expand(x => x.BusinessAgreement)
				.GetOne();

			var getPointOfDelivery = GetOrAddPointOfDelivery(command);
			var electricityContract = await getElectricityContract;
			var getElectricityPremise = _queryResolver.GetPremise(electricityContract.PremiseID, true);
			var pointOfDelivery = await getPointOfDelivery;
			var isNewAcquisition = await _queryResolver.IsPrnNewAcquisition(command.GPRN, pointOfDelivery.IsNewAcquisition);
	
			await _meterReadingsSubmitter.SubmitGasMeterReading(command.BaseElectricityAccountNumber,
				isNewAcquisition, command.GPRN, command.MeterReading, true);

			await _contractSaleBuilder.ExecuteAddGasContractSale(command.BaseElectricityAccountNumber,
				command.PaymentSetUp, command.GPRN, command.MeterReading, 
				await getElectricityPremise, command.IBAN, command.NameOnBankAccount, pointOfDelivery);
			
			//TODO: Events AddGas.cs #72
		}

		private async Task<PointOfDeliveryInfo> GetOrAddPointOfDelivery(AddGasAccountCommand commandData)
		{
			var pointOfDelivery =
				await _queryResolver.GetPointOfDeliveryInfoByPrn(commandData.GPRN, true);

			if (pointOfDelivery == null)
			{
				pointOfDelivery = await _pointOfDeliveryBuilder.AddPointOfDelivery(commandData.GPRN, 
														ClientAccountType.Gas,
														usePremisesAddressOfAccountNumber: commandData.BaseElectricityAccountNumber);
			}

			return pointOfDelivery;
		}
	}
}
using System.Linq;
using EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.Steps
{
    public class ConfirmAddress : AddGasAccountScreen
    {
	    public static class StepEvent
		{
			public static readonly ScreenEvent ConfirmNewAddress = new ScreenEvent(nameof(ConfirmAddress),nameof(ConfirmNewAddress));
		}
		public override ScreenName ScreenStep => AddGasAccountStep.ConfirmAddress;
        private readonly IDomainQueryResolver _domainQueryResolver;

        public ConfirmAddress(IDomainQueryResolver domainQueryResolver)
        {
            _domainQueryResolver = domainQueryResolver;
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<AddGasAccountFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var stepData = new ScreenModel
            {
	            AccountNumber = rootData.ElectricityAccountNumber
            };

            SetTitle(Title, stepData);

            var collectAccountConsumptionStepData = contextData.GetStepData<CollectAccountConsumptionDetails.ScreenModel>();
            stepData.GPRN = collectAccountConsumptionStepData.GPRN;

            var pointOfDeliveryInfo = await _domainQueryResolver.GetPointOfDeliveryInfoByPrn((GasPointReferenceNumber)stepData.GPRN);
            if (pointOfDeliveryInfo?.AddressInfo != null)
            {
                stepData.HouseNo = pointOfDeliveryInfo.AddressInfo.HouseNo;
                stepData.City = pointOfDeliveryInfo.AddressInfo.City;
                stepData.Street = pointOfDeliveryInfo.AddressInfo.Street;
                stepData.PostalCode = pointOfDeliveryInfo.AddressInfo.PostalCode;
            }
            else
            {
                var account = await _domainQueryResolver.GetAccountInfoByAccountNumber(stepData.AccountNumber);
                var premiseInfo = account.BusinessAgreement.Contracts.FirstOrDefault()?.Premise;
                var addressInfo = premiseInfo?.Address;
                stepData.HouseNo = addressInfo?.HouseNo;
                stepData.City = addressInfo?.City;
                stepData.Street = addressInfo?.Street;
                stepData.PostalCode = addressInfo?.PostalCode;
            }

            return stepData;
        }

       

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventNavigatesTo(StepEvent.ConfirmNewAddress, AddGasAccountStep.ExecutePaymentConfigurationFlowThenStoreResults);
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public string AccountNumber { get; set; }
            public string GPRN { get; set; }

            public string HouseNo { get; set; }

            public string Street { get; set; }

            public string City { get; set; }

            public string PostalCode { get; set; }
        }
    }
}

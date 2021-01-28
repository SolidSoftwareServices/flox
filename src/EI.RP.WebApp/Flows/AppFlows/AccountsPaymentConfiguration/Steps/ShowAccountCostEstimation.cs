using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Billing;
using EI.RP.DomainServices.Queries.Billing.NextBill;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
    public class ShowAccountCostEstimation : AccountsPaymentConfigurationScreen
    {
	    private const string Title = "Estimate your costs";
	    
	    private readonly IDomainQueryResolver _domainQueryResolver;

	    public ShowAccountCostEstimation(IDomainQueryResolver domainQueryResolver)
		{
			_domainQueryResolver = domainQueryResolver;
		}

        public static class StepEvent
        {
            public static readonly ScreenEvent MakeAPayment = new ScreenEvent(nameof(ShowAccountCostEstimation),nameof(MakeAPayment));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
        }


        public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.ShowAccountCostEstimation;


        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);

            var accountNumber = rootData.CurrentAccount().Account.AccountNumber;
            var estimation = await _domainQueryResolver.GetNextBillEstimationByAccountNumber(accountNumber);
            var data = Map(estimation);

            SetTitle(Title, data);

            return data;
        }

        private EstimateYourCostData Map(NextBillEstimation domainModel)
        {
	        return new EstimateYourCostData
	        {
		        AccountNumber = domainModel.AccountNumber, 
		        EstimatedAmount = domainModel.EstimatedAmount
	        };
        }

        public class EstimateYourCostData : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == AccountsPaymentConfigurationStep.ShowAccountCostEstimation;
            }            
            public string AccountNumber { get; set; }

            public EuroMoney EstimatedAmount { get; set; }
        }
    }
}
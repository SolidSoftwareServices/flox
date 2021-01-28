using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountDashboard.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps;
using EI.RP.WebApp.Flows.AppFlows.TermsInfo.Steps;
using EI.RP.WebApp.Flows.AppFlows.UserContactDetails.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Steps
{
	class LoadAccountDashboard : CustomerAccountsScreen
	{
		public override ScreenName ScreenStep => CustomerAccountsStep.LoadAccountDashboard;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>();
			var input = new AccountDashboard.Steps.AccountDashboardFlowInitializer.RootScreenModel();

			var data = (IAccountDashboardFlowInput) input;
			data.AccountNumber = rootData.SelectedUserAccountNumber;
			
			ResolveDashboard();
			var connectToFlow = new ConnectToFlow(
				ResidentialPortalFlowType.AccountDashboard.ToString(),
				input
			);
			
			return connectToFlow;

			void ResolveDashboard()
			{
				if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToContactUs))
				{
					data.InitialFlow = ResidentialPortalFlowType.ContactUs;
				}
				if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToHelp))
				{
					data.InitialFlow = ResidentialPortalFlowType.Help;
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToEditDirectDebit))
				{
					data.InitialFlow = ResidentialPortalFlowType.AccountsPaymentConfiguration;
					data.InitialFlowStartType = AccountsPaymentConfigurationFlowStartType.EditDirectDebit.ToString();
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToMakeAPayment))
				{
					data.InitialFlow = ResidentialPortalFlowType.MakeAPayment;
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToEstimateCost))
				{
					data.InitialFlow = ResidentialPortalFlowType.AccountsPaymentConfiguration;
					data.InitialFlowStartType = AccountsPaymentConfigurationFlowStartType.EstimateYourCost.ToString();
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToDisclaimer))
				{
					data.InitialFlow = ResidentialPortalFlowType.TermsInfo;
					data.InitialFlowStartType = TermsInfoFlowInitializer.StartType.Disclaimer.ToString();
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToPrivacy))
				{
					data.InitialFlow = ResidentialPortalFlowType.TermsInfo;
					data.InitialFlowStartType = TermsInfoFlowInitializer.StartType.Privacy.ToString();
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToTermsAndConditions))
				{
					data.InitialFlow = ResidentialPortalFlowType.TermsInfo;
					data.InitialFlowStartType = TermsInfoFlowInitializer.StartType.TermsAndConditions.ToString();
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToChangePassword))
				{
					data.InitialFlow = ResidentialPortalFlowType.ChangePassword;
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToRequestRefund))
				{
					data.InitialFlow = ResidentialPortalFlowType.RequestRefund;
				}				
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToMarketingPreference))
                {
                    data.InitialFlow = ResidentialPortalFlowType.UserContactDetails;
                    data.InitialFlowStartType = UserContactFlowType.MarketingPreferences.ToString();

                }
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToProductsAndServices))
                {
                    data.InitialFlow = ResidentialPortalFlowType.ProductAndServices;
                }
                else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToUsage))
                {
                    data.InitialFlow = ResidentialPortalFlowType.Usage;
                }
				else if (
					contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.SubmitMeterReading) ||
					rootData.UiFlowInitiator != null &&
					 rootData.UiFlowInitiator == AppLoginType.MeterReading)
				{
					data.InitialFlow = ResidentialPortalFlowType.MeterReadings;
					data.InitialFlowStartType =
						MeterReadingFlowInitializer.StartType.ShowAndEditMeterReading.ToString();
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToCompetition))
				{
					data.InitialFlow = ResidentialPortalFlowType.CompetitionEntry;
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToPromotion))
				{
					data.InitialFlow = ResidentialPortalFlowType.PromotionEntry;
				}
				else if (contextData.CurrentEvents.Any(x => x == AccountSelection.StepEvent.ToSmartActivation))
				{
					data.InitialFlow = ResidentialPortalFlowType.SmartActivation;
					data.SourceFlowType = ResidentialPortalFlowType.Accounts;
				}
			}
		}
	}
}
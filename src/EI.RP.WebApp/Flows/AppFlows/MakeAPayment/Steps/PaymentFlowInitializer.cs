using EI.RP.CoreServices.Http.Session;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MakeAPayment.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.MakeAPayment.Steps
{
	public class PaymentFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType,
		PaymentFlowInitializer.RootScreenModel>
	{
		public enum StartType
		{
			Default = 0,
			FromEstimateCost,
			FromLastBill
		}

		private readonly IUserSessionProvider _userSessionProvider;

		public PaymentFlowInitializer(IUserSessionProvider userSessionProvider)
		{
			_userSessionProvider = userSessionProvider;
		}

		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.MakeAPayment;

		public override bool Authorize()
		{
			return !_userSessionProvider.IsAnonymous();
		}

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
		{
			return preStartCfg
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(ScreenEvent.Start, PaymentStep.PaymentGatewayInteractionAndMainOptions);
		}

		public static class StepEvent
		{ }

		public class RootScreenModel : InitialFlowScreenModel, IMakeAPaymentInput
		{
			public string AccountNumber { get; set; }
			public StartType StartType { get; set; }
		}
	}
}
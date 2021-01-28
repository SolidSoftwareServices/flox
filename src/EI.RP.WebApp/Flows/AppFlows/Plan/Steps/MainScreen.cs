using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions;
using EI.RP.DomainServices.Commands.Contracts.ChangeSmartPlanToStandard;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Steps
{
	public class MainScreen : PlanScreen
	{
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IDomainCommandDispatcher _commandsDispatcher;

		public MainScreen(IDomainQueryResolver queryResolver, IDomainCommandDispatcher commandsDispatcher)
		{
			_queryResolver = queryResolver;
			_commandsDispatcher = commandsDispatcher;
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent SwitchOffPaperBill =
				new ScreenEvent(nameof(MainScreen), nameof(SwitchOffPaperBill));

			public static readonly ScreenEvent SwitchOnPaperBill =
				new ScreenEvent(nameof(MainScreen), nameof(SwitchOnPaperBill));

			public static readonly ScreenEvent SwitchMonthlyBilling =
				new ScreenEvent(nameof(MainScreen), nameof(SwitchMonthlyBilling));

			public static readonly ScreenEvent SwitchBiMonthlyBilling =
				new ScreenEvent(nameof(MainScreen), nameof(SwitchBiMonthlyBilling));

			public static readonly ScreenEvent SwitchOffMeterData =
				new ScreenEvent(nameof(MainScreen), nameof(SwitchOffMeterData));
		}

		public override ScreenName ScreenStep => PlanStep.MainScreen;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventsReentriesCurrent(new ScreenEvent[]
			{
				StepEvent.SwitchMonthlyBilling,
				StepEvent.SwitchBiMonthlyBilling,
				StepEvent.SwitchOffPaperBill,
				StepEvent.SwitchOnPaperBill,
				StepEvent.SwitchOffMeterData
			});
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var screenModel = new ScreenModel
			{
				AccountNumber = contextData.GetStepData<PlanFlowInitializer.RootScreenModel>().AccountNumber
			};

			SetTitle(Title, screenModel);

			return screenModel;
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			var stepData = contextData.GetCurrentStepData<ScreenModel>();
			
			await HandlePaperBillChoiceEvent();
			await HandleSetBillingPeriod();
			await HandleMeterDataEvent();
			
			async Task HandleSetBillingPeriod()
			{
				if (triggeredEvent == StepEvent.SwitchMonthlyBilling)
				{
					await _commandsDispatcher.ExecuteAsync(new SetMonthlyBillingPeriodCommand(stepData.AccountNumber, stepData.MonthlyBillingDayOfTheMonth));
				}
				else if (triggeredEvent == StepEvent.SwitchBiMonthlyBilling)
				{
					await _commandsDispatcher.ExecuteAsync(new SetBiMonthlyBillingPeriodCommand(stepData.AccountNumber));
				}
			}

			async Task HandlePaperBillChoiceEvent()
			{
				if (triggeredEvent.IsOneOf(StepEvent.SwitchOffPaperBill, StepEvent.SwitchOnPaperBill))
				{

					await _commandsDispatcher.ExecuteAsync(
						new ChangePaperBillChoiceCommand(stepData.AccountNumber,
							triggeredEvent == StepEvent.SwitchOffPaperBill
								? PaperBillChoice.Off
								: PaperBillChoice.On)
					);
				}

				if (triggeredEvent == StepEvent.SwitchOffPaperBill)
				{
					var getAccountInfo = _queryResolver.GetAccountInfoByAccountNumber(stepData.AccountNumber);
					var getBillingInfo = _queryResolver.GetAccountBillingInfoByAccountNumber(stepData.AccountNumber);
					var accountInfo = await getAccountInfo;
					if (accountInfo.IsElectricityAccount() && accountInfo.IsSmart() && (await getBillingInfo).MonthlyBilling.IsMonthlyBillingActive)
					{
						await _commandsDispatcher.ExecuteAsync(new SetBiMonthlyBillingPeriodCommand(stepData.AccountNumber));
					}
				}
			}

			async Task HandleMeterDataEvent()
			{
				if (triggeredEvent == StepEvent.SwitchOffMeterData)
				{
					await _commandsDispatcher.ExecuteAsync(new ChangeSmartPlanToStandardCommand(stepData.AccountNumber));
					stepData.IsMoveToStandardPlanRequestSendSuccesfully = true;
				}
			}
		}

		public sealed class ScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == PlanStep.MainScreen;
			}

			public string AccountNumber { get; set; }

			public int MonthlyBillingDayOfTheMonth { get; set; }

			public bool ShouldValidateMeterData { get; set; } = false;

			[RequiredIf(nameof(ShouldValidateMeterData), IfValue = true, ErrorMessage = "You must check this box to proceed")]
			public bool NoAccessToFeatures { get; set; }

			[RequiredIf(nameof(ShouldValidateMeterData), IfValue = true, ErrorMessage = "You must check this box to proceed")]
			public bool MovedToStandardPlan { get; set; }

			[RequiredIf(nameof(ShouldValidateMeterData), IfValue = true, ErrorMessage = "You must accept the terms and conditions to proceed")]
			public bool AgreeTermsAndConditions { get; set; }

			public bool IsMoveToStandardPlanRequestSendSuccesfully { get; set; }
		}
	}
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
	public class ConfirmationOfChanges : AccountsPaymentConfigurationScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent BackToBillingAndPaymentOptions =
				new ScreenEvent(nameof(ConfirmationOfChanges),nameof(BackToBillingAndPaymentOptions));
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.BackToBillingAndPaymentOptions,
					AccountsPaymentConfigurationStep.SetupDirectDebitWithPaymentOptions);
		}

		public override ScreenName ScreenStep => AccountsPaymentConfigurationStep.ConfirmationOfChangesStep;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var lastStepData = contextData.GetStepData<InputDirectDebitDetails.ScreenModel>();
			
			var inputAccount = rootData.CurrentAccount();

			var stepData = new DirectDebitSettingsUpdateConfirmationData
			{
				FlowHandler = contextData.FlowHandler,
				AccountNumber = inputAccount.Account.AccountNumber,
				ConfirmationViewMode = ResolveConfirmationViewMode(),
				AccountType = inputAccount.Account.ClientAccountType
			};

			SetTitle(ResolveTitle(), stepData);

			return stepData;

			DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode ResolveConfirmationViewMode()
			{
				DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode result;
				if (lastStepData != null)
				{
					switch (lastStepData.ViewMode)
					{
						case InputDirectDebitDetails.ScreenModel.StepMode.DefaultSetUp:
							result = DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.DefaultSetUp;
							break;
						case InputDirectDebitDetails.ScreenModel.StepMode.DefaultEdit:
							result = DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.DefaultEdit;
							break;
						case InputDirectDebitDetails.ScreenModel.StepMode.EqualizerSetup:
							result = DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.EqualizerSetup;
							break;
						case InputDirectDebitDetails.ScreenModel.StepMode.GasAccountSetUp:
							result = DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.GasAccountSetUp;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else if (rootData.StartType == AccountsPaymentConfigurationFlowStartType.UseExistingAccountDirectDebit)
					result= DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.GasAccountSetUp;
				else
					result =DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.None;
				return result;
			}

			string ResolveTitle()
			{
				switch (stepData.ConfirmationViewMode)
				{
					case DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.DefaultSetUp:
					case DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.DefaultEdit:
						return "Direct Debit Settings";
					case DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.EqualizerSetup:
						return "Equal Monthly Payments";
					case DirectDebitSettingsUpdateConfirmationData.ConfirmationStepMode.GasAccountSetUp:
						return "Add Gas";
					default:
						return null;
				}
			}
		}


		public class DirectDebitSettingsUpdateConfirmationData : UiFlowScreenModel
		{

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == AccountsPaymentConfigurationStep.ConfirmationOfChangesStep;
			}

			public string AccountNumber { get; set; }
			public ClientAccountType AccountType { get; set; }

			public enum ConfirmationStepMode
			{
				None = 0,
				DefaultSetUp = 1,
				DefaultEdit,
				EqualizerSetup,
				GasAccountSetUp
			}

			public ConfirmationStepMode ConfirmationViewMode { get; set; }
		}
	}
}
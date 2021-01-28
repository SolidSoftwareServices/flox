using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.DomainServices.Queries.Metering.Premises;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class Step5ConfirmationScreen : MovingHouseScreen
	{
		protected string StepTitle => string.Join(" | ", "Confirmation", Title);

		private readonly IDomainQueryResolver _queryResolver;

		public Step5ConfirmationScreen(IDomainQueryResolver queryResolver, IConfigurableTestingItems configurableTestingItems)
		{
			_queryResolver = queryResolver;
			_configurableTestingItems = configurableTestingItems;
		}

		public override ScreenName ScreenStep => MovingHouseStep.Step5ReviewConfirmation;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError);
		}
		private readonly IConfigurableTestingItems _configurableTestingItems;
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var screenModel = await BuildStepData(contextData, new ScreenModel());

			SetTitle(StepTitle, screenModel);

			return screenModel;
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(
			IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var screenModel = await BuildStepData(contextData, (ScreenModel)originalScreenModel);

			SetTitle(StepTitle, screenModel);

			return screenModel;
		}

		private async Task<ScreenModel> BuildStepData(IUiFlowContextData contextData, ScreenModel screenModel)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var step0StepData = contextData.GetStepData<Step0OperationSelection.ScreenModel>();
			var step5StepData = contextData.GetStepData<Step5ReviewAndComplete.ScreenModel>();

			screenModel.InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber;
			screenModel.ElectricityAccountNumber = rootData.ElectricityAccountNumber;
			screenModel.GasAccountNumber = rootData.GasAccountNumber;
			screenModel.HasFreeElectricityAllowance = step5StepData.HasFreeElectricityAllowance;

			if (step0StepData.MovingHouseType == MovingHouseType.MoveElectricity ||
				step0StepData.MovingHouseType == MovingHouseType.MoveElectricityAndAddGas ||
				step0StepData.MovingHouseType == MovingHouseType.MoveElectricityAndGas)
			{
				var accountBilling = await _queryResolver.GetInvoicesByAccountNumber(screenModel.ElectricityAccountNumber);
				var invoice = accountBilling.FirstOrDefault(x =>
								x.OriginalDate == DateTime.Today && x.IsBill());
				if (invoice != null)
				{
					var electricityAccountBalanceInfo = await _queryResolver.GetAccountBillingInfoByAccountNumber(screenModel.ElectricityAccountNumber);
					screenModel.ElectricityAccountBalance = electricityAccountBalanceInfo?.CurrentBalanceAmount?.Amount?.ToString() ?? string.Empty;
				}
			}

			if (step0StepData.MovingHouseType == MovingHouseType.MoveGas ||
				step0StepData.MovingHouseType == MovingHouseType.MoveGasAndAddElectricity ||
				step0StepData.MovingHouseType == MovingHouseType.MoveElectricityAndGas)
			{
				var accountBilling = await _queryResolver.GetInvoicesByAccountNumber(screenModel.GasAccountNumber);
				var invoice = accountBilling.FirstOrDefault(x =>
					x.OriginalDate == DateTime.Today && x.IsBill());
				if (invoice != null)
				{
					var gasAccountBalanceInfo = await _queryResolver.GetAccountBillingInfoByAccountNumber(screenModel.GasAccountNumber);
					screenModel.GasAccountBalance = gasAccountBalanceInfo?.CurrentBalanceAmount?.Amount?.ToString() ?? string.Empty;
				}
			}

			var paymentResult = contextData.GetStepData<Step4ConfigurePayment.StepData>().CalledFlowResult;
			screenModel.PaymentInfo = paymentResult.ConfigurationSelectionResults;
			screenModel.ElectricityTargetPaymentSetupType = GetTargetPaymentSetupType(screenModel.PaymentInfo, ClientAccountType.Electricity);
			screenModel.GasTargetPaymentSetupType = GetTargetPaymentSetupType(screenModel.PaymentInfo, ClientAccountType.Gas);
			screenModel.IsElectricityAccountSwitchedToDirectDebit = IsAccountSwitchedToDirectDebit(screenModel.PaymentInfo, ClientAccountType.Electricity);
			screenModel.IsGasAccountSwitchedToDirectDebit = IsAccountSwitchedToDirectDebit(screenModel.PaymentInfo, ClientAccountType.Gas);
			screenModel.MovingHouseType = step0StepData.MovingHouseType;
			return screenModel;

		}

		private bool IsAccountSwitchedToDirectDebit(
			IEnumerable<AccountsPaymentConfigurationResult.AccountConfigurationInfo> accountsPaymentConfiguration,
			ClientAccountType forClientAccountType)
		{
			var paymentInfo = accountsPaymentConfiguration.FirstOrDefault(pInfo => pInfo.TargetAccountType == forClientAccountType);
			if (paymentInfo == null ||
			   paymentInfo.SelectedPaymentSetUpType != PaymentSetUpType.SetUpNewDirectDebit
			   || paymentInfo.Account == null)
			{
				return false;
			}

			return paymentInfo.Account.PaymentMethod == PaymentMethodType.Manual;
		}

		private PaymentSetUpType? GetTargetPaymentSetupType(
			IEnumerable<AccountsPaymentConfigurationResult.AccountConfigurationInfo> paymentInfo,
			ClientAccountType forClientAccountType)
		{
			return paymentInfo.FirstOrDefault(pInfo => pInfo.TargetAccountType == forClientAccountType)?.SelectedPaymentSetUpType;
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public IEnumerable<AccountsPaymentConfigurationResult.AccountConfigurationInfo> PaymentInfo { get; set; }
			public bool? HasFreeElectricityAllowance { get; set; }
			public MovingHouseType MovingHouseType { get; set; }
			public PaymentSetUpType? ElectricityTargetPaymentSetupType { get; set; }
			public PaymentSetUpType? GasTargetPaymentSetupType { get; set; }
			public string InitiatedFromAccountNumber { get; set; }
			public string ElectricityAccountNumber { get; set; }
			public string GasAccountNumber { get; set; }
			public bool IsElectricityAccountSwitchedToDirectDebit { get; set; }
			public bool IsGasAccountSwitchedToDirectDebit { get; set; }
			public string ElectricityAccountBalance { get; set; }
			public string GasAccountBalance { get; set; }
		}
	}
}
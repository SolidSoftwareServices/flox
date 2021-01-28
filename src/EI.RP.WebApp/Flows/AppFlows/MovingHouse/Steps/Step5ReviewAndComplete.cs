using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using Newtonsoft.Json;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using NLog;
using EI.RP.DataServices;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class Step5ReviewAndComplete : MovingHouseScreen
	{
		protected string StepTitle => string.Join(" | ", "5. Review", Title);
		private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;
		

		public Step5ReviewAndComplete(IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
		{
			_domainQueryResolver = domainQueryResolver;
			_domainCommandDispatcher = domainCommandDispatcher;
		}
		
		public override ScreenName ScreenStep => MovingHouseStep.Step5ReviewAndComplete;

		public static class StepEvent
		{
			public static readonly ScreenEvent ToStep1 = new ScreenEvent(nameof(Step5ReviewAndComplete),nameof(ToStep1));
			public static readonly ScreenEvent ToStep2 = new ScreenEvent(nameof(Step5ReviewAndComplete), nameof(ToStep2));
			public static readonly ScreenEvent ToStep3 = new ScreenEvent(nameof(Step5ReviewAndComplete), nameof(ToStep3));
			public static readonly ScreenEvent ToPaymentOptions = new ScreenEvent(nameof(Step5ReviewAndComplete), nameof(ToPaymentOptions));
			public static readonly ScreenEvent RequestCompleteMoveHouse = new ScreenEvent(nameof(Step5ReviewAndComplete), nameof(RequestCompleteMoveHouse));
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError)
				.OnEventNavigatesTo(StepEvent.ToStep1, MovingHouseStep.Step1InputMoveOutPropertyDetails)
				.OnEventNavigatesTo(StepEvent.ToStep2, MovingHouseStep.Step2InputPrns)
				.OnEventNavigatesTo(StepEvent.ToStep3, MovingHouseStep.Step3InputMoveInPropertyDetails)
				.OnEventNavigatesTo(StepEvent.ToPaymentOptions, MovingHouseStep.Step4ConfigurePayment)
				.OnEventNavigatesTo(
					StepEvent.RequestCompleteMoveHouse,
					MovingHouseStep.Step5ReviewConfirmation,
					() => !HasValidationErrors(contextData),
					"Business Rules Passed")
				.OnEventNavigatesTo(
					StepEvent.RequestCompleteMoveHouse, 
					MovingHouseStep.ShowMovingHouseValidationError,
					() => HasValidationErrors(contextData),
					"Business Rules Failed");
		}

		private bool HasValidationErrors(IUiFlowContextData contextData)
		{
			var stepData = contextData.GetStepData<ScreenModel>();
			return stepData.HasValidationErrors == true;
		}

        async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var query = new MovingHouseValidationQuery
            {
                ElectricityAccountNumber = rootData.ElectricityAccountNumber,
                GasAccountNumber = rootData.GasAccountNumber,
            };

            var validation = await _domainQueryResolver.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
            return validation.Any(x => x.Output == OutputType.Failed);
        }
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			Log.Debug($"Step5Review OnCreateStepDataAsync called.");

			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>();
			var paymentResult = contextData.GetStepData<Step4ConfigurePayment.StepData>().CalledFlowResult;
			if (paymentResult.Exit == AccountsPaymentConfigurationResult
				.ExitType.UserCancelled)
			{
				return new ConnectToFlow(ResidentialPortalFlowType.MovingHouse.ToString(),
					new FlowInitializer.RootScreenModel
					{ InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber });
			}

			var stepData = await BuildStepData(contextData, new ScreenModel());

			SetTitle(StepTitle, stepData);

			return stepData;
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
		IDictionary<string, object> stepViewCustomizations = null)
		{
			Log.Debug($"Step5Review OnRefreshStepDataAsync called.");

			var result = (ScreenModel)originalScreenModel.CloneDeep();

			SetTitle(StepTitle, result);

			await BuildStepData(contextData, result);

			#region TODO: REPLACE WHEN STEP5 IS COMPLETED
			var step0 = contextData.GetStepData<Step0OperationSelection.ScreenModel>();
			var step2 = contextData.GetStepData<Step2InputPrns.ScreenModel>();
			if (step0.MovingHouseType.IsMoveElectricity() || step0.MovingHouseType.IsAddElectricity())
			{
				result.ElectricityAccount.IsApplicable = true;
				var account = await _domainQueryResolver.GetAccountInfoByPrn((ElectricityPointReferenceNumber)step2.NewMPRN);
				result.ElectricityAccount.RetrievedByNewPrn = account != null;
				result.ElectricityAccount.IsOpen = account?.IsOpen ?? false;
				if (step0.MovingHouseType.IsMoveElectricity())
				{
					account = await _domainQueryResolver.GetAccountInfoByPrn(
						(ElectricityPointReferenceNumber)step2.ExistingMPRN);
					result.ElectricityAccount.RetrievedByPreviousPrn = account != null;
					result.ElectricityAccount.IsOpen = account?.IsOpen ?? false;
				}
			}

			if (step0.MovingHouseType.IsMoveGas() || step0.MovingHouseType.IsAddGas())
			{
				result.GasAccount.IsApplicable = true;
				var account = await _domainQueryResolver.GetAccountInfoByPrn((GasPointReferenceNumber)step2.NewGPRN);
				result.GasAccount.RetrievedByNewPrn = account != null;
				result.GasAccount.IsOpen = account?.IsOpen ?? false;
				if (step0.MovingHouseType.IsMoveGas())
				{
					account = await _domainQueryResolver.GetAccountInfoByPrn((GasPointReferenceNumber)step2.ExistingGPRN);
					result.GasAccount.RetrievedByPreviousPrn = account != null;
					result.GasAccount.IsOpen = account?.IsOpen ?? false;
				}
			}

			if (step0.MovingHouseType.IsCloseElectricity())
			{
				result.ElectricityAccount.IsApplicable = true;
				var account = await _domainQueryResolver.GetAccountInfoByPrn(
					(ElectricityPointReferenceNumber)step2.ExistingMPRN);

				result.ElectricityAccount.RetrievedByPreviousPrn = account != null;
				result.ElectricityAccount.IsOpen = account?.IsOpen ?? false;
			}
			if (step0.MovingHouseType.IsCloseGas())
			{
				result.GasAccount.IsApplicable = true;
				var account = await _domainQueryResolver.GetAccountInfoByPrn(
					(GasPointReferenceNumber)step2.ExistingGPRN);

				result.GasAccount.RetrievedByPreviousPrn = account != null;
				result.GasAccount.IsOpen = account?.IsOpen ?? false;
			}
			#endregion TODO: REPLACE WHEN STEP5 IS COMPLETED
			return result;
		}

		private async Task<ScreenModel> BuildStepData(IUiFlowContextData contextData, ScreenModel screenModel)
		{
			Log.Debug($"Step5Review BuildStepData called.");

			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>();
			var step0Data = contextData.GetStepData<Step0OperationSelection.ScreenModel>();
			var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();
			var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();
			var step3Data = contextData.GetStepData<Step3InputMoveInPropertyDetails.ScreenModel>();
			var paymentResult = contextData.GetStepData<Step4ConfigurePayment.StepData>().CalledFlowResult;
			screenModel.MovingHouseType = step0Data.MovingHouseType;
			screenModel.InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber;
			screenModel.SecondaryAccountNumber = step1Data.GasAccountNumber;
			ConfigurePaymentDetails();

			await Task.WhenAll(PreviousPropertyInfo(), NewPropertyInfo(), MovingDateAndMeterReadings(), PricePlanScreen(), ResolveHasFreeElectricityAllowance());

			return screenModel;


			async Task PreviousPropertyInfo()
			{
				if (step1Data.UserMeterInputFields.Mprn != null)
				{
					screenModel.PreviousPropertyInfo = await ResolvePremiseAddressInfo(
						(ElectricityPointReferenceNumber)step1Data.UserMeterInputFields.Mprn,
						true);
				}
				else if (step1Data.UserMeterInputFields.Gprn != null)
				{
					screenModel.PreviousPropertyInfo = await ResolvePremiseAddressInfo(
						(GasPointReferenceNumber)step1Data.UserMeterInputFields.Gprn,
						false);
				}
			}

			async Task NewPropertyInfo()
			{
				var premiseInfo = step2Data.PremiseAddressInfos.FirstOrDefault();
				screenModel.NewPropertyInfo = new ScreenModel.PremiseInfo();
				if (premiseInfo != null)
				{
					screenModel.NewPropertyInfo.CareOf = premiseInfo.CareOf;
					screenModel.NewPropertyInfo.HouseNo = premiseInfo.HouseNo;
					screenModel.NewPropertyInfo.City = premiseInfo.City;
					screenModel.NewPropertyInfo.PostalCode = premiseInfo.PostalCode;
					screenModel.NewPropertyInfo.Street = premiseInfo.Street;
				}
			}

			async Task<ScreenModel.PremiseInfo> ResolvePremiseAddressInfo(PointReferenceNumber prn, bool withFailOver)
			{
				ScreenModel.PremiseInfo premiseInfo = null;
				var pointOfDeliveryInfo = await _domainQueryResolver.GetPointOfDeliveryInfoByPrn(prn, withFailOver);

				if (pointOfDeliveryInfo?.AddressInfo != null)
					premiseInfo = new ScreenModel.PremiseInfo
					{
						CareOf = pointOfDeliveryInfo.AddressInfo.CareOf,
						HouseNo = pointOfDeliveryInfo.AddressInfo.HouseNo,
						City = pointOfDeliveryInfo.AddressInfo.City,
						PostalCode = pointOfDeliveryInfo.AddressInfo.PostalCode,
						Street = pointOfDeliveryInfo.AddressInfo.Street,
						PremiseId = pointOfDeliveryInfo.PremiseId,
						PRN = (string)prn
					};

				return premiseInfo;
			}

			async Task MovingDateAndMeterReadings()
			{
				screenModel.MovingDatesMeterReadingInfoReadingsInfo.MoveOutElectricityDevicesMeterReadings =
					step1Data.ElectricityDevicesMeterReadings;

				screenModel.MovingDatesMeterReadingInfoReadingsInfo.MoveOutGasDevicesMeterReadings =
					step1Data.GasDevicesMeterReadings;

				screenModel.MovingDatesMeterReadingInfoReadingsInfo.MoveInElectricityDevicesMeterReadings =
					step3Data.ElectricityDevicesMeterReadings;

				if(step0Data.MovingHouseType != MovingHouseType.MoveElectricityAndCloseGas)
				{
					screenModel.MovingDatesMeterReadingInfoReadingsInfo.MoveInGasDevicesMeterReadings =
					step3Data.GasDevicesMeterReadings;
				}
			
				screenModel.MovingDatesMeterReadingInfoReadingsInfo.PreviousPropertyUserMeterInputFields = step1Data.UserMeterInputFields;
				screenModel.MovingDatesMeterReadingInfoReadingsInfo.NewPropertyUserMeterInputFields = step3Data.UserMeterInputFields;
				screenModel.MovingDatesMeterReadingInfoReadingsInfo.MoveOutDate =
					step1Data.MoveInOutDatePicker.MovingInOutSelectedDateTime?.ToDisplayDate();
				screenModel.MovingDatesMeterReadingInfoReadingsInfo.MoveInDate =
					step3Data.MoveInOutDatePicker.MovingInOutSelectedDateTime?.ToDisplayDate();
			}

			async Task ResolveHasFreeElectricityAllowance()
			{
				var hasFreeElectricityAllowance = false;
				var accountInfo = await _domainQueryResolver.GetFlowElectricityAccount(rootData);
				if(accountInfo!=null)
				{
					var premise = await _domainQueryResolver.GetPremiseByPrn((ElectricityPointReferenceNumber)accountInfo.PointReferenceNumber);
					if (premise.Installations.Any(x => x.HasFreeElectricityAllowance == true))
					{
						hasFreeElectricityAllowance = true;
					}
				}
				screenModel.HasFreeElectricityAllowance = hasFreeElectricityAllowance;
			}

			void ConfigurePaymentDetails()
			{
				screenModel.PaymentInfo = paymentResult.ConfigurationSelectionResults;
			}
			async Task PricePlanScreen()
			{
				var getAccounts =  _domainQueryResolver.GetFlowAccounts(rootData);
				
				var isPrnDeregistered = step2Data.IsMPRNDeregistered || step2Data.IsGPRNDeregistered;
				var isIsDeviceAcquisitionElectricity = step3Data.IsNewAcquisitionElectricity ?? false;
				var isDeviceAcquisitionGas = step3Data.IsNewAcquisitionGas ?? false;
                var isDirectDebit = screenModel.PaymentInfo.Any(c =>
					 c.SelectedPaymentSetUpType == PaymentSetUpType.SetUpNewDirectDebit ||
					 c.SelectedPaymentSetUpType == PaymentSetUpType.UseExistingDirectDebit);

				var isEqualizer =
					screenModel.PaymentInfo.Any(x => x.Account?.PaymentMethod == PaymentMethodType.Equalizer);

				var accounts = await getAccounts;
				var electricityAccountPaperBillChoice = accounts
					.FirstOrDefault(x => x.ClientAccountType == ClientAccountType.Electricity)?.PaperBillChoice;

				var gasAccountPaperBillChoice = accounts
					.FirstOrDefault(x => x.ClientAccountType == ClientAccountType.Gas)?.PaperBillChoice;

				if (isPrnDeregistered || isIsDeviceAcquisitionElectricity || isDeviceAcquisitionGas)
				{
					var isDualFuel = accounts.Length == 2;
					if (isDualFuel || electricityAccountPaperBillChoice == PaperBillChoice.On || gasAccountPaperBillChoice == PaperBillChoice.On || isDirectDebit || isEqualizer)
						screenModel.ValueSaver = true;
				}
				else
				{
					screenModel.ValueSaver = false;
				}
			}
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.RequestCompleteMoveHouse)
			{

				var stepData = contextData.GetStepData<ScreenModel>();
				var hasMovingHouseValidationError = await HasMovingHouseValidationError(contextData);
				if (hasMovingHouseValidationError)
				{
					stepData.HasValidationErrors = true;
					await base.OnHandlingStepEvent(triggeredEvent, contextData);
					return;
				}

				var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>();
				
				var getInitiatorAccountInfo=_domainQueryResolver.GetAccountInfoByAccountNumber(rootData.InitiatedFromAccountNumber);
				
				var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();


				var setUpDirectDebitDomainCommands = new List<SetUpDirectDebitDomainCommand>();

				foreach (var item in stepData.PaymentInfo)
				{
					if (item.SelectedPaymentSetUpType != PaymentSetUpType.Manual)
					{
						var clientAccountType =
							item.Account == null ? item.TargetAccountType : item.Account.ClientAccountType;
						var command = new SetUpDirectDebitDomainCommand(item.Account?.AccountNumber,
							item.CommandToExecute.NameOnBankAccount,
							item.BankAccount?.IBAN,
							item.CommandToExecute.IBAN,
							item.Account?.Partner,
							clientAccountType,
							PaymentMethodType.DirectDebit,
							null,
							null,
							null);

						setUpDirectDebitDomainCommands.Add(command);
					}
				}

				var moveHouse = new MoveHouse(
					rootData.ElectricityAccountNumber,
					rootData.GasAccountNumber,
					step1Data.MovingHouseType,
					setUpDirectDebitDomainCommands,
					(await getInitiatorAccountInfo).ClientAccountType);

				await _domainCommandDispatcher.ExecuteAsync(moveHouse);
				Log.Debug(() => "MoveHouse completed sucessfully ");

			
			}
			else
			{
				await base.OnHandlingStepEvent(triggeredEvent, contextData);
			}

		}


		public class ScreenModel : UiFlowScreenModel,  ICancellableMovingHouseStep
		{
			#region TODO: REPLACE WHEN STEP5 IS COMPLETED
			[JsonIgnore]
			public AccountStatus GasAccount { get; set; } = new AccountStatus();
			[JsonIgnore]
			public AccountStatus ElectricityAccount { get; set; } = new AccountStatus();
			public class AccountStatus
			{
				public bool IsApplicable { get; set; }
				public bool RetrievedByNewPrn { get; set; }
				public bool IsOpen { get; set; }
				public bool RetrievedByPreviousPrn { get; set; }
			}
			#endregion TODO: REPLACE WHEN STEP5 IS COMPLETED

			public PremiseInfo PreviousPropertyInfo { get; set; }

			public PremiseInfo NewPropertyInfo { get; set; }


			public MovingDatesMeterReadings MovingDatesMeterReadingInfoReadingsInfo { get; set; } = new MovingDatesMeterReadings();

			//TODO: consider refactor using custom type
			public IEnumerable<AccountsPaymentConfigurationResult.AccountConfigurationInfo> PaymentInfo { get; set; }

			public string SecondaryAccountNumber { get; set; }

			public string InitiatedFromAccountNumber { get; set; }

			public bool ValueSaver { get; set; }
			public MovingHouseType MovingHouseType { get; set; }
			public bool? HasFreeElectricityAllowance { get; set; }

			public class PremiseInfo
			{
				public string PremiseName { get; set; }
				public string PRN { get; set; }
				public string PostalCode { get; set; }
				public string Street { get; set; }
				public string HouseNo { get; set; }
				public string CareOf { get; set; }
				public string City { get; set; }
				public string PremiseId { get; set; }
			}

			public class MovingDatesMeterReadings
			{
				public string MoveOutDate { get; set; }

				public string MoveInDate { get; set; }

				public IEnumerable<DeviceRegisterUiModel> MoveOutGasDevicesMeterReadings { get; set; }
				public IEnumerable<DeviceRegisterUiModel> MoveOutElectricityDevicesMeterReadings { get; set; }

				public IEnumerable<DeviceRegisterUiModel> MoveInGasDevicesMeterReadings { get; set; }
				public IEnumerable<DeviceRegisterUiModel> MoveInElectricityDevicesMeterReadings { get; set; }

				public InputFields PreviousPropertyUserMeterInputFields { get; set; }

				public InputFields NewPropertyUserMeterInputFields { get; set; }
			}

			public bool? HasValidationErrors { get; set; }
		}
	}
}
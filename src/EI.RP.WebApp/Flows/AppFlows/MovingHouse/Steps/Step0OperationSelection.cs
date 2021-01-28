using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults;
using EI.RP.DomainServices.Queries.MovingHouse.InstalmentPlans;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class Step0OperationSelection : MovingHouseScreen
    {
	    private readonly IDomainQueryResolver _queryResolver;
	    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
		public Step0OperationSelection(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}

	    public override ScreenName ScreenStep => MovingHouseStep.Step0OperationSelection;
		

		public static class StepEvent
		{
			public static readonly ScreenEvent MoveElectricitySelected = new ScreenEvent(nameof(Step0OperationSelection),nameof(MoveElectricitySelected));
			public static readonly ScreenEvent MoveElectricityAndAddGasSelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(MoveElectricityAndAddGasSelected));
			public static readonly ScreenEvent CloseElectricitySelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(CloseElectricitySelected));

			public static readonly ScreenEvent MoveElectricityAndGasSelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(MoveElectricityAndGasSelected));
			public static readonly ScreenEvent MoveElectricityAndCloseGasSelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(MoveElectricityAndCloseGasSelected));
			public static readonly ScreenEvent CloseElectricityAndGasSelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(CloseElectricityAndGasSelected));


			public static readonly ScreenEvent MoveGasOnlySelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(MoveGasOnlySelected));
			public static readonly ScreenEvent MoveGasAndAddElectricitySelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(MoveGasAndAddElectricitySelected));
			public static readonly ScreenEvent CloseGasSelected = new ScreenEvent(nameof(Step0OperationSelection), nameof(CloseGasSelected));
			
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			const string rulesValidationFailed = "Rules validation failed";
			const string rulesAreValid = "Rules are valid";

			var nextStep = MovingHouseStep.Step1InputMoveOutPropertyDetails;
            var closeStep = MovingHouseStep.StepCloseAccounts;

            screenConfiguration.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError);

			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.MoveElectricitySelected,
				nextStep,
				async () => !await HasMovingHouseValidationError(contextData),
				rulesAreValid);

			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.MoveElectricitySelected,
				MovingHouseStep.ShowMovingHouseValidationError,
				async () => await HasMovingHouseValidationError(contextData),
				rulesValidationFailed);

			screenConfiguration.OnEventNavigatesTo(StepEvent.MoveElectricityAndAddGasSelected, nextStep);
			screenConfiguration.OnEventNavigatesTo(StepEvent.MoveElectricityAndGasSelected,nextStep);
			screenConfiguration.OnEventNavigatesTo(StepEvent.MoveElectricityAndCloseGasSelected,nextStep);
			screenConfiguration.OnEventNavigatesTo(StepEvent.MoveGasOnlySelected,nextStep);
			screenConfiguration.OnEventNavigatesTo(StepEvent.MoveGasAndAddElectricitySelected,nextStep);
			
			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.CloseElectricitySelected,
				closeStep,
				() => HasMovingHouseValidationError(contextData).ContinueWith(x => !x.Result),
				rulesAreValid);

			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.CloseElectricitySelected,
				MovingHouseStep.ShowMovingHouseValidationError,
				() => HasMovingHouseValidationError(contextData),
				rulesValidationFailed);
			
			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.CloseElectricityAndGasSelected,
				closeStep,
				() => HasMovingHouseValidationError(contextData).ContinueWith(x => !x.Result),
				rulesAreValid);

			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.CloseElectricityAndGasSelected,
				MovingHouseStep.ShowMovingHouseValidationError,
				() => HasMovingHouseValidationError(contextData),
				rulesValidationFailed);
			
			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.CloseGasSelected,
				closeStep,
				() => HasMovingHouseValidationError(contextData).ContinueWith(x => !x.Result),
				rulesAreValid);

			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.CloseGasSelected,
				MovingHouseStep.ShowMovingHouseValidationError,
				() => HasMovingHouseValidationError(contextData),
				rulesValidationFailed);

			return screenConfiguration;
		}

		async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
		{
            var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var validation = await _queryResolver.GetMovingHouseValidationResult(
                rootData.ElectricityAccountNumber,
                rootData.GasAccountNumber,
                null);

            return validation.Any(x => x.Output== OutputType.Failed);
        }

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
            IUiFlowContextData contextData)
		{
			if (triggeredEvent == ScreenEvent.ErrorOccurred)
			{
				Log.Debug($"Triggered Event: {nameof(ScreenEvent.ErrorOccurred)} in {nameof(Step4ConfigurePayment)}.{nameof(OnHandlingStepEvent)}.");
				return;
			}

			var stepData = contextData.GetCurrentStepData<ScreenModel>();
            if (triggeredEvent == StepEvent.MoveElectricityAndGasSelected)
            {
                stepData.MovingHouseType = MovingHouseType.MoveElectricityAndGas;
            }
            else if (triggeredEvent == StepEvent.MoveElectricityAndCloseGasSelected)
            {
                stepData.MovingHouseType = MovingHouseType.MoveElectricityAndCloseGas;
            }
            else if (triggeredEvent == StepEvent.MoveElectricitySelected)
            {
                stepData.MovingHouseType = MovingHouseType.MoveElectricity;
            }
            else if (triggeredEvent == StepEvent.MoveGasOnlySelected)
            {
                stepData.MovingHouseType = MovingHouseType.MoveGas;
            }
            else if (triggeredEvent == StepEvent.MoveElectricityAndAddGasSelected)
            {
                stepData.MovingHouseType = MovingHouseType.MoveElectricityAndAddGas;
            }
            else if (triggeredEvent == StepEvent.MoveGasAndAddElectricitySelected)
            {
                stepData.MovingHouseType = MovingHouseType.MoveGasAndAddElectricity;
            }
            else if (triggeredEvent == StepEvent.CloseGasSelected)
            {
                stepData.MovingHouseType = MovingHouseType.CloseGas;
            }
            else if (triggeredEvent == StepEvent.CloseElectricitySelected)
            {
                stepData.MovingHouseType = MovingHouseType.CloseElectricity;
            }
            else if (triggeredEvent == StepEvent.CloseElectricityAndGasSelected)
            {
                stepData.MovingHouseType = MovingHouseType.CloseElectricityAndGas;
            }
            else
            {
                throw new InvalidOperationException($"user selection {triggeredEvent} not supported");
            }

            stepData.ShowTermsAcknowledgmentType = stepData.MovingHouseType.ToShowTermsAcknowledgementType();

        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        var screenModel = await BuildStepData(contextData, new ScreenModel());

	        SetTitle(Title, screenModel);

	        return screenModel;
        }

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(
			IUiFlowContextData contextData, 
			UiFlowScreenModel originalStepData, 
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalStepData.CloneDeep();
			var data = (ScreenModel)refreshedStepData;

			SetTitle(Title, data);

			await BuildStepData(contextData, data);

			return data;
		}

		async Task<UiFlowScreenModel> BuildStepData(
			IUiFlowContextData contextData,
			ScreenModel stepData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);

			await Task.WhenAll(
				MapInitiatorAccountInfo(),
				MapGasAccountInfo(),
				MapElectricityAccountInfo()
			);

			return stepData;

			async Task MapInitiatorAccountInfo()
			{
				var initiatorAccount =
					await _queryResolver.GetAccountInfoByAccountNumber(rootData.InitiatedFromAccountNumber);
				stepData.InitiatedFromAccountNumberDescription = initiatorAccount.Description;
			}

			async Task MapGasAccountInfo()
			{
				if (!string.IsNullOrEmpty(rootData.GasAccountNumber))
				{
					var gasInfo = await _queryResolver.GetAccountInfoByAccountNumber(rootData.GasAccountNumber);
					stepData.GasAccountNumber = gasInfo.AccountNumber;

					var checkMoveOutRequestResult = await _queryResolver.CheckMoveOut(gasInfo.ContractId);
					stepData.HasGasAccountExitFee = checkMoveOutRequestResult.HasExitFee;
					if (!string.IsNullOrEmpty(rootData.ElectricityAccountNumber))
					{
						var instalmentPlanRequestResult =
							await _queryResolver.CheckInstalmentPlan(gasInfo.AccountNumber);
						stepData.HasInstalmentPlan = instalmentPlanRequestResult.HasInstalmentPlan;
					}
				}
			}

			async Task MapElectricityAccountInfo()
			{
				if (!string.IsNullOrEmpty(rootData.ElectricityAccountNumber))
				{
					var electricityInfo =
						await _queryResolver.GetAccountInfoByAccountNumber(rootData.ElectricityAccountNumber);
					stepData.ElectricityAccountNumber = electricityInfo.AccountNumber;
					var checkMoveOutRequestResult = await _queryResolver.CheckMoveOut(electricityInfo.ContractId);
					stepData.HasElectricityAccountExitFee = checkMoveOutRequestResult.HasExitFee;
					stepData.HasStaffDiscount = electricityInfo.HasStaffDiscountApplied;
				}
			}
		}


		public class ScreenModel : UiFlowScreenModel
		{
            public override bool IsValidFor(ScreenName screenStep)
            {
                return
					screenStep == MovingHouseStep.Step0OperationSelection ||
					screenStep == MovingHouseStep.Step1InputMoveOutPropertyDetails;
            }

			public string ElectricityAccountNumber { get; set; }
            public string GasAccountNumber { get; set; }
            public bool ShowRequirementsMessage { get; set; } = false;
            public string InitiatedFromAccountNumberDescription{ get; set; }
            public MovingHouseType MovingHouseType { get; set; }
            public ShowTermsAcknowledgmentType ShowTermsAcknowledgmentType { get; set; }
            public bool HasElectricityAccountExitFee { get; set; }
            public bool HasGasAccountExitFee { get; set; }
            public bool HasInstalmentPlan { get; set; }
            public bool HasStaffDiscount { get; set; }
		}
    }
}
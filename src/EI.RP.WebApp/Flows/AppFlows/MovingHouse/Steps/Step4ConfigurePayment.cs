using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using Newtonsoft.Json;
using NLog;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class Step4ConfigurePayment : MovingHouseScreen
	{
		protected string StepTitle => string.Join(" | ", "4. Payment", Title);

		private readonly IDomainQueryResolver _domainQueryResolver;

        public override ScreenName ScreenStep => MovingHouseStep.Step4ConfigurePayment;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
	            .OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError)
                
                .OnEventNavigatesToAsync(StepEvent.AccountsPaymentConfigurationCompleted, MovingHouseStep.Step5ReviewAndComplete,
                    () => HasMovingHouseValidationError(contextData).ContinueWith(x => !x.Result),
                    "Business rules validators are valid")
                .OnEventNavigatesToAsync(StepEvent.AccountsPaymentConfigurationCompleted, MovingHouseStep.ShowMovingHouseValidationError,
                    () => HasMovingHouseValidationError(contextData),
                    "Business rules validators are invalid");
        }

        async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();

            var query = new MovingHouseValidationQuery
            {
                ElectricityAccountNumber = rootData.ElectricityAccountNumber,
                GasAccountNumber = rootData.GasAccountNumber,
                MovingHouseType = step1Data.MovingHouseType,
                InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber,
                ValidateContractSaleChecks = true
            };

            var validation = await _domainQueryResolver.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
            return validation.Any(x => x.Output == OutputType.Failed);
        }

        public static class StepEvent
		{
			public static readonly ScreenEvent AccountsPaymentConfigurationCompleted = new ScreenEvent(nameof(Step4ConfigurePayment),nameof(AccountsPaymentConfigurationCompleted));
        }

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var result = (ConnectToFlow<AccountsPaymentConfigurationFlowInitializer.RootScreenModel, AccountsPaymentConfigurationResult>)originalScreenModel;
			result.StartData.CallbackFlowHandler = contextData.FlowHandler;

			SetTitle(StepTitle, result);

			return result;
		}
		private readonly IConfigurableTestingItems _configurableTestingItems;
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();
			var stepData = new StepData(ResidentialPortalFlowType.AccountsPaymentConfiguration, new AccountsPaymentConfigurationFlowInitializer.RootScreenModel
			{
                CallbackFlowHandler = contextData.FlowHandler,
				CallbackFlowEvent = StepEvent.AccountsPaymentConfigurationCompleted.ToString(),
			})
			{
				MovingHouseType = step1Data.MovingHouseType
			};

			SetTitle(StepTitle, stepData);

			ConfigurePaymentConfigurationFlowInput();

			

			return stepData;

			void ConfigurePaymentConfigurationFlowInput()
			{
				var startData = stepData.StartData;
				if (step1Data.MovingHouseType == MovingHouseType.MoveElectricity)
				{
					startData.AccountNumber = step1Data.ElectricityAccountNumber;
					startData.StartType = AccountsPaymentConfigurationFlowStartType.MoveElectricity;
				}
				else if (step1Data.MovingHouseType == MovingHouseType.MoveGas)
				{
					startData.AccountNumber = step1Data.GasAccountNumber;
					startData.StartType = AccountsPaymentConfigurationFlowStartType.MoveGas;
				}
				else if (step1Data.MovingHouseType == MovingHouseType.MoveElectricityAndAddGas)
				{
					startData.AccountNumber = step1Data.ElectricityAccountNumber;
					startData.SecondaryAccountNumber = step1Data.GasAccountNumber;
					startData.StartType = AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother;
				}
				else if (step1Data.MovingHouseType == MovingHouseType.MoveGasAndAddElectricity)
				{
					startData.AccountNumber = step1Data.GasAccountNumber;
					startData.SecondaryAccountNumber = step1Data.ElectricityAccountNumber;
					startData.StartType = AccountsPaymentConfigurationFlowStartType.MoveOneAndAddAnother;
				}
				else if (step1Data.MovingHouseType == MovingHouseType.MoveElectricityAndCloseGas)
				{
					startData.AccountNumber = step1Data.ElectricityAccountNumber;
					startData.StartType = AccountsPaymentConfigurationFlowStartType.MoveElectricityAndCloseGas;
				}
				else if (step1Data.MovingHouseType == MovingHouseType.MoveElectricityAndGas)
				{
					startData.AccountNumber = step1Data.ElectricityAccountNumber;
					startData.SecondaryAccountNumber = step1Data.GasAccountNumber;
					startData.StartType = AccountsPaymentConfigurationFlowStartType.MoveElectricityAndGas;
				}
				else
				{
					throw new NotImplementedException($"Case {step1Data.MovingHouseType} has not been yet implemented.");
				}
			}

			
		}
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public Step4ConfigurePayment(IConfigurableTestingItems configurableTestingItems, IDomainQueryResolver domainQueryResolver)
		{
			_configurableTestingItems = configurableTestingItems;
            _domainQueryResolver = domainQueryResolver;
        }

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.AccountsPaymentConfigurationCompleted)
			{
				AccountsPaymentConfigurationResult configurationResult=null;

				try
				{
					configurationResult = contextData.GetCurrentStepData<StepData>().CalledFlowResult;
					if (configurationResult.Exit == AccountsPaymentConfigurationResult.ExitType.ErrorOcurred)
					{
						throw new Exception(" AccountsPaymentFlow failed");
					}
				}
				catch (Exception ex)
				{
					Logger.Warn(() => ex.ToString());
					contextData.GetCurrentStepData<StepData>().CalledFlowResult.Exit =
						AccountsPaymentConfigurationResult.ExitType.ErrorOcurred;
					throw;
				}
			}
		}


		public class StepData : ConnectToFlow<AccountsPaymentConfigurationFlowInitializer.RootScreenModel, AccountsPaymentConfigurationResult>
		{
			public StepData(ResidentialPortalFlowType startFlowType, AccountsPaymentConfigurationFlowInitializer.RootScreenModel startData = null) 
				: base(startFlowType.ToString(), startData, true,true)
			{
			}

			[JsonConstructor]
			private StepData():base(true)
			{
			}

			public MovingHouseType MovingHouseType { get; set; }
		}

        public class ScreenModel : UiFlowScreenModel
        {
        }
    }
}
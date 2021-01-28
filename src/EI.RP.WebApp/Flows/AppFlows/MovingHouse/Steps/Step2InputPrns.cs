using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.PointOfDelivery;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class Step2InputPrns : MovingHouseScreen
	{
		private static ILogger Logger = LogManager.GetCurrentClassLogger();
	
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IDomainCommandDispatcher _commandDispatcher;

		public static class StepEvent
		{
			public static readonly ScreenEvent SubmitPRNS = new ScreenEvent(nameof(Step2InputPrns), nameof(SubmitPRNS));
			public static readonly ScreenEvent SubmitNewPRNS = new ScreenEvent(nameof(Step2InputPrns), nameof(SubmitNewPRNS));
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesToAsync(
					ScreenEvent.ErrorOccurred,
					MovingHouseStep.ShowMovingHouseUnhandledError,
					IsErrorOccurredEvent,
					"When Any Error other than Address/Mprn/Gprn Occurs")
				.OnEventReentriesCurrentAsync(
					 ScreenEvent.ErrorOccurred ,
					async () => !(await IsErrorOccurredEvent()),
					"When Any Other Error Occurs")

				.OnEventNavigatesToAsync(
					StepEvent.SubmitNewPRNS,
					MovingHouseStep.ShowMovingHouseValidationError,
					 () => HasMovingHouseValidationError(contextData),
					"When has Moving House Validation Errors")
				.OnEventNavigatesToAsync(
					StepEvent.SubmitNewPRNS,
					MovingHouseStep.ShowMovingHouseReEnterPrnError,
					async() => !await HasMovingHouseValidationError(contextData)&& (!MPRNIsValid() || !AddressMatch()),
					"When ReEnter MPRN is not valid or MPRN and GRPN address not match")
				.OnEventNavigatesToAsync(
					StepEvent.SubmitNewPRNS,
					MovingHouseStep.Step2ConfirmAddress,
					async () => !await HasMovingHouseValidationError(contextData) && MPRNIsValid() && AddressMatch(),
					"When ReEnter MPRN is valid And MPRN and GRPN address match")
				.OnEventNavigatesToAsync(
					StepEvent.SubmitPRNS,
					MovingHouseStep.Step2ConfirmAddress,
					async() => !await HasMovingHouseValidationError(contextData) && GPRNIsValid(),
					"Submit GPRN valid")
				.OnEventNavigatesToAsync(
					StepEvent.SubmitPRNS,
					MovingHouseStep.ShowMovingHouseValidationError,
					async() => await HasMovingHouseValidationError(contextData),
					"When has Moving House Validation Errors")
				.OnEventNavigatesToAsync(
					StepEvent.SubmitPRNS,
					MovingHouseStep.ShowMovingHouseReEnterPrnError,
					async() => !await HasMovingHouseValidationError(contextData) && !GPRNIsValid(),
					"Submit GPRN is not valid");

			bool AddressMatch()
			{
				var data = contextData.GetStepData<ScreenModel>();
				return data.PrnsAreFromSameAddress;
			}

            bool MPRNIsValid()
            {
                var data = contextData.GetStepData<ScreenModel>();
                return !string.IsNullOrEmpty(data.NewMPRN) && data.MPRNExist;
            }

            bool GPRNIsValid()
            {

                var data = contextData.GetStepData<ScreenModel>();
                return data.GPRNExist;
            }

            Task<bool> IsErrorOccurredEvent()
            {
				//methods that evaluate navigations event should handle errors themselves
	            try
	            {
		            var isAddressMatch = AddressMatch();
		            var mprnIsValid = MPRNIsValid();
		            var gprnIsValid = GPRNIsValid();
		            var hasValidationError = HasMovingHouseValidationError(contextData).Result;

		            if (hasValidationError)
		            {
			            return Task.FromResult(false); //"When has Moving House Validation Errors
		            }

		            if (!hasValidationError && (!mprnIsValid || !isAddressMatch))
		            {
			            return
				            Task.FromResult(
					            false); //"When ReEnter MPRN is not valid or MPRN and GRPN address not match"
		            }

		            if (!hasValidationError && mprnIsValid && isAddressMatch)
		            {
			            return Task.FromResult(false); // "When ReEnter MPRN is valid And MPRN and GRPN address match"
		            }

		            if (!hasValidationError && gprnIsValid)
		            {
			            return Task.FromResult(false); // "Submit GPRN valid"
		            }

		            if (!hasValidationError && !gprnIsValid)
		            {
			            return Task.FromResult(false);
		            }

		            if (gprnIsValid)
		            {
			            return Task.FromResult(false); // "Submit GPRN is not valid"
		            }

	            }
	            catch (Exception ex)
	            {
					Logger.Error(()=>ex.ToString());
	            }
	            return Task.FromResult(true);

			}
		}

		async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
		{
			try
			{
				var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
				var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();
				var data = contextData.GetStepData<ScreenModel>();
				var query = new MovingHouseValidationQuery
				{
					ElectricityAccountNumber = rootData.ElectricityAccountNumber,
					GasAccountNumber = rootData.GasAccountNumber,
					ValidateNewPremise = true,
					MovingHouseType = step1Data.MovingHouseType,
					NewMPRN = data.NewMPRN,
					NewGPRN = data.NewGPRN
				};

				var validation =
					await _queryResolver
						.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
				return validation.Any(x => x.Output == OutputType.Failed);
			}
			catch (Exception ex)
			{
				Logger.Error(() => ex.ToString());
			}

			return true;
		}

		public Step2InputPrns(IDomainQueryResolver queryResolver,IDomainCommandDispatcher commandDispatcher, IConfigurableTestingItems configurableTestingItems)
		{
			_queryResolver = queryResolver;
			_commandDispatcher = commandDispatcher;
			_configurableTestingItems = configurableTestingItems;
		}

		public override ScreenName ScreenStep => MovingHouseStep.Step2InputPrns;
		private readonly IConfigurableTestingItems _configurableTestingItems;
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();
			var stepData = new ScreenModel
			{
				ElectricityAccountNumber = rootData.ElectricityAccountNumber,
				GasAccountNumber = rootData.GasAccountNumber,
				InitiatedByAccountNumber = rootData.InitiatedFromAccountNumber,
				MovingHouseType = step1Data.MovingHouseType
			};

			var mapTask = MapCurrentProgress();

			ResolveVisibilities();

			
			await mapTask;

			SetTitle(string.Join(" | ", $"2. New {stepData.MovingHouseType.ToPrnText()}", Title), stepData);

			return stepData;


		

			async Task MapCurrentProgress()
			{
				var progress = await ResolveProgress();
				stepData.ExistingGPRN = step1Data.UserMeterInputFields.Gprn;
				stepData.ExistingMPRN = step1Data.UserMeterInputFields.Mprn;
				stepData.NewGPRN = (string) progress.NewGprn;
				stepData.NewMPRN = (string) progress.NewMprn;
			}

			async Task<MovingHouseInProgressNewPRNsInfo> ResolveProgress()
			{
				var account1Task = _queryResolver.GetAccountInfoByAccountNumber(rootData.InitiatedFromAccountNumber);
				var account2Task =  _queryResolver.GetSecondaryAccount(rootData);

				var account1 = await account1Task;
				var account2 = await account2Task;

				var progress = await _queryResolver.GetMovingHouseProgressNewPrnsInfo(account1, account2, contextData.GetStepData<Step0OperationSelection.ScreenModel>().MovingHouseType);

				return progress;
			}

			void ResolveVisibilities()
			{
				stepData.ShowElectricityAccount = !string.IsNullOrWhiteSpace(stepData.ElectricityAccountNumber);
				stepData.ShowGasAccount = !string.IsNullOrWhiteSpace(stepData.GasAccountNumber) &&
				                          stepData.MovingHouseType !=
				                          MovingHouseType.MoveElectricityAndCloseGas;
				if (stepData.MovingHouseType == MovingHouseType.MoveElectricityAndAddGas ||
				    stepData.MovingHouseType == MovingHouseType.MoveGasAndAddElectricity ||
				    stepData.MovingHouseType == MovingHouseType.MoveElectricityAndGas)
				{
					stepData.ShowMPRNGPRNInput = true;
					stepData.ShowElectricityAccount = true;
					stepData.ShowGasAccount = true;
				}
			}
		}
		
		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			var transitionEvents = new[] {StepEvent.SubmitNewPRNS, StepEvent.SubmitPRNS};
			if (transitionEvents.Contains(triggeredEvent))
			{
				var command = await MapProgressCommand();
				await _commandDispatcher.ExecuteAsync(command);
			}

			await base.OnHandlingStepEvent(triggeredEvent, contextData);

			async Task<RecordMovingHomePrns> MapProgressCommand()
			{
				var rootStepData = contextData.GetStepData<FlowInitializer.RootScreenModel>();
				var stepData = contextData.GetCurrentStepData<ScreenModel>();
				var account1Task = _queryResolver.GetAccountInfoByAccountNumber(rootStepData.InitiatedFromAccountNumber);
				var account2Task = _queryResolver.GetSecondaryAccount(rootStepData);

				var account1 = await account1Task;
				var account2 = await account2Task;
				return new RecordMovingHomePrns(contextData.GetStepData<Step0OperationSelection.ScreenModel>().MovingHouseType,account1, account2, stepData.NewMPRN, stepData.NewGPRN);
			}
		}

		private async Task<ScreenModel> ResolvePremiseInfo(ScreenModel screenModel, MovingHouseType movingHouseType)
		{
			var premiseAddressInfos = new ConcurrentHashSet<ScreenModel.PremiseInfo>();
			var mprnPremiseInfo = await ResolvePremiseAddressInfo((ElectricityPointReferenceNumber) screenModel.NewMPRN);
			var gprnPremiseInfo = await ResolvePremiseAddressInfo((GasPointReferenceNumber) screenModel.NewGPRN);
			ResolveElectricity(mprnPremiseInfo);
			ResolveGas(gprnPremiseInfo);

			screenModel.PremiseAddressInfos = premiseAddressInfos;
			return screenModel;

			void ResolveElectricity(ScreenModel.PremiseInfo mprnAddress)
			{
				if (mprnAddress != null)
				{
					mprnAddress.PremiseName = "MPRN";
					premiseAddressInfos.Add(mprnAddress);
					screenModel.MPRNExist = true;
					if (string.IsNullOrEmpty(mprnAddress.PremiseId))
						screenModel.IsMPRNDeregistered = true;

                    screenModel.IsMPRNAddressInSwitch = mprnAddress.IsAddressInSwitch;
                }
				else
				{
					screenModel.MPRNExist = string.IsNullOrEmpty(screenModel.NewMPRN);
				}
			}

			void ResolveGas(ScreenModel.PremiseInfo gprnAddress)
			{
				if(gprnAddress == null && 
					(movingHouseType == MovingHouseType.MoveGas))
				{
					screenModel.GPRNExist = false;
					return;
				}

				if (gprnAddress != null)
				{
					gprnAddress.PremiseName = "GPRN";
					premiseAddressInfos.Add(gprnAddress);
					screenModel.GPRNExist = true;
					if (!string.IsNullOrEmpty(screenModel.NewMPRN))
					{
						ResolveAddressMatch(gprnAddress);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(screenModel.NewGPRN))
					{
						var premiseInfo = premiseAddressInfos.FirstOrDefault();
						gprnAddress = new ScreenModel.PremiseInfo
						{
							HouseNo = premiseInfo?.HouseNo,
							Street = premiseInfo?.Street,
							PostalCode = premiseInfo?.PostalCode,
							City = premiseInfo?.City,
							PRN = screenModel.NewGPRN,
							PremiseId = premiseInfo?.PremiseId,
							PremiseName = "GPRN"
						};

						screenModel.IsGPRNDeregistered = true;
						premiseAddressInfos.Add(gprnAddress);
					}
					else
					{
						screenModel.GPRNExist = string.IsNullOrEmpty(screenModel.NewGPRN);
					}
				}
			}

			async Task<ScreenModel.PremiseInfo> ResolvePremiseAddressInfo(PointReferenceNumber prn)
			{
				ScreenModel.PremiseInfo premiseInfo = null;
				if (prn!=null)
				{
					var pointOfDeliveryInfo =
						await _queryResolver.GetPointOfDeliveryInfoByPrn(prn);

					var addressInfo = pointOfDeliveryInfo?.AddressInfo;

					if (addressInfo != null)
					{
						premiseInfo = new ScreenModel.PremiseInfo
						{
							CareOf = addressInfo.CareOf,
							HouseNo = addressInfo.HouseNo,
							City = addressInfo.City,
							PostalCode = addressInfo.PostalCode,
							Street = addressInfo.Street,
							PremiseId = pointOfDeliveryInfo.PremiseId,
							PRN = (string) prn,
                            IsAddressInSwitch = pointOfDeliveryInfo.IsAddressInSwitch
                        };
					}
				}

				return premiseInfo;
			}

			void ResolveAddressMatch(ScreenModel.PremiseInfo gprnAddress)
			{
				var mprnAddress = premiseAddressInfos.FirstOrDefault(x => x.PremiseName == "MPRN");
				screenModel.PrnsAreFromSameAddress = mprnAddress?.Street == gprnAddress.Street 
				                          && mprnAddress?.City == gprnAddress.City 
				                          && mprnAddress?.HouseNo == gprnAddress.HouseNo;
			}
		}

		protected override bool OnValidateTransitionAttempt(
			ScreenEvent triggeredEvent, 
			IUiFlowContextData contextData,
			out string errorMessage)
		{
			errorMessage = null;
			if (triggeredEvent == StepEvent.SubmitPRNS || triggeredEvent == StepEvent.SubmitNewPRNS)
			{
				var input = contextData.GetCurrentStepData<ScreenModel>();
				var movingHouseType = contextData.GetStepData<Step0OperationSelection.ScreenModel>().MovingHouseType;
				if (!string.IsNullOrEmpty(input.NewMPRN) || !string.IsNullOrEmpty(input.NewGPRN))
				{
					ResolvePremiseInfo(input, movingHouseType).Wait();
					if (triggeredEvent == StepEvent.SubmitPRNS)
					{

						if (!input.MPRNExist)
						{
							errorMessage = "MPRN not found";
							return false;
						}
						if (!input.PrnsAreFromSameAddress)
						{
							errorMessage = "Address not found";
							return false;
						}
					}
				}
			}

			return base.OnValidateTransitionAttempt(triggeredEvent, contextData, out errorMessage);
		}

		public class ScreenModel : UiFlowScreenModel, ICancellableMovingHouseStep
		{
			public string InitiatedByAccountNumber { get; set; }
			public string ElectricityAccountNumber { get; set; }
			public string GasAccountNumber { get; set; }

			public bool IsMPRNDeregistered { get; set; }
			public bool IsMPRNAddressInSwitch { get; set; }
            public bool IsGPRNDeregistered { get; set; }

			[RequiredIf(nameof(ShowElectricityAccount), IfValue = true, ErrorMessage = "You must enter a valid MPRN")]
			[RegularExpression(ElectricityPointReferenceNumber.NewMPRNRegEx, ErrorMessage =
				"Please enter a valid MPRN")]
			[CustomCompare(nameof(ExistingMPRN), ErrorMessage =
				"The MPRN you entered is the same as the MPRN for the home you are leaving. Please enter new MPRN.")]
			public string NewMPRN { get; set; }

			[RequiredIf(nameof(ShowGasAccount), IfValue = true, ErrorMessage = "You must enter a valid GPRN")]
			[RegularExpression(GasPointReferenceNumber.GPRNRegEx, ErrorMessage = "Please enter a valid GPRN")]
			[CustomCompare(nameof(ExistingGPRN), ErrorMessage =
				"The GPRN you entered is the same as the GPRN for the home you are leaving. Please enter new GPRN.")]
			public string NewGPRN { get; set; }

			public string ExistingGPRN { get; set; }
			public string ExistingMPRN { get; set; }

			public bool MPRNExist { get; set; } = true;

			public bool GPRNExist { get; set; } = true;

			public bool ShowMPRNGPRNInput { get; set; }
			public MovingHouseType MovingHouseType { get; set; }

			public bool PrnsAreFromSameAddress { get; set; } = true;

			public IEnumerable<PremiseInfo> PremiseAddressInfos { get; set; } = new PremiseInfo[0];

			public bool ShowElectricityAccount { get; set; }

			public bool ShowGasAccount { get; set; }

			public string InitiatedFromAccountNumber { get; set; }



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
                public bool IsAddressInSwitch { get; set; }
            }
		}
	}
}
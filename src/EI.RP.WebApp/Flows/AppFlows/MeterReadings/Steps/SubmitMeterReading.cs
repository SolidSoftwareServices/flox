using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps
{
	public class SubmitMeterReading : MeterReadingScreen
	{
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;
		private readonly IDomainQueryResolver _domainQueryResolver;

		public SubmitMeterReading(
			IDomainCommandDispatcher domainCommandDispatcher,
			IDomainQueryResolver domainQueryResolver)
		{
			_domainCommandDispatcher = domainCommandDispatcher;
			_domainQueryResolver = domainQueryResolver;
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent SubmitMeterReading =
				new ScreenEvent(nameof(SubmitMeterReading), nameof(SubmitMeterReading));

			public static readonly ScreenEvent SubmitMeterReadingErrored =
				new ScreenEvent(nameof(SubmitMeterReading), nameof(SubmitMeterReadingErrored));
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MeterReadingStep.ErrorSubmittingMeterReadingStep)
				.OnEventNavigatesTo(StepEvent.SubmitMeterReading, MeterReadingStep.ErrorSubmittedLessThanActual,
					() => HasMeterReadingLessThanActualError(contextData),
					"When submit meter readings is less then actual failed")
				.OnEventNavigatesTo(StepEvent.SubmitMeterReading, MeterReadingStep.ShowMeterReadingStatus,
					() => !HasMeterReadingLessThanActualError(contextData),
					"When submit meter readings is less then actual");
		}

		public override ScreenName ScreenStep => MeterReadingStep.SubmitMeterReading;

		bool HasMeterReadingLessThanActualError(IUiFlowContextData contextData)
		{
			var stepData = contextData.GetStepData<ScreenModel>();
			return stepData.HasMeterReadingLessThanActualNetworkError ||
			       stepData.HasMeterReadingLessThanActualCustomerError;
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<MeterReadingFlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var getDevices = _domainQueryResolver.GetDevicesByAccount(rootData.AccountNumber);
			var stepData = new ScreenModel
			{
				AccountNumber = rootData.AccountNumber,
				AccountType = rootData.AccountType
			};

			SetTitle(Title, stepData);

			var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(stepData.AccountNumber);
			var contractItem =  accountInfo.BusinessAgreement.Contracts.FirstOrDefault();
			if (contractItem != null)
			{
				stepData.MeterPointRefNumber = contractItem.Premise.PointOfDeliveries.FirstOrDefault()?.Prn ?? " ";
			}

			stepData.CanSubmitMeterReading = accountInfo.CanSubmitMeterReading;

			await ConfigureMeterReadings();

			return stepData;

			async Task ConfigureMeterReadings()
			{
				var gasMeterTypes = new[]
				{
					MeterType.Gas,
					MeterType.GasConsumption,
					MeterType.GasConversionFactor,
					MeterType.GasPeakDayCapacity,
					MeterType.GasVolume
				};

				var devices = (await getDevices) .ToArray();
				var registers = (stepData.AccountType == ClientAccountType.Electricity
					? devices.SelectMany(x => x.Registers)
					: devices.SelectMany(x => x.Registers).Where(x =>
						x.RegisterId == MeterReadingRegisterType.ActiveEnergyRegisterType)).ToArray();

				stepData.MeterReadings = registers
					.Select(x => new ScreenModel.MeterData
					{
						MeterNumber = x.MeterNumber,
						MaskedMeterNumber = GetMaskedMeterNumber(x.MeterNumber),
						MeterTypeName = x.MeterType.ToString(),
						MeterLabel = gasMeterTypes.Contains(x.MeterType) ? "Gas" : x.MeterType.ToString(),
						MeterUnit = x.MeterUnit.ToDisplayFormat(),
						DeviceId = x.DeviceId,
						RegisterId = x.RegisterId.ToString()
					}).ToArray();

				var meterTypes = registers.GroupBy(x => x.MeterType)
					.Select(grp => grp.First().MeterType)
					.ToArray();

				stepData.HasElectricityDayNightMeter =
					meterTypes.Any(x => x != null && x == MeterType.ElectricityDay) &&
					meterTypes.Any(x => x != null && x == MeterType.ElectricityNight);

			}

			string GetMaskedMeterNumber(string meterNumber)
			{
				return meterNumber.Substring(meterNumber.Length - 4).PadLeft(meterNumber.Length, '*');
			}
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.SubmitMeterReading)
			{
				await _Submit();
			}

			async Task _Submit()
			{
				var rootData =
					contextData.GetStepData<MeterReadingFlowInitializer.RootScreenModel>(ScreenName.PreStart);
				var stepData = contextData.GetCurrentStepData<ScreenModel>();
				stepData.HasMeterReadingLessThanActualNetworkError = false;
				stepData.HasMeterReadingLessThanActualCustomerError = false;

				var meterReadingDataResults = new List<MeterReadingData>();

				foreach (var meterReaderResult in stepData.MeterReadings)
				{
					var meterReadingData = new MeterReadingData
					{
						DeviceId = meterReaderResult.DeviceId,
						MeterReading = meterReaderResult.ReadingValue,
						RegisterId = meterReaderResult.RegisterId,
						MeterNumber = meterReaderResult.MeterNumber,
						MeterTypeName = meterReaderResult.MeterTypeName
					};

					meterReadingDataResults.Add(meterReadingData);
				}

				var cmd = new SubmitMeterReadingCommand(stepData.AccountNumber, meterReadingDataResults, validateLastResults: true);

				try
				{
					await _domainCommandDispatcher.ExecuteAsync(cmd);
				}
				catch (AggregateException ex)
				{
					//TODO: message mapping mechanism
					if (ex.InnerExceptions != null && ex.InnerExceptions.Any(x => (x as DomainException).DomainError
						.IsOneOf(ResidentialDomainError.UnableToProcessRequest,
							ResidentialDomainError.DataAlreadyReleased)))
					{
						throw new DomainException(ResidentialDomainError.SorryUnableToProcessRequest);
					}

					if (ex.InnerExceptions != null && ex.InnerExceptions.Any(x =>
					{
						var domainException = x as DomainException;
						if (domainException != null && domainException.DomainError.Equals(ResidentialDomainError.MeterReadingLessThanActualNetwork))
						{
							stepData.HasMeterReadingLessThanActualNetworkError = true;
						}

						if (domainException != null && domainException.DomainError.Equals(ResidentialDomainError.MeterReadingLessThanActualCustomer))
						{
							stepData.HasMeterReadingLessThanActualCustomerError = true;
						}

						return domainException == null ||
							!(domainException.DomainError.Equals(ResidentialDomainError.MeterReadingLessThanActualNetwork) ||
							 domainException.DomainError.Equals(ResidentialDomainError.MeterReadingLessThanActualCustomer));

					}))
					{
						throw;
					}
				}
			}
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalScreenModel.CloneDeep();
			var updatedStepData = (ScreenModel) refreshedStepData;

			SetTitle(Title, updatedStepData);

			if (updatedStepData.Errors.Any())
			{
				foreach (var meterReader in updatedStepData.MeterReadings)
				{
					var index = Array.IndexOf(updatedStepData.MeterReadings, meterReader);
					var itemNameKey =
						$"{nameof(updatedStepData.MeterReadings)}[{index}].{nameof(meterReader.ReadingValue)}";
					if (updatedStepData.Errors.Any(x => x.MemberName == itemNameKey))
					{
						updatedStepData.MeterReadings[index].ReadingValue = null;
					}
				}
			}

			if (stepViewCustomizations != null)
			{
				updatedStepData.SetFlowCustomizableValue(stepViewCustomizations, x => x.PageIndex);
			}

			return updatedStepData;
		}

		public class ScreenModel : UiFlowScreenModel
		{
			public class MeterData
			{
				public string DeviceId { get; set; }
				public string MeterTypeName { get; set; }
				public string MeterNumber { get; set; }
				public string MaskedMeterNumber { get; set; }
				public string MeterLabel { get; set; }
				public string MeterUnit { get; set; }
				public string RegisterId { get; set; }

				[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid meter reading ")]
				[RegularExpression(ReusableRegexPattern.ValidMeterReading,
					ErrorMessage = "Please enter a valid meter reading ")]
				public string ReadingValue { get; set; }
			}

			public MeterData[] MeterReadings { get; set; }
			public string AccountNumber { get; set; }
			public string MeterPointRefNumber { get; set; }
			public bool HasElectricityDayNightMeter { get; set; }

			public ClientAccountType AccountType { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == MeterReadingStep.SubmitMeterReading;
			}

			public int PageIndex { get; set; }

			public bool HasMeterReadingLessThanActualNetworkError { get; set; }
			public bool HasMeterReadingLessThanActualCustomerError { get; set; }
			public bool CanSubmitMeterReading { get; set; }
		}
	}
}
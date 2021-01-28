using System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;

using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows;

using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using Newtonsoft.Json;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Geography.Country;
using EI.RP.DomainServices.Queries.Geography.Region;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class Step1InputCloseAccountDetails : MovingHouseScreen
	{
		protected override string Title => "Close Account";

		public override ScreenName ScreenStep => MovingHouseStep.StepCloseAccounts;
		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;

		public Step1InputCloseAccountDetails(IDomainCommandDispatcher domainCommandDispatcher, IDomainQueryResolver domainQueryResolver, IConfigurableTestingItems configurableTestingItems)
		{
			_domainQueryResolver = domainQueryResolver;
			_configurableTestingItems = configurableTestingItems;
			_domainCommandDispatcher = domainCommandDispatcher;
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent CancelMovingHouse = new ScreenEvent(nameof(Step1InputCloseAccountDetails), nameof(CancelMovingHouse));
			public static readonly ScreenEvent CloseElectricityAndGas = new ScreenEvent(nameof(Step1InputCloseAccountDetails), nameof(CloseElectricityAndGas));
			public static readonly ScreenEvent CloseElectricity = new ScreenEvent(nameof(Step1InputCloseAccountDetails), nameof(CloseElectricity));
			public static readonly ScreenEvent CloseGas = new ScreenEvent(nameof(Step1InputCloseAccountDetails), nameof(CloseGas));
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(StepEvent.CancelMovingHouse, MovingHouseStep.Step0OperationSelection)
				.OnEventNavigatesTo(StepEvent.CloseElectricity, MovingHouseStep.CloseAccountConfirmation,
					() => !HasValidationErrors(contextData),
					"Rules are valid")
				.OnEventNavigatesTo(
					StepEvent.CloseElectricity,
					MovingHouseStep.ShowMovingHouseValidationError,
					() => HasValidationErrors(contextData),
					"Rules validation failed")
				.OnEventNavigatesTo(StepEvent.CloseGas, MovingHouseStep.CloseAccountConfirmation,
					() => !HasValidationErrors(contextData),
					"Rules are valid")
				.OnEventNavigatesTo(
					StepEvent.CloseGas,
					MovingHouseStep.ShowMovingHouseValidationError,
					() => HasValidationErrors(contextData),
					"Rules validation failed")
				.OnEventNavigatesTo(StepEvent.CloseElectricityAndGas, MovingHouseStep.CloseAccountConfirmation,
					() => !HasValidationErrors(contextData),
					"Rules are valid")
				.OnEventNavigatesTo(
					StepEvent.CloseElectricityAndGas,
					MovingHouseStep.ShowMovingHouseValidationError,
					() => HasValidationErrors(contextData),
					"Rules validation failed")
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError)
				;
		}

		private bool HasValidationErrors(IUiFlowContextData contextData)
		{
			var stepData = contextData.GetStepData<ScreenModel>();
			return stepData.HasValidationErrors == true;
		}

		async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var data = contextData.GetStepData<ScreenModel>();
			var query = new MovingHouseValidationQuery
			{
				ElectricityAccountNumber = rootData.ElectricityAccountNumber,
				GasAccountNumber = rootData.GasAccountNumber,
				ValidatePhoneNumber = data.UserLettingFields.OccupierDetailsAccepted,
				PhoneNumber = data.UserLettingFields.OccupierDetailsAccepted ? data.UserLettingFields.LettingPhoneNumber : string.Empty,
				MovingHouseType = data.MovingHouseType,
				ValidateCanCloseAccounts = true,
				MoveOutDate = data.MoveInOutDatePicker.MovingInOutSelectedDateTime
			};

			var validation = await _domainQueryResolver.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
			return validation.Any(x => x.Output == OutputType.Failed);
		}
		private readonly IConfigurableTestingItems _configurableTestingItems;
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var stepData = new ScreenModel();

			SetTitle(Title, stepData);

			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);

			var getAccounts =  _domainQueryResolver.GetFlowAccounts(rootData);

			var landingPageStepData = contextData.GetStepData<Step0OperationSelection.ScreenModel>();

			stepData.ShowTermsAcknowledgmentType =
				GetShowTermsAcknowledgmentType(landingPageStepData);
			stepData.MovingHouseType = landingPageStepData.MovingHouseType;
			stepData.InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber;

			MapElectricityData();
			MapGasData();
			await MapMovingOutDate();
			stepData.UserMeterInputFields.MeterReadingDescription =
				"We'll need your meter readings from the day of your move to ensure your final bill is accurate.";
			
			await YourForwardingAddress();
			return stepData;



			async Task MapMovingOutDate()
			{
				var userAccount = (await  getAccounts).FirstOrDefault();

				if (userAccount == null)
				{
					return;
				}
				stepData.MoveInOutDatePicker = new MoveInOutDatePicker();

				var datePicker = stepData.MoveInOutDatePicker;
				datePicker.MovingInOutSelectedDateTime = userAccount.ContractStartDate == DateTime.Now.AddDays(-1).Date
						? DateTime.Now.Date
						: DateTime.Now.AddDays(-1).Date;

				var diffDays = (DateTime.Now.Date - (await  getAccounts).OrderByDescending(c => c.ContractStartDate).FirstOrDefault()?.ContractStartDate)?.TotalDays;

				if (diffDays.HasValue)
				{
					var datePickerRangeOfDays = diffDays > 30 ? 30 : (diffDays - 1 <= 0 ? -1 : diffDays - 1);
					datePicker.Intervals = datePickerRangeOfDays.Value;
				}
				BuildDatePicker(stepData);
				datePicker.DatePickerHoverPopupDescription = "Please select the date of move out from date range available";
			}
			async Task YourForwardingAddress()
			{
				stepData.AddressType = AddressType.RepublicOfIreland;
				stepData.ROIAddress.IsROIBoxFieldRequired = true;
				stepData = await ResolveRegionListAndCountryList(stepData);
			}

			void MapGasData()
			{
				stepData.GasAccountNumber = rootData.GasAccountNumber;
				stepData.GasDevicesMeterReadings = rootData.GasDevicesMeterReadings;
				stepData.UserMeterInputFields.Gprn = rootData.UserMeterInputFields.Gprn;
				stepData.UserMeterInputFields.GasDevicesFieldRequired = rootData.UserMeterInputFields.GasDevicesFieldRequired;
			}

			void MapElectricityData()
			{
				stepData.ElectricityAccountNumber = rootData.ElectricityAccountNumber;
				stepData.UserMeterInputFields.Mprn = rootData.UserMeterInputFields.Mprn;
				stepData.ElectricityDevicesMeterReadings = rootData.ElectricityDevicesMeterReadings;
				stepData.UserMeterInputFields.Electricity24HrsDevicesFieldRequired = rootData.UserMeterInputFields.Electricity24HrsDevicesFieldRequired;
				stepData.UserMeterInputFields.ElectricityDayDevicesFieldRequired = rootData.UserMeterInputFields.ElectricityDayDevicesFieldRequired;
				stepData.UserMeterInputFields.ElectricityNightDevicesFieldRequired = rootData.UserMeterInputFields.ElectricityDayDevicesFieldRequired;
			}

		}

		private static void BuildDatePicker(ScreenModel stepData)
		{
			var interval = stepData.MoveInOutDatePicker.Intervals;
			var startDate = DateTime.Now.AddDays(-(Math.Abs(interval)));
			var endDate = DateTime.Now;
			stepData.MoveInOutDatePicker.SelectableDateRange = new DateTimeRange(startDate, endDate);
			stepData.MoveInOutDatePicker.DatePickerTitle =
				$"Date of move out. Please use format DD / MM / YYYY and date in between {stepData.MoveInOutDatePicker.SelectableDateRange.Start.ToString("yyyy-MM-dd")} and {stepData.MoveInOutDatePicker.SelectableDateRange.End.ToString("yyyy-MM-dd")}";
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			var closeEvents = new[] { StepEvent.CloseGas, StepEvent.CloseElectricity, StepEvent.CloseElectricityAndGas };
			var stepData = contextData.GetCurrentStepData<ScreenModel>();

			if (closeEvents.Any(x => x == triggeredEvent))
			{
				var hasMovingHouseValidationError = await HasMovingHouseValidationError(contextData);
				if (hasMovingHouseValidationError)
				{
					stepData.HasValidationErrors = true;
					await base.OnHandlingStepEvent(triggeredEvent, contextData);
					return;
				}

				var addressInfo = ResolveAddress(stepData);

				var cmd = new CloseAccountsCommand(
					stepData.InitiatedFromAccountNumber == stepData.ElectricityAccountNumber
						? ClientAccountType.Electricity
						: ClientAccountType.Gas
					, addressInfo
					, stepData.MoveInOutDatePicker.MovingInOutSelectedDateTime.Value
					, stepData.ElectricityAccountNumber != null
						? new ElectricityMeterReading(stepData.ElectricityAccountNumber,
							MeterReadAsNumber(stepData.UserMeterInputFields.MeterReading24Hrs),
							MeterReadAsNumber(stepData.UserMeterInputFields.MeterReadingDay),
							MeterReadAsNumber(stepData.UserMeterInputFields.MeterReadingNight)
						)
						: null
					, stepData.GasAccountNumber != null
						? new GasMeterReading(stepData.GasAccountNumber
							, MeterReadAsNumber(stepData.UserMeterInputFields.MeterReadingGas))
						: null,
					moveOutIncommingOccupantInfo: MapLettingInfo());
				await _domainCommandDispatcher.ExecuteAsync(cmd);
			}

			MoveOutIncommingOccupantInfo MapLettingInfo()
			{
				if (stepData.UserLettingFields == null) return null;
				return new MoveOutIncommingOccupantInfo
				{
					IncomingOccupant = stepData.UserLettingFields.IncomingOccupant,
					LettingAgentName = stepData.UserLettingFields.LettingAgentName,
					LettingPhoneNumber = stepData.UserLettingFields.LettingPhoneNumber
				};
			}

			int? MeterReadAsNumber(string value)
			{
				return value == null ? (int?)null : int.Parse(value);
			}
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = (ScreenModel)originalScreenModel.CloneDeep();

			SetTitle(Title, refreshedStepData);

			refreshedStepData = await ResolveRegionListAndCountryList(refreshedStepData);
			BuildDatePicker(refreshedStepData);
			return refreshedStepData;
		}

		private async Task<ScreenModel> ResolveRegionListAndCountryList(ScreenModel screenModel)
		{
			screenModel.ROIAddress.RegionList =
				await _domainQueryResolver.GetRegionByCountryId("IE");

			var countries = (await _domainQueryResolver.GetCountries()).ToArray();
			screenModel.NIAddress.CountryList = countries;
			screenModel.PostalAddress.CountryList = countries;
			return screenModel;
		}

		private static AddressInfo ResolveAddress(ScreenModel screenModel)
		{
			var roiAddress = screenModel.ROIAddress;
			var niAddress = screenModel.NIAddress;
			var postalAddress = screenModel.PostalAddress;
			var accountAddress = new AddressInfo();

			if (screenModel.ROIAddress.IsROIBoxFieldRequired)
			{
				accountAddress.AddressType = AddressType.RepublicOfIreland;
				accountAddress.PostalCode =
					roiAddress.PostalCode == null ? string.Empty : roiAddress.PostalCode.ToUpper();
				accountAddress.Street = roiAddress.Street == null ? string.Empty : roiAddress.Street.ToUpper();
				accountAddress.City = roiAddress.Town == null ? string.Empty : roiAddress.Town.ToUpper();
				accountAddress.AddressLine1 =
					roiAddress.AddressLine1 == null ? string.Empty : roiAddress.AddressLine1.ToUpper();
				accountAddress.AddressLine5 =
					roiAddress.AddressLine2 == null ? string.Empty : roiAddress.AddressLine2.ToUpper();
				accountAddress.AddressLine4 = string.Empty;
				accountAddress.AddressLine2 = string.Empty;
				accountAddress.HouseNo =
					roiAddress.HouseNumber == null ? string.Empty : roiAddress.HouseNumber.ToUpper();
				accountAddress.Region = roiAddress.County == null ? string.Empty : roiAddress.County.ToUpper();
			}
			else if (screenModel.PostalAddress.IsPOBoxFieldRequired)
			{
				accountAddress.AddressType = AddressType.PO;
				accountAddress.POBoxPostalCode = postalAddress.POBoxPostCode == null
					? string.Empty
					: postalAddress.POBoxPostCode.ToUpper();
				accountAddress.POBox = postalAddress.POBoxNumber == null
					? string.Empty
					: postalAddress.POBoxNumber.ToUpper();
				accountAddress.District =
					postalAddress.District == null ? string.Empty : postalAddress.District.ToUpper();
				accountAddress.CountryID =
					postalAddress.Country == null ? string.Empty : postalAddress.Country.ToUpper();
			}
			else if (screenModel.NIAddress.IsNIFieldRequired)
			{
				accountAddress.AddressType = AddressType.NI;
				accountAddress.PostalCode = niAddress.PostCode == null ? string.Empty : niAddress.PostCode.ToUpper();
				accountAddress.Street = niAddress.Street == null ? string.Empty : niAddress.Street.ToUpper();
				accountAddress.City = niAddress.Town == null ? string.Empty : niAddress.Town.ToUpper();
				accountAddress.CountryID = niAddress.Country == null ? CountryIdType.IE : niAddress.Country.ToUpper();
				accountAddress.AddressLine5 =
					niAddress.AddressLine2 == null ? string.Empty : niAddress.AddressLine2.ToUpper();
				accountAddress.AddressLine1 =
					niAddress.AddressLine1 == null ? string.Empty : niAddress.AddressLine1.ToUpper();
				accountAddress.AddressLine4 = string.Empty;
				accountAddress.AddressLine2 = string.Empty;
				accountAddress.HouseNo = niAddress.HouseNumber.ToUpper();
				accountAddress.District = niAddress.District.ToUpper();
			}

			return accountAddress;
		}

		private ShowTermsAcknowledgmentType GetShowTermsAcknowledgmentType(Step0OperationSelection.ScreenModel step0Data)
		{
			if (step0Data.MovingHouseType == MovingHouseType.CloseElectricityAndGas)
				return ShowTermsAcknowledgmentType.ShowBoth;

			if (step0Data.MovingHouseType == MovingHouseType.CloseElectricity)
				return ShowTermsAcknowledgmentType.ShowElectricity;

			if (step0Data.MovingHouseType == MovingHouseType.CloseGas)
				return ShowTermsAcknowledgmentType.ShowGas;

			return step0Data.ShowTermsAcknowledgmentType;
		}

		public class ScreenModel : UiFlowScreenModel, ICancellableMovingHouseStep,
			IElectricityAndGasMeterMovingHouse, IAcknowledgmentsPartMovingHouse, IMoveInOutDatePicker, IIncommingOccupantPart, IForwardingAddressPartMovingHouse
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == MovingHouseStep.StepCloseAccounts;
			}

			public string ElectricityAccountNumber { get; set; }
			public string GasAccountNumber { get; set; }

			public bool IsDuelFuel()
			{
				return !string.IsNullOrEmpty(ElectricityAccountNumber) && !string.IsNullOrEmpty(GasAccountNumber);
			}
			public IEnumerable<DeviceRegisterUiModel> ElectricityDevicesMeterReadings { get; set; }
			public IEnumerable<DeviceRegisterUiModel> GasDevicesMeterReadings { get; set; }
			public MovingHouseType MovingHouseType { get; set; }
			public InputFields UserMeterInputFields { get; set; } = new InputFields();
			public ConfirmationInputFields UserConfirmationInputFields { get; set; } = new ConfirmationInputFields();
			public LettingFields UserLettingFields { get; set; } = new LettingFields();
			public string InitiatedFromAccountNumber { get; set; }
			public string AddressType { get; set; }
			public MoveInOutDatePicker MoveInOutDatePicker { get; set; }
			public ROIAddressInfo ROIAddress { get; set; } = new ROIAddressInfo();
			public NIAddressInfo NIAddress { get; set; } = new NIAddressInfo();
			public POBoxFields PostalAddress { get; set; } = new POBoxFields();
			public ShowTermsAcknowledgmentType? ShowTermsAcknowledgmentType { get; set; }
			public bool? HasValidationErrors { get; set; }
		}
	}
}
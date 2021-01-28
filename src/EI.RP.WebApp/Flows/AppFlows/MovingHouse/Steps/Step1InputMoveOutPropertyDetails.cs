using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
	public class Step1InputMoveOutPropertyDetails : MovingHouseScreen
	{
		protected string StepTitle => string.Join(" | ", "1. Previous Property Details", Title);

		private readonly IDomainCommandDispatcher _commandDispatcher;

		private readonly IDomainQueryResolver _queryResolver;

		private readonly IConfigurableTestingItems _configurableTestingItems;

		public Step1InputMoveOutPropertyDetails(IDomainQueryResolver queryResolver, IDomainCommandDispatcher commandDispatcher, IConfigurableTestingItems configurableTestingItems)
		{
			_queryResolver = queryResolver;
			_commandDispatcher = commandDispatcher;
			_configurableTestingItems = configurableTestingItems;
		}

		public override ScreenName ScreenStep => MovingHouseStep.Step1InputMoveOutPropertyDetails;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			var nextStep = MovingHouseStep.Step2InputPrns;

			screenConfiguration.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError);

			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.ToStep2,
				nextStep,
				async () => !await HasMovingHouseValidationError(contextData),
				"Rules are valid");

			screenConfiguration.OnEventNavigatesToAsync(
				StepEvent.ToStep2,
				MovingHouseStep.ShowMovingHouseValidationError,
				async () => await HasMovingHouseValidationError(contextData),
				"Rules validation failed");

			return screenConfiguration
				.OnEventNavigatesTo(StepEvent.CancelMovingHouse, MovingHouseStep.Step0OperationSelection);
		}

		async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var data = contextData.GetStepData<ScreenModel>();
			var query = new MovingHouseValidationQuery
			{
				ElectricityAccountNumber = rootData.ElectricityAccountNumber,
				GasAccountNumber = rootData.GasAccountNumber,
				ValidateHasAccountDevices = true,
				ValidatePhoneNumber = data.UserLettingFields.OccupierDetailsAccepted,
				PhoneNumber = data.UserLettingFields.OccupierDetailsAccepted ? data.UserLettingFields.LettingPhoneNumber : string.Empty,
				MovingHouseType = data.MovingHouseType,
				ValidateCanCloseAccounts = true,
				MoveOutDate = data.MoveInOutDatePicker.MovingInOutSelectedDateTime
			};

			var validation = await _queryResolver.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
			return validation.Any(x => x.Output == OutputType.Failed);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var stepData = new ScreenModel();

			SetTitle(StepTitle, stepData);

			var landingPageStepData = contextData.GetStepData<Step0OperationSelection.ScreenModel>();

			stepData.ShowTermsAcknowledgmentType =
				GetShowTermsAcknowledgmentType(landingPageStepData);
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			stepData.MovingHouseType = landingPageStepData.MovingHouseType;
			stepData.InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber;

			stepData = await BuildStepData(contextData, stepData, true);
			await MapCurrentProgress();

			return stepData;

			async Task MapCurrentProgress()
			{
				//load only the first time then let the flows keep the values store on exit
				var progress = await ResolveProgress();

				stepData.MoveInOutDatePicker.MovingInOutSelectedDateTime = progress.MovingOutDate < DateTime.Today.Subtract(TimeSpan.FromDays(7)) ? DateTime.Today.AddDays(-1) : progress.MovingOutDate;
				stepData.UserMeterInputFields.MeterReadingGas = progress.GasMeterReadingValue == 0
					? string.Empty
					: progress.GasMeterReadingValue.ToString();
				stepData.UserMeterInputFields.MeterReadingNightStorageHeater =
					progress.ElectricityMeterReadingNightOrNshValue == 0
						? string.Empty
						: progress.ElectricityMeterReadingNightOrNshValue.ToString();
				stepData.UserMeterInputFields.MeterReadingNight = progress.ElectricityMeterReadingNightOrNshValue == 0
					? string.Empty
					: progress.ElectricityMeterReadingNightOrNshValue.ToString();
				stepData.UserMeterInputFields.MeterReading24Hrs = progress.ElectricityMeterReadingDayOr24HrsValue == 0
					? string.Empty
					: progress.ElectricityMeterReadingDayOr24HrsValue.ToString();
				stepData.UserMeterInputFields.MeterReadingDay = progress.ElectricityMeterReadingDayOr24HrsValue == 0
					? string.Empty
					: progress.ElectricityMeterReadingDayOr24HrsValue.ToString();

				var inputFields = stepData.UserConfirmationInputFields;
				inputFields.InformationCollectionAuthorized = progress.InformationCollectionAuthorized;
				inputFields.TermsAndConditionsAccepted = progress.TermsAndConditionsAccepted;

				var lettingFields = stepData.UserLettingFields;
				lettingFields.IncomingOccupant = progress.IncomingOccupant;
				lettingFields.LettingAgentName = progress.LettingAgentName;
				lettingFields.LettingPhoneNumber = progress.LettingPhoneNumber;
				lettingFields.OccupierDetailsAccepted = progress.OccupierDetailsAccepted;
			}

			async Task<MovingHouseInProgressMovingOutInfo> ResolveProgress()
			{
				var account1Task = _queryResolver.GetAccountInfoByAccountNumber(rootData.InitiatedFromAccountNumber);
				var account2 = await _queryResolver.GetSecondaryAccount(rootData);
				var account1 = await account1Task;
				

				var progress =
					await _queryResolver.GetMovingHouseProgressMoveOutInfo(account1, account2,
						landingPageStepData.MovingHouseType);
				if (progress == null)
				{
					var command = await MapProgressCommand(stepData, rootData, landingPageStepData);
					await _commandDispatcher.ExecuteAsync(command);
					progress = await _queryResolver.GetMovingHouseProgressMoveOutInfo(account1, account2,
						landingPageStepData.MovingHouseType);
				}

				return progress;
			}


		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(
			IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalScreenModel.CloneDeep();
			var data = (ScreenModel)refreshedStepData;

			SetTitle(StepTitle, data);

			return await BuildStepData(contextData, data, false);
		}

		private async Task<ScreenModel> BuildStepData(IUiFlowContextData contextData, ScreenModel screenModel, bool firstLoad)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var getAccounts = _queryResolver.GetFlowAccounts(rootData);
			screenModel.ShowTermsAcknowledgmentType =
				GetShowTermsAcknowledgmentType(contextData.GetStepData<Step0OperationSelection.ScreenModel>());
			MapGasData();
			MapElectricityData();

			await MapMovingOutDate();

			async Task  MapMovingOutDate()
			{
				var userAccount = (await getAccounts).FirstOrDefault();

				if (userAccount == null) return;

				if (firstLoad) screenModel.MoveInOutDatePicker = new MoveInOutDatePicker();
				var datePicker = screenModel.MoveInOutDatePicker;

				if (firstLoad)
					datePicker.MovingInOutSelectedDateTime =
						userAccount.ContractStartDate == DateTime.Now.AddDays(-1).Date
							? DateTime.Now.Date
							: DateTime.Now.AddDays(-1).Date;

				var diffDays = (DateTime.Now.Date - (await getAccounts).OrderByDescending(c => c.ContractStartDate)
					.FirstOrDefault()?.ContractStartDate)?.TotalDays;
				if (diffDays.HasValue)
				{
					var datePickerRangeOfDays = diffDays > 30 ? 30 : diffDays - 1 <= 0 ? -1 : diffDays - 1;
					datePicker.Intervals = datePickerRangeOfDays.Value;
				}
				var interval = screenModel.MoveInOutDatePicker.Intervals;
				var startDate = DateTime.Now.AddDays(-(Math.Abs(interval)));
				var endDate = DateTime.Now;
				screenModel.MoveInOutDatePicker.SelectableDateRange = new DateTimeRange(startDate, endDate);
				screenModel.MoveInOutDatePicker.DatePickerTitle = $"Date of move out. Please use format DD / MM / YYYY and date in between {screenModel.MoveInOutDatePicker.SelectableDateRange.Start.ToString("yyyy-MM-dd")} and {screenModel.MoveInOutDatePicker.SelectableDateRange.End.ToString("yyyy-MM-dd")}";
				datePicker.DatePickerHoverPopupDescription =
					"Please select the date of move out from date range available";
				if (screenModel.GasAccountNumber != null && screenModel.ElectricityAccountNumber != null)
					datePicker.DatePickerDescription =
						"This date must be the date of the final meter readings for Electricity and Gas.";
				else if (screenModel.ElectricityAccountNumber != null)
					datePicker.DatePickerDescription =
						"This date must be the date of the final meter reading for Electricity.";
				else if (screenModel.GasAccountNumber != null)
					datePicker.DatePickerDescription =
						"This date must be the date of the final meter reading for Gas.";
			}

			void MapGasData()
			{
				screenModel.GasAccountNumber = rootData.GasAccountNumber;
				screenModel.GasDevicesMeterReadings = rootData.GasDevicesMeterReadings;
				screenModel.UserMeterInputFields.Gprn = rootData.UserMeterInputFields.Gprn;
				screenModel.UserMeterInputFields.GasDevicesFieldRequired =
					rootData.UserMeterInputFields.GasDevicesFieldRequired;
				screenModel.UserMeterInputFields.MeterReadingDescription =
					"We'll need your meter readings from the day of your move to ensure your final bill is accurate.";
			}

			void MapElectricityData()
			{
				screenModel.ElectricityAccountNumber = rootData.ElectricityAccountNumber;
				screenModel.UserMeterInputFields.Mprn = rootData.UserMeterInputFields.Mprn;
				screenModel.ElectricityDevicesMeterReadings = rootData.ElectricityDevicesMeterReadings;
				screenModel.UserMeterInputFields.Electricity24HrsDevicesFieldRequired =
					rootData.UserMeterInputFields.Electricity24HrsDevicesFieldRequired;
				screenModel.UserMeterInputFields.ElectricityDayDevicesFieldRequired =
					rootData.UserMeterInputFields.ElectricityDayDevicesFieldRequired;
				screenModel.UserMeterInputFields.ElectricityNightDevicesFieldRequired =
					rootData.UserMeterInputFields.ElectricityNightDevicesFieldRequired;
				screenModel.UserMeterInputFields.ElectricityNightStorageHeaterDevicesFieldRequired =
					rootData.UserMeterInputFields.ElectricityNightStorageHeaterDevicesFieldRequired;
				screenModel.UserMeterInputFields.MeterReadingDescription =
					"We'll need your meter readings from the day of your move to ensure your final bill is accurate.";
			}

			return screenModel;
		}

		private ShowTermsAcknowledgmentType GetShowTermsAcknowledgmentType(Step0OperationSelection.ScreenModel step0Data)
		{
			if (step0Data.MovingHouseType == MovingHouseType.MoveElectricityAndCloseGas)
				return ShowTermsAcknowledgmentType.ShowBoth;

			if (step0Data.MovingHouseType == MovingHouseType.MoveElectricityAndAddGas)
				return ShowTermsAcknowledgmentType.ShowElectricity;

			if (step0Data.MovingHouseType == MovingHouseType.MoveGasAndAddElectricity)
				return ShowTermsAcknowledgmentType.ShowGas;

			return step0Data.ShowTermsAcknowledgmentType;
		}

		private async Task<RecordMovingOutProgress> MapProgressCommand(ScreenModel screenModel,
			FlowInitializer.RootScreenModel rootScreenModel, Step0OperationSelection.ScreenModel step0Data)
		{
			var account1Task = _queryResolver.GetAccountInfoByAccountNumber(rootScreenModel.InitiatedFromAccountNumber);
			var account2Task = _queryResolver.GetSecondaryAccount(rootScreenModel);

			var account1 = await account1Task;
			var account2 = await account2Task;

			var electricityAccount = account1.IsElectricityAccount()
				? account1
				: account2 != null && account2.IsElectricityAccount()
					? account2
					: null;
			var gasAccount = account1.IsGasAccount()
				? account1
				: account2 != null && account2.IsGasAccount()
					? account2
					: null;
			var lettingFields = screenModel.UserLettingFields;

			var meterInputFields = screenModel.UserMeterInputFields;
			var electricityMeterReading24HrsOrDayValue = 0;
			var electricityMeterReadingNightValueOrNshValue = 0;

			if (electricityAccount != null)
			{
				electricityMeterReading24HrsOrDayValue =
					int.Parse((meterInputFields.MeterReading24Hrs ?? meterInputFields.MeterReadingDay) ?? 0.ToString());
				electricityMeterReadingNightValueOrNshValue = int.Parse(
					(meterInputFields.MeterReadingNight ?? meterInputFields.MeterReadingNightStorageHeater) ??
					0.ToString());
			}

			var gasMeterReadingValue = 0;
			if (gasAccount != null) gasMeterReadingValue = int.Parse(meterInputFields.MeterReadingGas ?? 0.ToString());

			var command = new RecordMovingOutProgress(
				step0Data.MovingHouseType,
				account1,
				account2,
				electricityMeterReading24HrsOrDayValue,
				electricityMeterReadingNightValueOrNshValue,
				gasMeterReadingValue,
				incomingOccupant: lettingFields.IncomingOccupant,
				lettingAgentName: lettingFields.LettingAgentName,
				lettingPhoneNumber: lettingFields.LettingPhoneNumber,
				occupierDetailsAccepted: lettingFields.OccupierDetailsAccepted,
				informationCollectionAuthorized: screenModel.UserConfirmationInputFields.InformationCollectionAuthorized,
				termsAndConditionsAccepted: screenModel.UserConfirmationInputFields.TermsAndConditionsAccepted,
				movingOutDate: screenModel.MoveInOutDatePicker.MovingInOutSelectedDateTime.Value
			);

			return command;
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.ToStep2)
			{
				var rootStepData = contextData.GetStepData<FlowInitializer.RootScreenModel>();
				var stepData = contextData.GetCurrentStepData<ScreenModel>();
				var command = await MapProgressCommand(stepData, rootStepData,
					contextData.GetStepData<Step0OperationSelection.ScreenModel>());
				await _commandDispatcher.ExecuteAsync(command);
			}

			await base.OnHandlingStepEvent(triggeredEvent, contextData);
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent ToStep2 =
				new ScreenEvent(nameof(Step1InputMoveOutPropertyDetails), nameof(ToStep2));

			public static readonly ScreenEvent CancelMovingHouse =
				new ScreenEvent(nameof(Step1InputMoveOutPropertyDetails), nameof(CancelMovingHouse));
		}


		public class ScreenModel : UiFlowScreenModel, ICancellableMovingHouseStep,
			IElectricityAndGasMeterMovingHouse, IAcknowledgmentsPartMovingHouse, IMoveInOutDatePicker,
			IIncommingOccupantPart
		{
			public MovingHouseType MovingHouseType { get; set; }
			public string ElectricityAccountNumber { get; set; }
			public string GasAccountNumber { get; set; }
			public ConfirmationInputFields UserConfirmationInputFields { get; set; } = new ConfirmationInputFields();
			public ShowTermsAcknowledgmentType? ShowTermsAcknowledgmentType { get; set; }
			public string InitiatedFromAccountNumber { get; set; }
			public IEnumerable<DeviceRegisterUiModel> GasDevicesMeterReadings { get; set; }
			public IEnumerable<DeviceRegisterUiModel> ElectricityDevicesMeterReadings { get; set; }
			public InputFields UserMeterInputFields { get; set; } = new InputFields();
			public LettingFields UserLettingFields { get; set; } = new LettingFields();
			public MoveInOutDatePicker MoveInOutDatePicker { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == MovingHouseStep.Step1InputMoveOutPropertyDetails;
			}
		}
	}
}
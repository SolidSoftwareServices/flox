using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EI.RP.CoreServices.System;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.Register;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.Progress;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Infrastructure.StringResources;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
    public class Step3InputMoveInPropertyDetails : MovingHouseScreen
	{
		protected string StepTitle => string.Join(" | ", "3. New Property Details", Title);

		public override ScreenName ScreenStep => MovingHouseStep.Step3InputMoveInPropertyDetails;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private readonly IConfigurableTestingItems _configurableTestingItems;

		public Step3InputMoveInPropertyDetails(IDomainQueryResolver queryResolver, IDomainCommandDispatcher commandDispatcher, IConfigurableTestingItems configurableTestingItems)
		{
			_queryResolver = queryResolver;
			_commandDispatcher = commandDispatcher;
			_configurableTestingItems = configurableTestingItems;
		}

		public static class StepEvent
		{
			public static readonly ScreenEvent ToStep4 = new ScreenEvent(nameof(Step3InputMoveInPropertyDetails),nameof(ToStep4));
			public static readonly ScreenEvent CancelMovingHouse = new ScreenEvent(nameof(Step3InputMoveInPropertyDetails), nameof(CancelMovingHouse));
		}

		private static DateTime PopulateMoveInDate(DateTime? step1MoveOutDate,
			InstallationInfo electricityInstallationInfo, InstallationInfo gasInstallationInfo, bool isNewAcquisitionElectricity)
		{
			var newPremiseDateList = new List<DateTime>();
			var moveInDateList = new List<DateTime>();
			var moveOutDateList = new List<DateTime>();

			step1MoveOutDate = step1MoveOutDate ?? default(DateTime);
			newPremiseDateList.Add(step1MoveOutDate.Value);

			var electricityDevices = electricityInstallationInfo?.Devices?.ToList();
			var gasDevices = gasInstallationInfo?.Devices?.ToList();
			ResolvePremiseDevices(electricityDevices, gasDevices);

			newPremiseDateList = newPremiseDateList.OrderByDescending(d => d.Date).ToList();

			var result = default(DateTime);
			if (newPremiseDateList.FirstOrDefault() == moveInDateList.OrderByDescending(d => d.Date).FirstOrDefault())
			{
				result = moveInDateList.OrderByDescending(d => d.Date).FirstOrDefault().Date >= DateTime.Now.Date
					? default(DateTime)
					: newPremiseDateList.FirstOrDefault().AddDays(2);
			}
			else if (newPremiseDateList.FirstOrDefault() ==
					 moveOutDateList.OrderByDescending(d => d.Date).FirstOrDefault())
			{
				result = moveOutDateList.OrderByDescending(d => d.Date).FirstOrDefault().Date > DateTime.Now.Date
					? default(DateTime)
					: newPremiseDateList.FirstOrDefault().AddDays(1);
			}
			else
			{
				result = newPremiseDateList.FirstOrDefault().AddDays(1);
			}

            if (isNewAcquisitionElectricity)
            {
                var minElectricityAcquisitionDate = DateTime.Today.AddDays(-3);
                if (result < minElectricityAcquisitionDate)
                    result = minElectricityAcquisitionDate;
            }

			return result;


			void ResolvePremiseDevices(List<DeviceInfo> devices1, List<DeviceInfo> devices2)
			{
				devices1 = devices1 ?? new List<DeviceInfo>();
				devices2 = devices2 ?? new List<DeviceInfo>();

				foreach (var device in devices1.Union(devices2))
				{
					var meterReadingResult = device.MeterReadingResults.Where(m => m.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveIn)
						.OrderByDescending(x => x.ReadingDate).FirstOrDefault();
					var moveInDate = meterReadingResult?.ReadingDate ?? default(DateTime);
					moveInDateList.Add(moveInDate);
					newPremiseDateList.Add(moveInDate);

					meterReadingResult = device.MeterReadingResults.Where(m => m.MeterReadingReasonID == MeterReadingReason.MeterReadingAtMoveOut)
						.OrderByDescending(x => x.ReadingDate).FirstOrDefault();
					var moveOutDate =
						meterReadingResult?.ReadingDate ?? default(DateTime);
					moveOutDateList.Add(moveOutDate);
					newPremiseDateList.Add(moveOutDate);
				}
			}
		}

		private async Task<ScreenModel> PopulateGasMeterReading(InstallationInfo installationInfo,
			ScreenModel screenModel, string newGprn)
		{
			IEnumerable<DeviceRegisterUiModel> gasDevicesMeterReadingList = new DeviceRegisterUiModel[0];

			var isNewAcquisitionGas = installationInfo == null ||
									  installationInfo.DeregStatus == DeregStatusType.Deregistered ||
									  installationInfo?.Devices == null ||
									  !installationInfo.Devices.Any(x => x.Registers.Any(r => r.MeterType.IsGas()));

			if (!isNewAcquisitionGas)
			{
				gasDevicesMeterReadingList = GetInstallationDeviceRegisters(installationInfo)
					.Where(x => x.MeterType.IsGas() && x.RegisterId == MeterReadingRegisterType.ActiveEnergyRegisterType)
					.Select(x => x.MeterNumber).Distinct().Select(meter => new DeviceRegisterUiModel
					{ MeterType = MeterType.Gas, MeterNumber = meter, MeterUnit = MeterUnit.M3 }).ToArray();
			}
			else
			{
				screenModel.IsNewAcquisitionGas = true;
				gasDevicesMeterReadingList = new DeviceRegisterUiModel
				{
					MeterType = MeterType.Gas,
					MeterNumber = "123456",
					MeterUnit = MeterUnit.M3
				}.ToOneItemArray();
			}

			if (gasDevicesMeterReadingList.Count() == 1)
			{
				screenModel.UserMeterInputFields.Gprn = newGprn;
				screenModel.UserMeterInputFields.GasDevicesFieldRequired = true;
			}

			screenModel.GasDevicesMeterReadings = gasDevicesMeterReadingList;

			return screenModel;
		}

		private IEnumerable<DeviceRegisterInfo> GetInstallationDeviceRegisters(
			InstallationInfo installationInfo)
		{
			return installationInfo.Devices.SelectMany(x => x.Registers).ToArray();
		}

		private async Task<ScreenModel> PopulateElectricityMeterReading(InstallationInfo installationInfo,
			ScreenModel screenModel, PointReferenceNumber newMprn)
		{
			var electricityDevicesMeterReadingList = new List<DeviceRegisterUiModel>();
			var hasElectricityDevices = installationInfo?.Devices != null &&
					installationInfo.Devices.Any(x => x.Registers.Any(r => r.MeterType.IsElectricity()));

			var isNewAcquisitionElectricity = installationInfo.DeregStatus == DeregStatusType.Deregistered || !hasElectricityDevices;

			if (!isNewAcquisitionElectricity)
			{
				electricityDevicesMeterReadingList = GetInstallationDeviceRegisters(installationInfo)
														.Where(x => x.MeterType.IsElectricity())
														.Select(register => new DeviceRegisterUiModel(register))
														.ToList();
			}
			else
			{
				screenModel.IsNewAcquisitionElectricity = true;

				var registerList = await _queryResolver.GetRegisterInfoByPrn(newMprn);
				if (registerList.Length == 0) return screenModel;
				foreach (var device in registerList)
				{
					var electricityDevicesMeterReading = new DeviceRegisterUiModel
					{
						MeterType = device.TimeofUsePeriod.ToMeterType(),
						MeterNumber = device.DeviceId,
						MeterUnit = MeterUnit.KWH
					};
					electricityDevicesMeterReadingList.Add(electricityDevicesMeterReading);
				}
			}

			screenModel.ElectricityDevicesMeterReadings = electricityDevicesMeterReadingList.ToArray();

			foreach (var meterReadingInput in screenModel.ElectricityDevicesMeterReadings)
			{
				if (meterReadingInput.MeterType == MeterType.Electricity24h)
				{
					screenModel.UserMeterInputFields.Electricity24HrsDevicesFieldRequired = true;
				}
				else if (meterReadingInput.MeterType == MeterType.ElectricityDay)
				{
					screenModel.UserMeterInputFields.ElectricityDayDevicesFieldRequired = true;
				}
				else if (meterReadingInput.MeterType == MeterType.ElectricityNight)
				{
					screenModel.UserMeterInputFields.ElectricityNightDevicesFieldRequired = true;
				}
				else if (meterReadingInput.MeterType == MeterType.ElectricityNightStorageHeater)
				{
					screenModel.UserMeterInputFields.ElectricityNightStorageHeaterDevicesFieldRequired = true;
				}
			}

			screenModel.UserMeterInputFields.Mprn = newMprn.ToString();
			return screenModel;
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var screenModel = new ScreenModel
			{
				ShowTermsAcknowledgmentType = contextData.GetStepData<Step0OperationSelection.ScreenModel>()
					.MovingHouseType.ToShowTermsAcknowledgementType()
			};

			SetTitle(StepTitle, screenModel);

			var step1StepData = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			screenModel.MovingHouseType = step1StepData.MovingHouseType;
			screenModel.InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber;

			screenModel.ElectricityAccountNumber = rootData.ElectricityAccountNumber;
			screenModel.GasAccountNumber = rootData.GasAccountNumber;

			await BuildStepData(contextData, screenModel,true);
			return screenModel;			
		}

		private async Task<ScreenModel> BuildStepData(IUiFlowContextData contextData, ScreenModel screenModel,bool creatingStep)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var step1StepData = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();
			var step2StepData = contextData.GetStepData<Step2InputPrns.ScreenModel>();
			var premiseAddressInfos = step2StepData.PremiseAddressInfos.ToArray();
			Task<MovingHouseInProgressMovingInInfo> getProgressTask=null;

			if (creatingStep)
			{
				var acc1 = _queryResolver.GetAccountInfoByAccountNumber(rootData.InitiatedFromAccountNumber);
				var acc2 = _queryResolver.GetSecondaryAccount(rootData);
				getProgressTask = _queryResolver.GetMovingHouseProgressMoveInInfo(await acc1, await acc2, contextData.GetStepData<Step0OperationSelection.ScreenModel>().MovingHouseType);
            }

            var electricityInstallationInfo = await GetFirstInstallationFromPremise("MPRN");
            var gasInstallationInfo = await GetFirstInstallationFromPremise("GPRN");

            screenModel.ShowTermsAcknowledgmentType = contextData.GetStepData<Step0OperationSelection.ScreenModel>()
				.ShowTermsAcknowledgmentType;
		
			await Task.WhenAll(MapElectricityData(), MapGasData());
			screenModel.UserMeterInputFields.MeterReadingDescription =
				"We will need the meter reading at your new home from the day you move in to make sure you are billed correctly from the start.";

			MapMoveInDate();

			if (creatingStep)
			{
				await MapCurrentProgress(await getProgressTask);
			}
			return screenModel;

			async Task MapCurrentProgress(MovingHouseInProgressMovingInInfo progress)
			{
				screenModel.MoveInOutDatePicker.MovingInOutSelectedDateTime = progress.MovingInDate==null||progress.MovingInDate.Value<DateTime.Today.AddDays(-3)? screenModel.MoveInOutDatePicker.MovingInOutSelectedDateTime:progress.MovingInDate;
				screenModel.ContactNumber = progress.ContactNumber?? screenModel.ContactNumber;
				screenModel.UserMeterInputFields.MeterReadingGas = progress.GasMeterReadingValue == 0 ? screenModel.UserMeterInputFields.MeterReadingGas : progress.GasMeterReadingValue.ToString();
				screenModel.UserMeterInputFields.MeterReadingNightStorageHeater = progress.ElectricityMeterReadingNightOrNshValue == 0 ? string.Empty : progress.ElectricityMeterReadingNightOrNshValue.ToString();
				screenModel.UserMeterInputFields.MeterReadingNight = progress.ElectricityMeterReadingNightOrNshValue == 0 ? screenModel.UserMeterInputFields.MeterReadingNight : progress.ElectricityMeterReadingNightOrNshValue.ToString();
				screenModel.UserMeterInputFields.MeterReading24Hrs = progress.ElectricityMeterReadingDayOr24HrsValue == 0 ? screenModel.UserMeterInputFields.MeterReading24Hrs : progress.ElectricityMeterReadingDayOr24HrsValue.ToString();
				screenModel.UserMeterInputFields.MeterReadingDay = progress.ElectricityMeterReadingDayOr24HrsValue == 0 ? screenModel.UserMeterInputFields.MeterReadingDay : progress.ElectricityMeterReadingDayOr24HrsValue.ToString();

				screenModel.UserConfirmationInputFields.InformationCollectionAuthorized = progress.HasConfirmedAuthority;
				screenModel.UserConfirmationInputFields.TermsAndConditionsAccepted = progress.HasConfirmedTermsAndConditions;
			}

            async Task<InstallationInfo> GetFirstInstallationFromPremise(string premiseName)
            {
                InstallationInfo installationInfo = null;
                var premiseAddressInfo = premiseAddressInfos.FirstOrDefault(x => x.PremiseName == premiseName);
                if (premiseAddressInfo!=null && premiseAddressInfo.PremiseId !=null)
                {
                    var installationDeviceInfoTask =
                        _queryResolver.GetPremise(premiseAddressInfo.PremiseId);
                    installationInfo = (await installationDeviceInfoTask).Installations.FirstOrDefault();
                }
                return installationInfo;
            }

            async Task MapElectricityData()
			{
				var electricityAddressInfo = premiseAddressInfos.FirstOrDefault(x => x.PremiseName == "MPRN");
				if (electricityAddressInfo != null && electricityAddressInfo.PremiseId != null)
				{
					screenModel = await PopulateElectricityMeterReading(electricityInstallationInfo, screenModel,
						(ElectricityPointReferenceNumber)step2StepData.NewMPRN);
				}
			}

			async Task MapGasData()
			{
				var gasAddressInfo = premiseAddressInfos.FirstOrDefault(x => x.PremiseName == "GPRN");
				if (gasAddressInfo != null)
				{
					screenModel = await PopulateGasMeterReading(gasInstallationInfo, screenModel,
						step2StepData.NewGPRN); 
				}
			}

			void MapMoveInDate()
			{
				var isNewAcquisitionElectricity = step2StepData.IsMPRNDeregistered || (screenModel.IsNewAcquisitionElectricity ?? false);
				screenModel.MoveInOutDatePicker.MovingInOutSelectedDateTime = 
					PopulateMoveInDate(step1StepData.MoveInOutDatePicker.MovingInOutSelectedDateTime,
                        electricityInstallationInfo, gasInstallationInfo, isNewAcquisitionElectricity);
				var difference = DateTime.Now.Date - screenModel.MoveInOutDatePicker.MovingInOutSelectedDateTime;
				if (difference == null) return;
				var diffDays = difference.Value.TotalDays;
				diffDays = diffDays > 30 ? 30 : diffDays;
				screenModel.MoveInOutDatePicker.Intervals = diffDays;
				var interval = screenModel.MoveInOutDatePicker.Intervals;
				var startDate = DateTime.Now.AddDays(-(Math.Abs(interval)));
				var endDate = DateTime.Now.AddDays(1);
				screenModel.MoveInOutDatePicker.SelectableDateRange = new DateTimeRange(startDate, endDate);
				screenModel.MoveInOutDatePicker.DatePickerTitle = $"Date of move in. Please use format DD / MM / YYYY and date in between {screenModel.MoveInOutDatePicker.SelectableDateRange.Start.ToString("yyyy-MM-dd")} and {screenModel.MoveInOutDatePicker.SelectableDateRange.End.ToString("yyyy-MM-dd")}";
				screenModel.MoveInOutDatePicker.DatePickerHoverPopupDescription =
					"Please select the date of move in from date range available";
				if (screenModel.GasAccountNumber != null && screenModel.ElectricityAccountNumber != null)
				{
					screenModel.MoveInOutDatePicker.DatePickerDescription =
						"This date must be the date of the first  meter readings for Electricity and Gas.";
				}
				else if (screenModel.ElectricityAccountNumber != null)
				{
					screenModel.MoveInOutDatePicker.DatePickerDescription =
						"This date must be the date of the first meter reading for Electricity.";
				}
				else if (screenModel.GasAccountNumber != null)
				{
					screenModel.MoveInOutDatePicker.DatePickerDescription =
						"This date must be the date of the first meter reading for Gas.";
				}
			}
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(
			IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalScreenModel.CloneDeep();
			var data = (ScreenModel) refreshedStepData;

			SetTitle(StepTitle, data);

			await BuildStepData(contextData, data,false);
			return data;
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
					.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError)
					.OnEventNavigatesTo(StepEvent.CancelMovingHouse,
						MovingHouseStep.Step0OperationSelection)
					.OnEventNavigatesToAsync(StepEvent.ToStep4,
						MovingHouseStep.Step4ConfigurePayment,
                        async() => !await HasMovingHouseValidationError(contextData),
                        "Business rules validators are valid")
                    .OnEventNavigatesToAsync(StepEvent.ToStep4, MovingHouseStep.ShowMovingHouseValidationError,
                        async() => await HasMovingHouseValidationError(contextData),
                        "Business rules validators are invalid")
                ;
		}

        async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();
            var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();

			var data = contextData.GetStepData<ScreenModel>();
            var query = new MovingHouseValidationQuery
            {
                ElectricityAccountNumber = rootData.ElectricityAccountNumber,
                GasAccountNumber = rootData.GasAccountNumber,
                NewMPRN = step2Data.NewMPRN,
                NewGPRN = step2Data.NewGPRN,
                ValidateMoveInDetails = true,
                MoveInDate = data.MoveInOutDatePicker.MovingInOutSelectedDateTime,
                ValidateElectricNewPremiseAddressInSwitch = true,
                IsNewAcquisitionElectricity = data.IsNewAcquisitionElectricity ?? false,
                IsMPRNAddressInSwitch = step2Data.IsMPRNAddressInSwitch,
                MovingHouseType = step1Data.MovingHouseType,
				InitiatedFromAccountNumber = rootData.InitiatedFromAccountNumber,
				ValidateContractSaleChecks = true
			};

            var validation = await _queryResolver.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
            return validation.Any(x => x.Output == OutputType.Failed);
        }

        protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.ToStep4)
			{
				var rootStepData = contextData.GetStepData<FlowInitializer.RootScreenModel>();
				var stepData = contextData.GetCurrentStepData<ScreenModel>();
				var command = await MapProgressCommand(stepData, rootStepData);
				await _commandDispatcher.ExecuteAsync(command);
			}

			await base.OnHandlingStepEvent(triggeredEvent, contextData);
			
			async Task<RecordMovingInProgress> MapProgressCommand(ScreenModel stepData, FlowInitializer.RootScreenModel rootStepData)
			{
				var account1Task = _queryResolver.GetAccountInfoByAccountNumber(rootStepData.InitiatedFromAccountNumber);
				var account2Task = _queryResolver.GetSecondaryAccount(rootStepData);

				var account1 = await account1Task;
				var account2 = await account2Task;

				var meterInputFields = stepData.UserMeterInputFields;
				int electricityMeterReading24HrsOrDayValue = 0;
				int electricityMeterReadingNightValueOrNshValue = 0;
				if (meterInputFields.Electricity24HrsDevicesFieldRequired || meterInputFields.ElectricityDayDevicesFieldRequired)
					electricityMeterReading24HrsOrDayValue = int.Parse((meterInputFields.MeterReading24Hrs ?? meterInputFields.MeterReadingDay) ?? 0.ToString());

				if (meterInputFields.ElectricityNightDevicesFieldRequired || meterInputFields.ElectricityNightStorageHeaterDevicesFieldRequired)
					electricityMeterReadingNightValueOrNshValue = int.Parse((meterInputFields.MeterReadingNight ?? meterInputFields.MeterReadingNightStorageHeater) ?? 0.ToString());

				int gasMeterReadingValue = 0;
				if (meterInputFields.GasDevicesFieldRequired)
				{
					gasMeterReadingValue = int.Parse(meterInputFields.MeterReadingGas ?? 0.ToString());
				}

				var command = new RecordMovingInProgress(
					contextData.GetStepData<Step0OperationSelection.ScreenModel>().MovingHouseType,
					account1, account2,
					electricityMeterReading24HrsOrDayValue,
					electricityMeterReadingNightValueOrNshValue,
					gasMeterReadingValue,
					stepData.MoveInOutDatePicker.MovingInOutSelectedDateTime.Value,
					stepData.ContactNumber, 
					stepData.UserConfirmationInputFields.InformationCollectionAuthorized,
					stepData.UserConfirmationInputFields.TermsAndConditionsAccepted
				);

				return command;
			}
		}
		
		public class ScreenModel : UiFlowScreenModel,  ICancellableMovingHouseStep,
			IElectricityAndGasMeterMovingHouse, IAcknowledgmentsPartMovingHouse, IMoveInOutDatePicker
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == MovingHouseStep.Step3InputMoveInPropertyDetails;
			}

			[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid contact number")]
			[RegularExpression(ReusableRegexPattern.ValidPhoneNumber, ErrorMessage =
				"Please enter a valid contact number")]
			public string ContactNumber { get; set; }
			public string InitiatedFromAccountNumber { get; set; }

			public IEnumerable<DeviceRegisterUiModel> GasDevicesMeterReadings { get; set; }
			public IEnumerable<DeviceRegisterUiModel> ElectricityDevicesMeterReadings { get; set; }
			public string ElectricityAccountNumber { get; set; }
			public string GasAccountNumber { get; set; }
			public InputFields UserMeterInputFields { get; set; } = new InputFields();
			public ConfirmationInputFields UserConfirmationInputFields { get; set; } = new ConfirmationInputFields();
			public MoveInOutDatePicker MoveInOutDatePicker { get; set; } = new MoveInOutDatePicker();
			public ShowTermsAcknowledgmentType? ShowTermsAcknowledgmentType { get; set; }

			public bool? IsNewAcquisitionElectricity { get; set; }
			public bool? IsNewAcquisitionGas { get; set; }
			public MovingHouseType MovingHouseType { get; set; }
		}
	}
}
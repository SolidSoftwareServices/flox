using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.Steps
{
	public class MeterReadingFlowInitializer : UiFlowInitializationStep<ResidentialPortalFlowType,
		MeterReadingFlowInitializer.RootScreenModel>
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent MeterReadingNotFound =
				new ScreenEvent(nameof(MeterReadingFlowInitializer),nameof(MeterReadingNotFound));
		}

		public enum StartType
		{
			Unknown = 0,
			ShowHistoryOnly = 1,
			ShowAndEditMeterReading = 2
		}

		private readonly IDomainQueryResolver _domainQueryResolver;

		private readonly IUserSessionProvider _userSessionProvider;

		public MeterReadingFlowInitializer(IUserSessionProvider userSessionProvider,
			IDomainQueryResolver domainQueryResolver)
		{
			_userSessionProvider = userSessionProvider;
			_domainQueryResolver = domainQueryResolver;
		}

		public override ResidentialPortalFlowType InitializerOfFlowType => ResidentialPortalFlowType.MeterReadings;

		public override bool Authorize()
		{
			return !_userSessionProvider.IsAnonymous();
		}

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(
			IScreenFlowConfigurator preStartCfg, UiFlowContextData contextData)
		{
			return preStartCfg
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MeterReadingStep.MeterReadingNotPresent)
				.OnEventNavigatesTo(StepEvent.MeterReadingNotFound, MeterReadingStep.MeterReadingNotPresent)
				.OnEventNavigatesTo(ScreenEvent.Start, MeterReadingStep.SubmitMeterReading,
					StartedInShowMeterReadingMode, "Show Submit Meter Reading");

			bool StartedInShowMeterReadingMode()
			{
				var rootStepData = contextData.GetStepData<RootScreenModel>(ScreenName.PreStart);
				return rootStepData.StartType == StartType.ShowAndEditMeterReading;
			}
		}

		protected override async Task<RootScreenModel> OnBuildStartData(UiFlowContextData contextData,
			RootScreenModel data)
		{
			var accountInfo = _domainQueryResolver.GetAccountInfoByAccountNumber(data.AccountNumber);

			data.AccountType = (await accountInfo).ClientAccountType;

			return data;
		}

		protected override async Task<ScreenEvent> OnResolveInitializationEventToTrigger(
			ScreenEvent defaultEventToTriggerAfter,
			UiFlowScreenModel screenModel)
		{
			ScreenEvent result = null;
			var data = (RootScreenModel) screenModel;
			var devices = await _domainQueryResolver.GetDevicesByAccount(data.AccountNumber);
			if (devices.SelectMany(x => x.Registers).Any())
				result = ScreenEvent.Start;
			else
				result = StepEvent.MeterReadingNotFound;

			return result;
		}

		
		public class RootScreenModel : InitialFlowScreenModel, IMeterReadingsInput
		{
			public string Partner { get; set; }

			public ClientAccountType AccountType { get; set; }

			public string AccountNumber { get; set; }


			public StartType StartType { get; set; }
		}
	}
}
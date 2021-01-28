using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.FlowDefinitions;
using NLog;

namespace EI.RP.WebApp.Flows.AppFlows.AccountAndMeterDetails.Steps
{
	public class AccountAndMeterDetails : AccountAndMeterDetailsScreen
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _domainQueryResolver;
		
		public AccountAndMeterDetails(
			IDomainQueryResolver domainQueryResolver)
		{
			_domainQueryResolver = domainQueryResolver;
		}

		public override ScreenName ScreenStep => AccountAndMeterDetailsStep.ShowAccountAndMeterDetails;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);

			var accountInfoTask = _domainQueryResolver.GetAccountInfoByAccountNumber(rootData.AccountNumber);
			var deviceInfoTask = _domainQueryResolver.GetDevicesByAccount(rootData.AccountNumber);

			var deviceInfos = (await deviceInfoTask).ToList();

			var accountInfo = await accountInfoTask;
			var stepData = new ScreenModel
            {
	            AccountName = accountInfo.Name,
				AccountNumber = accountInfo.AccountNumber
            };

            SetTitle(Title, stepData);

            var contractItem = accountInfo.BusinessAgreement.Contracts.FirstOrDefault();

			var meters = new List<ScreenModel.Meter>();
			foreach (var deviceInfo in deviceInfos)
			{
				var meter = new ScreenModel.Meter();

				if (contractItem != null)
				{
					var premise = contractItem.Premise;

					stepData.SiteAddress = premise.Address == null ? string.Empty : premise.Address.AsDescriptionText();

					meter.MeterPointRefNumberLabel = contractItem.Division == DivisionType.Gas ? "GPRN" : "MPRN";
					meter.MeterPointRefNumber = premise.PointOfDeliveries.FirstOrDefault()?.Prn ?? " ";
				}

				var billToAccountAddress = accountInfo.BusinessAgreement.BillToAccountAddress;
				stepData.BillingAddress = billToAccountAddress == null ? string.Empty : billToAccountAddress.AsDescriptionText();

				meter.NetworksMeterNumber = deviceInfo.SerialNum.Mask('*', deviceInfo.SerialNum.Length - 4);
				meter.Location = deviceInfo.DeviceLocation;
				meter.Description = deviceInfo.DeviceDescription;

				meter.CanShowSmartCommsLevel = deviceInfo.IsSmart && deviceInfo.MCCConfiguration != RegisterConfigType.MCC16;
				meter.CommsLevel =
                    (deviceInfo.CTF ?? CommsTechnicallyFeasibleValue.NotAvailableYet) == CommsTechnicallyFeasibleValue.NotAvailableYet
						? "Pending"
						: $"Level {deviceInfo.CTF.ToString().Replace("0", string.Empty)}";
				
				meters.Add(meter);
			}
			stepData.CanDownloadConsumptionData = ResolveCanDownloadConsumptionLink();
			stepData.Meters = meters.ToArray();

			return stepData;


			bool ResolveCanDownloadConsumptionLink()
			{
				var last2Years = new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today);

				return accountInfo.SmartPeriods.Any(x => x.Intersects(last2Years))

				                                      || deviceInfos.Any(x => x.SmartActivationStatus == SmartActivationStatus.SmartActive);
			}
		}
		
        public static class StepEvent
		{
		}

		public class ScreenModel : UiFlowScreenModel
		{

			public string AccountNumber { get;set; }
			public string AccountName { get;set; }
            public string SiteAddress { get; set; }
			public string BillingAddress { get; set; }
            public Meter[] Meters { get; set; }
			public bool CanDownloadConsumptionData { get; set; }

			

			public class Meter
			{
                public string MeterPointRefNumberLabel { get; set; }
                public string MeterPointRefNumber { get; set; }
                public string NetworksMeterNumber { get; set; }
                public string Description { get; set; }
                public string Location { get; set; }
				public bool? CanShowSmartCommsLevel { get; set; }
				public string CommsLevel { get; set; }
            }

            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == AccountAndMeterDetailsStep.ShowAccountAndMeterDetails;
            }
		}
	}
}
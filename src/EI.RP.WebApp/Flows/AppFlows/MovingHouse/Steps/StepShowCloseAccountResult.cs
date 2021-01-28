using System;
using System.Collections.Generic;

using EI.RP.UiFlows.Core.Flows;

using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps.Extensions;


namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
    public class StepShowCloseAccountResult : MovingHouseScreen
	{
		protected override string Title => "Confirmation | Close Account";

		public override ScreenName ScreenStep => MovingHouseStep.CloseAccountConfirmation;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
		}

		private readonly IDomainQueryResolver _queryResolver;

		public StepShowCloseAccountResult(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}


		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootStepData = contextData.GetStepData<FlowInitializer.RootScreenModel>();

			var getAccounts =  _queryResolver.GetFlowAccounts(rootStepData);

			var stepData = new ScreenModel
			{
				MovingType = contextData.GetStepData<Step0OperationSelection.ScreenModel>().MovingHouseType,
				
				LastStepEvent = contextData.EventsLog.Last().Event,
				ElectricityAccountNumber = rootStepData.ElectricityAccountNumber,
				GasAccountNumber = rootStepData.GasAccountNumber
			};

			SetTitle(Title, stepData);

			var accounts = await getAccounts;
			MapItems();
			return stepData;

			void MapItems()
			{
				
				var gasAccount =
					accounts.SingleOrDefault(x => x.ClientAccountType == ClientAccountType.Gas);
				var electricityAccount =
					accounts.SingleOrDefault(x => x.ClientAccountType == ClientAccountType.Electricity);
				var gasPaymentType = gasAccount == null
					? string.Empty
					: (gasAccount.PaymentMethod == string.Empty ? "manually" : "direct debit");
				var electricityPaymentType = electricityAccount == null
					? string.Empty
					: (electricityAccount.PaymentMethod == string.Empty ? "manually" : "direct debit");

				stepData.ElectricityPaymentInfo = $"Your Electricity bill will be paid {electricityPaymentType}.";
				stepData.GasPaymentInfo = $"Your Gas bill will be paid {gasPaymentType}.";
				stepData.CloseConfirmationHeader = "Your accounts have been closed.";

				if (stepData.LastStepEvent == Step0OperationSelection.StepEvent.CloseElectricitySelected)
				{
					stepData.GasPaymentInfo = String.Empty;
					stepData.CloseConfirmationHeader = "Your account has been closed.";
				}
				else if (stepData.LastStepEvent == Step0OperationSelection.StepEvent.CloseGasSelected)
				{
					stepData.ElectricityPaymentInfo = String.Empty;
					stepData.CloseConfirmationHeader = "Your account has been closed.";
				}
			}
		}

		public class ScreenModel : UiFlowScreenModel
        {
			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == MovingHouseStep.CloseAccountConfirmation;
			}
			public string LastStepEvent { get; set; }
			
			public string ElectricityAccountNumber { get; set; }
			public string GasAccountNumber { get; set; }
			public string ElectricityPaymentInfo { get; set; }
			public string GasPaymentInfo { get; set; }
			public string CloseConfirmationHeader { get; set; }
			public MovingHouseType MovingType { get; set; }
        }
	}
}
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using System.Linq;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps
{
    public class Step2ConfirmAddress : MovingHouseScreen
    {
	    public static class StepEvent
	    {
		    public static readonly ScreenEvent Reload = new ScreenEvent(nameof(Step2ConfirmAddress),nameof(Reload));
		    public static readonly ScreenEvent ToStep3 = new ScreenEvent(nameof(Step2ConfirmAddress), nameof(ToStep3));
	    }

		public override ScreenName ScreenStep => MovingHouseStep.Step2ConfirmAddress;

        private readonly IDomainQueryResolver _queryResolver;

        public Step2ConfirmAddress(IDomainQueryResolver queryResolver, IConfigurableTestingItems configurableTestingItems)
        {
	        _queryResolver = queryResolver;
	        _configurableTestingItems = configurableTestingItems;
        }
        private readonly IConfigurableTestingItems _configurableTestingItems;
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
	        var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();

            var stepData = new ScreenModel();

            await BuildStepData(step2Data,stepData);

            return stepData;
        }

        private async Task BuildStepData(Step2InputPrns.ScreenModel step2Data, ScreenModel screenModel)
        {
            screenModel.NewMPRN = step2Data.NewMPRN;
            screenModel.NewGPRN = step2Data.NewGPRN;
            screenModel.MovingHouseType = step2Data.MovingHouseType;
            screenModel.MovingHouseTitle = screenModel.MovingHouseType.ToDescriptionText();
            var premiseAddressInfos = new List<ScreenModel.PremiseInfo>();
            foreach (var item in step2Data.PremiseAddressInfos)
            {
                var premiseInfo = new ScreenModel.PremiseInfo();
                premiseInfo.PremiseName = item.PremiseName;
                premiseInfo.PRN = item.PRN;
                premiseInfo.CareOf = item.CareOf;
                premiseInfo.City = item.City;
                premiseInfo.HouseNo = item.HouseNo;
                premiseInfo.PostalCode = item.PostalCode;
                premiseInfo.Street = item.Street;
                premiseInfo.PremiseId = item.PremiseId;
                premiseAddressInfos.Add(premiseInfo);
            }

            screenModel.PremiseAddressInfos = premiseAddressInfos;

            SetTitle(string.Join(" | ", $"2. New {screenModel.MovingHouseType.ToPrnText()}", Title), screenModel);
		}

        protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
            UiFlowScreenModel originalScreenModel,
            IDictionary<string, object> stepViewCustomizations = null)
        {
            var refreshedStepData = originalScreenModel.CloneDeep();
            var data = (ScreenModel)refreshedStepData;

            var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();
            await BuildStepData(step2Data,data);
            return data;
        }

    
        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
	            .OnEventNavigatesTo(ScreenEvent.ErrorOccurred, MovingHouseStep.ShowMovingHouseUnhandledError)
                .OnEventNavigatesTo(StepEvent.Reload, MovingHouseStep.Step2InputPrns)
                .OnEventNavigatesToAsync(StepEvent.ToStep3, MovingHouseStep.Step3InputMoveInPropertyDetails,
                    async () => !await HasMovingHouseValidationError(contextData),
                            "Business rules validators are valid")
                .OnEventNavigatesToAsync(StepEvent.ToStep3, MovingHouseStep.ShowMovingHouseValidationError,
                    async () => await HasMovingHouseValidationError(contextData),
                            "Business rules validators are invalid");
        }

        async Task<bool> HasMovingHouseValidationError(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<FlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var step1Data = contextData.GetStepData<Step1InputMoveOutPropertyDetails.ScreenModel>();
            var step2Data = contextData.GetStepData<Step2InputPrns.ScreenModel>();
            var query = new MovingHouseValidationQuery
            {
                ElectricityAccountNumber = rootData.ElectricityAccountNumber,
                GasAccountNumber = rootData.GasAccountNumber,
                ValidateNewPremisePodNotRegisteredToday = true,
                IsMPRNDeregistered = step2Data.IsMPRNDeregistered,
                IsGPRNDeregistered = step2Data.IsGPRNDeregistered,
				NewMPRN = step2Data.NewMPRN,
                NewGPRN = step2Data.NewGPRN,
                MoveOutDate = step1Data.MoveInOutDatePicker.MovingInOutSelectedDateTime
            };

            var validation = await _queryResolver.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(query);
            return validation.Any(x => x.Output == OutputType.Failed);
        }

        public class ScreenModel : UiFlowScreenModel
        {
	        public string NewGPRN { get; set; }
	        public string NewMPRN { get; set; }
            public IEnumerable<PremiseInfo> PremiseAddressInfos { get; set; } = new PremiseInfo[0];
            public string MovingHouseTitle { get; set; }
            public MovingHouseType MovingHouseType { get; set; }

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
            }
        }
    }
}
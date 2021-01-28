using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
    public class ShowFlowGenericError : SmartActivationScreen
    {
        public static class StepEvent
        {
        //DEFINE HERE SCREEN EVENTS that this screen can receive. Usually these correspond to user acitons
        }

        private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        public ShowFlowGenericError(IDomainQueryResolver domainQueryResolver, IDomainCommandDispatcher domainCommandDispatcher)
        {
            _domainQueryResolver = domainQueryResolver;
            _domainCommandDispatcher = domainCommandDispatcher;
        }

        public override ScreenName ScreenStep => SmartActivationStep.ShowFlowGenericError;
        //TIP: inspect all overridable members and look for usages in the existing code to see weorking examples
        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            //handle the step data creation here
            var screenModel = new ScreenModel();

            SetTitle(Title, screenModel);

            return screenModel;
        }

        //protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration,
        //	IUiFlowContextData contextData)
        //{
        //	//define here what are the pages to be browsed on event in this screen
        //	return base.OnDefiningTransitionsFromCurrentScreen(screenConfiguration, contextData);
        //}
        //protected override Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
        //	IDictionary<string, object> stepViewCustomizations = null)
        //{
        //	//handle the step data updates in subsequent requests here
        //	return base.OnRefreshStepDataAsync(contextData, originalScreenModel, stepViewCustomizations);
        //}
        //this is the View model
        public sealed class ScreenModel : UiFlowScreenModel
        {
            public ScreenModel(): base(false)
            {
            }

            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == SmartActivationStep.ShowFlowGenericError;
            }
        }
    }
}
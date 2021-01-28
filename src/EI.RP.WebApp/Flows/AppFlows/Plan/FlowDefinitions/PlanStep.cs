using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.FlowDefinitions
{
    /// <summary>
    ///These are the steps of your flow(Plan) each step is defined by an screen and a template
    /// you can reach the step by defining a navigation from another step or the flow initializer
    /// </summary>
    public static class PlanStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName MainScreen = new ScreenName(nameof(MainScreen));
    }
}
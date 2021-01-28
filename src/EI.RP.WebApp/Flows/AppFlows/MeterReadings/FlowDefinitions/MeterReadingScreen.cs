

namespace EI.RP.WebApp.Flows.AppFlows.MeterReadings.FlowDefinitions
{
    public abstract class MeterReadingScreen : ResidentialPortalScreen
    {
        public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.MeterReadings;

        protected virtual string Title => "Meter Reading";
    }
}

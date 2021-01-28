using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.StringResources;

namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.Steps
{
    public class CollectAccountConsumptionDetails : AddGasAccountScreen
    {
        public override ScreenName ScreenStep => AddGasAccountStep.CollectAccountConsumptionDetails;

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var rootData = contextData.GetStepData<AddGasAccountFlowInitializer.RootScreenModel>(ScreenName.PreStart);
            var stepData = new ScreenModel
            {
	            ElectricityAccountNumber = rootData.ElectricityAccountNumber
            };

            SetTitle(Title, stepData);

            return stepData;
        }

        public static class StepEvent
        {
            public static readonly ScreenEvent Submit = new ScreenEvent(nameof(CollectAccountConsumptionDetails),nameof(Submit));
        }

        protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
            IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
        {
            return screenConfiguration
                .OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
                .OnEventNavigatesTo(StepEvent.Submit, AddGasAccountStep.ConfirmAddress);
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public string ElectricityAccountNumber { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter a valid GPRN with 7 digits")]
            [RegularExpression(GasPointReferenceNumber.GPRNRegEx, ErrorMessage = "You must enter a valid GPRN with 7 digits")]
            public string GPRN { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid meter reading")]
            [RegularExpression(ReusableRegexPattern.ValidMeterReading, ErrorMessage = "Please enter a valid meter reading")]
            public string GasMeterReading { get; set; }

            [Range(typeof(bool), "true", "true", ErrorMessage = "You must check this box to proceed")]
            public bool AuthorizationCheck { get; set; }

            [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
            public bool TermsAndConditionsAccepted { get; set; }

            [Range(typeof(bool), "true", "true", ErrorMessage = "You must check this box to proceed")]
            public bool DebtFlagAndArrearsTermsAndConditions { get; set; }

            [Range(typeof(bool), "true", "true", ErrorMessage = "You must check this box to proceed")]
            public bool PricePlanTermsAndConditions { get; set; }

            public void Reset()
            {
	            GPRN = GasMeterReading = null;
	            AuthorizationCheck = TermsAndConditionsAccepted =
		            DebtFlagAndArrearsTermsAndConditions = PricePlanTermsAndConditions = false;
            }
        }
    }
}

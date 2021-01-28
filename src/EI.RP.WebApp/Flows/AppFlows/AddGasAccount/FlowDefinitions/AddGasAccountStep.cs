using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;

namespace EI.RP.WebApp.Flows.AppFlows.AddGasAccount.FlowDefinitions
{
    public static class AddGasAccountStep
    {
        public static readonly ScreenName ShowFlowGenericError = new ScreenName(nameof(ShowFlowGenericError));
        public static readonly ScreenName CollectAccountConsumptionDetails = new ScreenName(nameof(CollectAccountConsumptionDetails));
        public static readonly ScreenName ConfirmAddress = new ScreenName(nameof(ConfirmAddress));
        
        public static readonly ScreenName ShowOperationWasCompleted = new ScreenName(nameof(ShowOperationWasCompleted));
        public static readonly ScreenName ExecutePaymentConfigurationFlowThenStoreResults = new ScreenName(nameof(ExecutePaymentConfigurationFlowThenStoreResults));
        public static readonly ScreenName ShowErrorMessage = new ScreenName(nameof(ShowErrorMessage));
    }
}

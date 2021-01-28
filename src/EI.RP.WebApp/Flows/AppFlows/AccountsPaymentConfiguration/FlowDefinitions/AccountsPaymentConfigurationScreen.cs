using System;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;

namespace EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions
{
	public abstract class AccountsPaymentConfigurationScreen : ResidentialPortalScreen
	{
		public override ResidentialPortalFlowType IncludedInFlowType => ResidentialPortalFlowType.AccountsPaymentConfiguration;

		
	}

	
}
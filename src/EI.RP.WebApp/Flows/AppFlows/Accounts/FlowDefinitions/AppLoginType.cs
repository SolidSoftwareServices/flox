using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions
{
	public class AppLoginType : TypedStringValue<AppLoginType>
	{


        [JsonConstructor]
        private AppLoginType()
        {
        }

        private AppLoginType(string value) : base(value)
        {
        }
        public static readonly AppLoginType Default = new AppLoginType(nameof(Default));

		public static readonly AppLoginType SmartHubPage =new AppLoginType(nameof(SmartHubPage));
        public static readonly AppLoginType MeterReading = new AppLoginType(nameof(MeterReading));
        public static readonly AppLoginType AddGas = new AppLoginType(nameof(AddGas));
        public static readonly AppLoginType MarketingPreferences = new AppLoginType(nameof(MarketingPreferences));

		/// <summary>
		/// Login used by internal deployments
		/// </summary>
        public static readonly AppLoginType InternalDeploymentLogin = new AppLoginType(nameof(InternalDeploymentLogin));
	}
}
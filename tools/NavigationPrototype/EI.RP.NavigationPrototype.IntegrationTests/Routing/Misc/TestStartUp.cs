using System.Reflection;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.UiFlows.Mvc.AppFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EI.RP.NavigationPrototype.IntegrationTests.Routing.Misc
{
	public class TestStartUp : Startup
	{
		public TestStartUp(IConfiguration configuration) : base(configuration)
		{
		}

		protected override void AddMvc(IServiceCollection services)
		{
			services.AddMvc()
				.AddApplicationPart(Assembly.GetExecutingAssembly()).AddControllersAsServices()
				.AddUiFlows<SampleAppFlowType, RoutingUiFlowController>(services,false)
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}
	}
}
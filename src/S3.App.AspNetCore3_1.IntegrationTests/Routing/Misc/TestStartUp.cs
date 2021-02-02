using System.Reflection;
using S3.UiFlows.Mvc.AppFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using S3.App.Flows.AppFlows;

namespace S3.App.AspNetCore3_1.IntegrationTests.Routing.Misc
{
	public class TestStartUp : Startup
	{
		public TestStartUp(IConfiguration configuration) : base(configuration)
		{
		}

		protected override void AddMvc(IServiceCollection services)
		{
			services.AddMvc(o =>
				{
					o.EnableEndpointRouting = false;
				})
				.AddApplicationPart(Assembly.GetExecutingAssembly()).AddControllersAsServices()
				.AddUiFlows<SampleAppFlowType, RoutingUiFlowController>(services);

		}
	}
}
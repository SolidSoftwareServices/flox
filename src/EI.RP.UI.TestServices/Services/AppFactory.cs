using Ei.Rp.Mvc.Core.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EI.RP.UI.TestServices.Services
{
	public class AppFactory<TStartUp>
		: WebApplicationFactory<TStartUp> where TStartUp : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services => { });
		}

		protected override IWebHostBuilder CreateWebHostBuilder()
		{
			return WebHost.CreateDefaultBuilder(new string[0])
				.CreateDefaultIISWebHostBuilder<TStartUp>(environmentName:"Test");
		}
	}
}
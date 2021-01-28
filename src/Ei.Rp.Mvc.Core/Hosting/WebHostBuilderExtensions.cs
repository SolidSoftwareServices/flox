using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Ei.Rp.Mvc.Core.Hosting
{
	public static class WebHostBuilderExtensions
	{
		// ReSharper disable once InconsistentNaming
		public static IWebHostBuilder CreateDefaultIISWebHostBuilder<TStartUp>(this IWebHostBuilder builder,
			bool useKestrel=true, string environmentName = null)
			where TStartUp : class
		{
			if (environmentName != null)
			{
				builder = builder.UseEnvironment(environmentName);
			}

			if (useKestrel)
			{
				builder = builder.UseKestrel(o=>o.AddServerHeader=false);
			}

			return builder
				.ConfigureServices(services => services.AddAutofac())
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<TStartUp>()
				.ConfigureLogging(logging =>
				{
					logging.ClearProviders();
					logging.SetMinimumLevel(LogLevel.Trace);
				})
				.UseNLog();
		}
	}
}
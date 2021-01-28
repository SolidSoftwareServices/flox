using System;
using Ei.Rp.Mvc.Core.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Web;

namespace EI.RP.WebApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			try
			{
				logger.Debug(() => "Bootstrapping app...");

				var webHost = CreateWebHostBuilder(args)
					.Build();
				logger.Info(() => "App Starting..");
				webHost.Run();
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Stopped program because of exception");
				throw;
			}
			finally
			{
				LogManager.Shutdown();
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args).CreateDefaultIISWebHostBuilder<Startup>();
		}
	}
}
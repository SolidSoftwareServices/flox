using System;
using Autofac.Extensions.DependencyInjection;
using Ei.Rp.Mvc.Core.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace EI.RP.NavigationPrototype
{
	public class Program
	{

		public static void Main(string[] args)
		{
			try
			{
				CreateWebHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				LogManager.GetCurrentClassLogger().Fatal(ex);
				throw;
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			return WebHost.CreateDefaultBuilder(args).CreateDefaultIISWebHostBuilder<Startup>();
		}
	}
}
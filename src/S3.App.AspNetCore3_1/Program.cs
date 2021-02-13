using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;

namespace S3.App
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

		public static IHostBuilder CreateWebHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.UseServiceProviderFactory(new AutofacServiceProviderFactory())
				.ConfigureWebHostDefaults(builder =>
				{
					builder
						.UseContentRoot(Directory.GetCurrentDirectory())
						.UseIISIntegration()
						.UseStartup<Startup>();
				});
		}
	}
}
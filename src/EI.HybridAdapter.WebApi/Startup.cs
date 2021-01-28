using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using EI.HybridAdapter.WebApi.Infrastructure.Bootstrap;
using EI.HybridAdapter.WebApi.Infrastructure.IoC;
using EI.HybridAdapter.WebApi.Infrastructure.Settings;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using EI.RP.CoreServices.IoC.Autofac;
using Ei.Rp.Mvc.Core.Middleware;
using Ei.Rp.Mvc.Core.Profiler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Web;

namespace EI.HybridAdapter.WebApi
{
	public class Startup
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IServiceProvider _serviceProvider;
		private readonly IHostingEnvironment _environment;

		public Startup(IHostingEnvironment environment, IServiceProvider serviceProvider)
		{
			if (!environment.IsEnvironmentConfiguredCorrectly())
				throw new InvalidProgramException(
					"The environment has not been configured yet, Set environment variable: 'ASPNETCORE_ENVIRONMENT'");

			Logger.Info(()=>$"Environment configured is {environment.EnvironmentName}");
			Logger.Info(()=>$"Running version {Assembly.GetEntryAssembly().GetName().Version}");
			_environment = environment;
			_serviceProvider = serviceProvider;

			ConfigureLoggingSettings();
			var builder = ConfigureEnvironmentSettings();
			Configuration = builder.Build();
			AppSettings = new AppSettings(Configuration, serviceProvider);

			IConfigurationBuilder ConfigureEnvironmentSettings()
			{
				var appConfig = new ConfigurationBuilder()
					.SetBasePath(environment.ContentRootPath)
					.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", false)
					.AddEnvironmentVariables();
				return appConfig;
			}

			void ConfigureLoggingSettings()
			{
				if (LogManager.Configuration.FileNamesToWatch.Any())
					NLogBuilder.ConfigureNLog(Path.Combine(environment.ContentRootPath,
						$"nlog.{environment.EnvironmentName}.config"));
			}
		}

		private AppSettings AppSettings { get; }
		public IContainer ApplicationContainer { get; private set; }
		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddProfiler(IsProfilerEnabled);
			services.AddApiVersioning(c =>
			{
				c.DefaultApiVersion = new ApiVersion(1, 0);
				c.AssumeDefaultVersionWhenUnspecified = true;
			});
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.RegisterSwagger(_environment);
			services.RegisterAuthx(AppSettings);
			services.RegisterAutofacContainer();
		}

		//gets called by infrastructure after ConfigureServices
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new IoCRegistrationModule());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (!env.IsProductionEnvironment())
				app.UseDeveloperExceptionPage();
			else
				app.UseHsts();

			app.ConfigureSwagger(_environment);
			app.ConfigureAuthentication();
			app.UseRequestResponseLogging();

			app.UseHttpsRedirection();
			app.UseMvc();
		}

		private bool IsProfilerEnabled()
		{
			return _environment.IsDevelopmentEnv() || _environment.IsDevelopmentDemoEnv() || _environment.IsTest();
		}
	}
}
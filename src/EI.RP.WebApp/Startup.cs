using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.DependencyInjection;
using Ei.Rp.Mvc.Core.Controllers;
using Ei.Rp.Mvc.Core.Middleware;
using Ei.Rp.Mvc.Core.Profiler;
using EI.RP.UiFlows.Mvc.AppFeatures;
using Ei.Rp.Web.DebugTools;
using Ei.Rp.Web.DebugTools.Infrastructure.Middleware;
using EI.RP.WebApp.Controllers;
using EI.RP.WebApp.Controllers.Membership;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Infrastructure.HealthChecks;
using EI.RP.WebApp.Infrastructure.IoC;
using EI.RP.WebApp.Infrastructure.Middleware;
using EI.RP.WebApp.Infrastructure.Session;
using EI.RP.WebApp.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;

namespace EI.RP.WebApp
{
	public class Startup
	{
		private readonly IHostingEnvironment _environment;
		private readonly IServiceProvider _serviceProvider;

		public Startup(IHostingEnvironment environment, IServiceProvider serviceProvider)
		{
			if (!environment.IsEnvironmentConfiguredCorrectly())
				throw new InvalidProgramException(
					"The environment has not been configured yet, Set environment variable: 'ASPNETCORE_ENVIRONMENT'");

			_environment = environment;
			_serviceProvider = serviceProvider;

			ConfigureLoggingSettings();
			var builder = ConfigureEnvironmentSettings();
			Configuration = builder.Build();
			AppSettings = new AppSettings(Configuration, serviceProvider);
			this.ServiceProvider = serviceProvider;
			IConfigurationBuilder ConfigureEnvironmentSettings()
			{
				var appConfig = new ConfigurationBuilder()
					.SetBasePath(environment.ContentRootPath)
					.AddJsonFile($"appsettings.json", false,true)
					.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", false,true);
					
				return appConfig;
			}

			void ConfigureLoggingSettings()
			{
				if (LogManager.Configuration.FileNamesToWatch.Any())
					NLogBuilder.ConfigureNLog(Path.Combine(environment.ContentRootPath,
						$"nlog.{environment.EnvironmentName}.config"));
			}
		}

		public IServiceProvider ServiceProvider
		{
			get;
		}

		private AppSettings AppSettings { get; }

		public IContainer ApplicationContainer { get; private set; }
		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var isInternal = AppSettings.IsInternalDeployment;
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			services.AddLocalization();
			services.AddAuthentication(o =>
			{
				o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
			{
				
				var cookieBuilder = opts.Cookie;
				//TODO: SPECIFY THE COOKIE name & EXPIRATION also missing domain etc
				cookieBuilder.Name = $"EI.RP{(isInternal?".Internal":null)}";
				cookieBuilder.Expiration = TimeSpan.FromMinutes(20);
				cookieBuilder.MaxAge = TimeSpan.FromMinutes(20);
				cookieBuilder.HttpOnly = true;
				cookieBuilder.SameSite = SameSiteMode.Strict;
				cookieBuilder.SecurePolicy = CookieSecurePolicy.Always;

				var baseUrl = _serviceProvider.Resolve<IHttpContextAccessor>().HttpContext.Request.PathBase;
				var prefix = $"{baseUrl}/{typeof(LoginController).GetNameWithoutSuffix()}";
				opts.LoginPath = $"{prefix}/{nameof(LoginController.Login)}";
				opts.LogoutPath = $"{prefix}/{nameof(LoginController.Logout)}";
				opts.AccessDeniedPath = $"{prefix}/{nameof(LoginController.AccessDenied)}";

				opts.SlidingExpiration = true;
				opts.SessionStore=new InMemorySessionStore();
			});

			services.AddIdentityCore<ClaimsPrincipal>()
				.AddSignInManager<SignInManager>();
			services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
			services.AddAuthorization();
			services.AddProfiler(IsProfilerEnabled);
			services.AddMvc(ops => { })
				.AddUiFlows<ResidentialPortalFlowType>(services, AppSettings.FlowDebuggerIsEnabled)
				//do not change compatibility version without a full regression test
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			services.AddAntiforgery(antiForgeryOptions =>
			{
				antiForgeryOptions.Cookie.HttpOnly = true;
				antiForgeryOptions.Cookie.SameSite = SameSiteMode.Strict;
				antiForgeryOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});

			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			services.AddSession(opts =>
			{
				opts.Cookie.HttpOnly = true;
				opts.Cookie.IsEssential = true;
				opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});
			services.RegisterAutofacContainer();
		}

		//gets called by infrastructure after ConfigureServices
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new IoCRegistrationModule(Configuration,ServiceProvider));
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			var fileExtProvider = new FileExtensionContentTypeProvider();
            fileExtProvider.Mappings.Add(".webmanifest", "application/manifest+json");
            fileExtProvider.Mappings.Add("js.map", "application/json");
            fileExtProvider.Mappings.Add("css.map", "application/octet-stream");
			app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = fileExtProvider
            });

			app.UseCookiePolicy();

			if (env.IsDevelopmentEnv() )
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler(
					$"/{typeof(ErrorController).GetNameWithoutSuffix()}/{nameof(ErrorController.Error)}");
				app.UseHsts();
			}
			app.UseErrorHandlingMiddleware(
				$"/{typeof(ErrorController).GetNameWithoutSuffix()}/{nameof(ErrorController.Error)}");
			if (!(_environment.IsPreProduction() || env.IsProductionEnvironment()))
				app.UseContinuousDeliveryMiddleware();

			UseLocalization();
			app.UseRequestResponseLogging();
			app.UseSession();
			app.UseAuthorizationMiddleware(
				typeof(LoginController), nameof(LoginController.Login),
				typeof(LoginController), nameof(LoginController.Login));

			app.UseAuthentication();
			app.UseProfiler(IsProfilerEnabled);
			Action<IRouteBuilder> configureRoutes = routes =>
			{
				routes.MapRoute(
					"default",
					$"{{controller={typeof(LoginController).GetNameWithoutSuffix()}}}/{{action={nameof(LoginController.Login)}}}/{{id?}}");

				routes.MapRoute(
					"defaultToIndex",
					"{controller}/{action=Index}");
			};

			if (Debugger.IsAttached)
			{
				app.UseMvcWithUiFlowsDebugger(configureRoutes);
			}
			else
			{
				app.UseMvc(configureRoutes);
			}
			void UseLocalization()
			{
				var culture = "en-IE";
				var enIeCulture = new CultureInfo(culture);
				var supportedCultures = enIeCulture.ToOneItemArray();
				app.UseRequestLocalization(new RequestLocalizationOptions
				{
					DefaultRequestCulture = new RequestCulture(culture),
					SupportedCultures = supportedCultures,
					SupportedUICultures = supportedCultures,
					FallBackToParentCultures = false,
					FallBackToParentUICultures = false
				});
				CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = enIeCulture;
			}
		}

		

		private bool IsProfilerEnabled()
		{
			return _environment.IsDevelopmentEnv() || _environment.IsDevelopmentDemoEnv() || _environment.IsTest();
		}

		private class SignInManager : SignInManager<ClaimsPrincipal>
		{
			public SignInManager(
				UserManager<ClaimsPrincipal> userManager,
				IHttpContextAccessor contextAccessor,
				IUserClaimsPrincipalFactory<ClaimsPrincipal> claimsFactory,
				IOptions<IdentityOptions> optionsAccessor,
				ILogger<SignInManager<ClaimsPrincipal>> logger,
				IAuthenticationSchemeProvider schemes)
				: base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
			{
			}

			public override async Task SignOutAsync()
			{
				await Context.SignOutAsync(IdentityConstants.ApplicationScheme);
			}
		}
	}
}
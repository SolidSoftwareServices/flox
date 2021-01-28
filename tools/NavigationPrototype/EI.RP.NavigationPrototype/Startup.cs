using System;
using System.Diagnostics;
using Autofac;
using Ei.Rp.Mvc.Core.Middleware;
using Ei.Rp.Mvc.Core.Profiler;
using Ei.Rp.Web.DebugTools;
using EI.RP.NavigationPrototype.Data;
using EI.RP.NavigationPrototype.Flows.AppFlows;
using EI.RP.NavigationPrototype.Infrastructure.IoC;
using EI.RP.UiFlows.Mvc.AppFeatures;
using Ei.Rp.Web.DebugTools.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EI.RP.NavigationPrototype
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));
			services.AddDefaultIdentity<IdentityUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromSeconds(10);
				options.Cookie.HttpOnly = true;
				// Make the session cookie essential
				options.Cookie.IsEssential = true;
			});
			services.AddHttpContextAccessor();
			services.AddProfiler(()=>true);
			

			AddMvc(services);
		}

		protected virtual void AddMvc(IServiceCollection services)
		{
			services.AddMvc()
				.AddUiFlows<SampleAppFlowType>(services,true)
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		//gets called by infrastructure after ConfigureServices
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new IoCRegistrationModule());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseSession();
			app.UseRequestResponseLogging();
			

			app.UseAuthentication();
			app.UseProfiler(()=>true);
			app.UseMvcWithUiFlowsDebugger(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}");
			});
		}
	}
}
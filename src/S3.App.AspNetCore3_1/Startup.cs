using System;
using Autofac;
using S3.Mvc.Core.Profiler;
using S3.App.AspNetCore3_1.Data;
using S3.App.AspNetCore3_1.Flows.AppFlows;
using S3.App.AspNetCore3_1.Infrastructure.IoC;
using S3.UiFlows.Mvc.AppFeatures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace S3.App.AspNetCore3_1
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
			services.AddMvc(opts =>
				{
					opts.EnableEndpointRouting = false;
				})
				.AddUiFlows<SampleAppFlowType>(services);
		}

		//gets called by infrastructure after ConfigureServices
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new IoCRegistrationModule());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
			

			app.UseAuthentication();
			app.UseProfiler(()=>true);
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"default",
					"{controller=Home}/{action=Index}");
			});
		}
	}
}
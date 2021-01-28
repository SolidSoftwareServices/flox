using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EI.HybridAdapter.WebApi.Infrastructure.Bootstrap
{
	internal static class SwaggerRegistrationExtensions
	{
		public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app,IHostingEnvironment hostingEnvironment)
		{
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("v1/swagger.json", $"HybridAdapter API Version 1.Environment:{hostingEnvironment.EnvironmentName}");
			});
			return app;
		}

		public static IServiceCollection RegisterSwagger(this IServiceCollection services,
			IHostingEnvironment hostingEnvironment)
		{


			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo()
				{
					Version = "v1",
					Title = $"Azure hybrid adapter on-prem. Environment:{hostingEnvironment.EnvironmentName}. Assembly version:{Assembly.GetEntryAssembly()?.GetName().Version.ToString()??"NOT SET"}",

				});
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

				c.IncludeXmlComments(xmlPath);
			});
			return services;
		}
	}
}

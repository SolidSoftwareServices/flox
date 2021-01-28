using System.Globalization;
using System.Threading.Tasks;
using EI.HybridAdapter.WebApi.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace EI.HybridAdapter.WebApi.Infrastructure.Bootstrap
{
	internal static class AuthxRegistrationExtensions
	{
		public static IApplicationBuilder ConfigureAuthentication(this IApplicationBuilder app)
		{
			app.UseAuthentication();
			return app;
		}

		public static IServiceCollection RegisterAuthx(this IServiceCollection services,IAzureSettings settings)
		{
			Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.Authority = string.Format(CultureInfo.InvariantCulture, settings.AzureAdInstance, settings.AzureAdTenant);
					options.Audience = settings.JwtAudience;
					
					options.Events=new AuthenticationEventsLogger();
				});
			return services;
		}

		private class AuthenticationEventsLogger : JwtBearerEvents
		{
			private static readonly ILogger Logger = LogManager.GetLogger("Authentication");

			public override Task AuthenticationFailed(AuthenticationFailedContext context)
			{
				Logger.Info(() => $"{nameof(AuthenticationFailed)}: {context.Exception}");
				return base.AuthenticationFailed(context);
			}

			public override Task MessageReceived(MessageReceivedContext context)
			{
				Logger.Trace(() => $"{nameof(MessageReceived)}: Token {context.Token}");
				return base.MessageReceived(context);
			}

			public override Task TokenValidated(TokenValidatedContext context)
			{
				Logger.Info(() => $"{nameof(TokenValidated)}: {context.SecurityToken}");
				return base.TokenValidated(context);
			}

			public override Task Challenge(JwtBearerChallengeContext context)
			{
				Logger.Trace(() => $"{nameof(Challenge)}:AuthenticateFailure= {context.AuthenticateFailure},Error= {context.Error},Handled={context.Handled}");
				return base.Challenge(context);
			}
		}
	}

	
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.FastReflection;
using EI.RP.DataServices.StreamServe.Clients;
using EI.RP.DomainServices.Commands.Platform.SendEmail;
using EI.RP.WebApp.Infrastructure.Caching.PreLoad.Processors;
using EI.RP.WebApp.Infrastructure.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;
using ServiceStack;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Azure
{

	class KeyVaultHealthCheck : ResidentialPortalHealthCheck
	{
		private readonly AppSettings _settings;

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


		public KeyVaultHealthCheck(AppSettings settings)
		{
			_settings = settings;
		}

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{
			

			if (_settings.IsAzureEnabled)
			{
				var methods = _settings.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
					.Where(x => x.ReturnType.IsOrHasGenericInterfaceTypeOf(typeof(Task<>))).ToArray();
				if (!methods.Any())
				{
					throw new InvalidOperationException("No methods found");
				}

				var errors=new ConcurrentHashSet<string>();

				try
				{
					var tasks = new List<Task>();
					foreach (var method in methods)
					{
						tasks.Add(Run(  ()=> (Task)method.Invoke(_settings, null),method.Name,errors,cancellationToken));
					}


					await Task.WhenAll(tasks);
				}

				catch (Exception ex)
				{

					return await GetErrorResult(new Exception($"Errors in the key vault expectations:{string.Join(',',errors)}",ex));
				}
				return HealthCheckResult.Healthy($"Health of KeyVault: The check indicates a healthy result.");
			}
			else
			{
				return HealthCheckResult.Healthy($"Health of KeyVault: Azure is not enabled. No health checks were performed");
			}

			
			Task Run(Func<Task> payload, string errorName, ConcurrentHashSet<string> errorsCollection,CancellationToken ct)
			{
				return Task.Run(async () =>
				{
					try
					{
						await payload();
					}
					catch (Exception ex)
					{
						errorsCollection.Add(errorName);
						throw;
					}
				},ct);
			}

		}

		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Unhealthy($"Health of KeyVault: The check indicates a unhealthy result.", ex);
		}
	}
}
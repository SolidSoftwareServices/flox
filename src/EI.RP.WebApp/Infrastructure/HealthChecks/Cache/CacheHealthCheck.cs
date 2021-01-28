using System;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Cache
{

	class CacheHealthCheck : ResidentialPortalHealthCheck
	{
		private readonly ICacheProvider _cacheProvider;

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


		public CacheHealthCheck(ICacheProvider cacheProvider)
		{
			_cacheProvider = cacheProvider;
		}

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{

			HealthCheckResult? result = null;
			try
			{
				await _cacheProvider.CheckHealth(cancellationToken);
				
				result= HealthCheckResult.Healthy($"Health of Cache ({_cacheProvider.Type}): The check indicates a healthy result.");
				
			}
			catch (Exception ex)
			{
				result= await GetErrorResult(ex);
			}
			

			return result.Value;
		}

		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Degraded($"Health of Cache ({_cacheProvider.Type}): The check indicates a unhealthy result.", ex);
		}
	}
}
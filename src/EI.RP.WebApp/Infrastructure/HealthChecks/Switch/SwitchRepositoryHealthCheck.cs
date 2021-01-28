using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Resiliency;
using EI.RP.DataServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Switch
{

	class SwitchRepositoryHealthCheck : ResidentialPortalHealthCheck
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly ISwitchDataRepository _repository;
		

		public SwitchRepositoryHealthCheck(ISwitchDataRepository repository)
		{
			_repository = repository;
		}

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{
			try
			{
				await ResilientOperations.Default.RetryIfNeeded(
					async () => await _repository.GetDiscountsAsync(cancellationToken), cancellationToken,5);
			}
			catch (HttpRequestException e)
			{
				if (e.Message == null || !e.Message.Contains("returned a status code: BadRequest"))
				{
					return await GetErrorResult(e);
				}
			}
		
			catch(Exception ex)
			{
				return await GetErrorResult(ex);
			}
			return HealthCheckResult.Healthy($"Health of Switch API: The check indicates a healthy result.");

		}

		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Unhealthy($"Health of Switch API: The check indicates a unhealthy result.", ex);
		}
	}
}
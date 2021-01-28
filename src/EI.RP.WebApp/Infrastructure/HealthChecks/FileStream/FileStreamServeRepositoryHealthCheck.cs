using System;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Resiliency;
using EI.RP.DataServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.FileStream
{

	class FileStreamServeRepositoryHealthCheck : ResidentialPortalHealthCheck
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IStreamServeRepository _repository;


		public FileStreamServeRepositoryHealthCheck(IStreamServeRepository repository)
		{
			_repository = repository;
		}

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{
			try
			{

				if (!await ResilientOperations.Default.RetryIfNeeded(
					async () => await _repository.IsHealthy(cancellationToken), cancellationToken, 5))
				{
					return HealthCheckResult.Unhealthy(
						$"Health of StreamServe API: The check indicates a unhealthy result.");
				}

			}
			catch (Exception ex)
			{
				return await GetErrorResult(ex);
			}

			return HealthCheckResult.Healthy($"Health of StreamServe API: The check indicates a healthy result.");


		}

		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Unhealthy($"Health of StreamServe API: The check indicates a unhealthy result.",
				ex);
		}
	}
}
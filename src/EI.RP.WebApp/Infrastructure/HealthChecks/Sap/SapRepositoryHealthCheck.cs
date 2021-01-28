using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.Resiliency;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Sap
{

	abstract class SapRepositoryHealthCheck : ResidentialPortalHealthCheck
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IODataRepository _repository;
		

		protected SapRepositoryHealthCheck(IODataRepository repository)
		{
			_repository = repository;
		}

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{
			
			try
			{
				var metadata = await ResilientOperations.Default.RetryIfNeeded(async()=>await _repository.ResolveMetadata(cancellationToken),cancellationToken);
			}
			catch (HttpRequestException e)
			{
				if (e.Message != "Response status code does not indicate success: 401 (Unauthorized).")
				{
					return GetErrorResult(await _repository.GetName(), e);
				}
			}
			catch(Exception ex)
			{
				return GetErrorResult(await _repository.GetName(),ex);
			}
			return HealthCheckResult.Healthy($"Health of {await _repository.GetName()}: The check indicates a healthy result.");

			
		}

		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Unhealthy(
				$"Health of {await _repository.GetName()}: The check indicates a unhealthy result.", ex);
		}

		private HealthCheckResult GetErrorResult(string repoName,Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Unhealthy($"Health of {repoName}: The check indicates a unhealthy result.", ex);
		}
	}
}
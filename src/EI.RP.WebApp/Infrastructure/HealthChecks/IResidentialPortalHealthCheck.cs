using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EI.RP.WebApp.Infrastructure.HealthChecks
{
	public interface IResidentialPortalHealthCheck : IHealthCheck
	{

	}

	abstract class ResidentialPortalHealthCheck : IResidentialPortalHealthCheck
	{
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
		{
			CancellationTokenSource cts = null;
			if (cancellationToken == CancellationToken.None)
			{
				cts=new CancellationTokenSource(TimeSpan.FromSeconds(30));
				cancellationToken = cts.Token;
			}

			var sw = new Stopwatch();
			sw.Start();
			HealthCheckResult result;
			try
			{
				result = await ExecuteCheck(context, cancellationToken);
			}
			catch (Exception ex)
			{
				result = await GetErrorResult(ex);
			}
			finally
			{
				if (cts != null)
				{
					cts.Cancel(false);
					cts.Dispose();
				}
				
			}
			sw.Stop();
			return new HealthCheckResult(result.Status,result.Description,result.Exception,new Dictionary<string, object>
			{
				{"Time Elapsed",sw.Elapsed}
			});

		}

		protected abstract Task<HealthCheckResult> ExecuteCheck(HealthCheckContext context, CancellationToken cancellationToken);
		protected abstract Task<HealthCheckResult> GetErrorResult(Exception ex);
	}
}
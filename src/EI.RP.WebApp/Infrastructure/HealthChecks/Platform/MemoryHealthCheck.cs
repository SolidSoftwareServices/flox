using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Diagnostics.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Platform
{
	class MemoryHealthCheck : ResidentialPortalHealthCheck
	{
	

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


		public MemoryHealthCheck()
		{
		}

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{
			string totalMemMb;
			try
			{
				using (var process = Process.GetCurrentProcess())
				{
					totalMemMb = $"{(process.PrivateMemorySize64 / 1024 / 1024).ToString()}MB";
				}
			}
			catch(Exception ex)
			{
				totalMemMb = $"Unknown. Reason:{ex}";
			}

			HealthCheckResult? result = null;
			try
			{
				var score = EvaluateScore();

				if (score < Score.JustEnough)
				{
					result= HealthCheckResult.Degraded($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}.Total Mem Allocated by app: {totalMemMb} -  Total system memory used:{PercentUsed()}%.");
				}
				else
				{
					result= HealthCheckResult.Healthy($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}: The check indicates a healthy result. Total Mem Allocated by app: {totalMemMb} -  Total system memory used:{PercentUsed()}%.");
				}
				
				
			}
			catch (Exception ex)
			{
				result= await GetErrorResult(ex);
			}
			

			return result.Value;
			Score EvaluateScore()
			{
				var percentUsed = PercentUsed();

				Score score = Score.VeryGood;
				if(percentUsed > 70)
				{
					score = Score.Good;
				}
				if(percentUsed > 80)
				{
					score = Score.JustEnough;
				}
				if(percentUsed > 85)
				{
					score = Score.Poor;
				}
				if(percentUsed > 95)
				{
					score = Score.VeryPoor;
				}

				return score;
			}
			int PercentUsed()
			{
				var values = new MemoryMetricsClient().GetMetrics();
				var percentUsed = 100 * values.Used / values.Total;
				return (int)percentUsed;
			}

		}

		
		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Degraded($"Could not determine the {GetType().Name.Replace("HealthCheck",string.Empty)} health check", ex);
		}
		
	}
}
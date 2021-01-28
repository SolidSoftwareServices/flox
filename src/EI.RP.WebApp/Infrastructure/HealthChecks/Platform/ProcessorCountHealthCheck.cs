using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Platform
{
	class ProcessorCountHealthCheck : ResidentialPortalHealthCheck
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{

			HealthCheckResult? result = null;
			try
			{
				var score = EvaluateScore();

				if (score < Score.JustEnough)
				{
					result= HealthCheckResult.Degraded($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}.Cores: {Environment.ProcessorCount}. Score:{score}");
				}
				else
				{
					result= HealthCheckResult.Healthy($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}: The check indicates a healthy result. Cores: {Environment.ProcessorCount}.Score:{score}.");
				}
				
				
			}
			catch (Exception ex)
			{
				result= await GetErrorResult(ex);
			}
			

			return result.Value;

			Score EvaluateScore()
			{
				switch (Environment.ProcessorCount)
				{
					case 0:
					case 1:
					case 2:
						return Score.VeryPoor;
					case 3:
					case 4:
						return Score.Poor;
					case 5:
					case 6:
						return Score.JustEnough;
					case 7:
					case 8:
						return Score.Good;
					default:
						return Score.VeryGood;
				}
			}
		}
		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Degraded($"Could not determine the {GetType().Name.Replace("HealthCheck",string.Empty)} health check", ex);
		}
		
	}
}
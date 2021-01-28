using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Platform
{
	class BitsHealthCheck : ResidentialPortalHealthCheck
	{
	

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


		public BitsHealthCheck()
		{
		}

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
					result= HealthCheckResult.Degraded($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}.Is64BitProcess: {Environment.Is64BitProcess} and Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}.");
				}
				else
				{
					result= HealthCheckResult.Healthy($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}: The check indicates a healthy result. Is64BitProcess: {Environment.Is64BitProcess} and Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}.");
				}
				
				
			}
			catch (Exception ex)
			{
				result= await GetErrorResult(ex);
			}
			

			return result.Value;
			Score EvaluateScore()
			{
				if (Environment.Is64BitProcess && Environment.Is64BitOperatingSystem) return Score.VeryGood;
				
				return Score.Poor;
			}
			
		}
		protected override async Task<HealthCheckResult> GetErrorResult(Exception ex)
		{
			Logger.Warn(() => ex.ToString());
			return HealthCheckResult.Degraded($"Could not determine the {GetType().Name.Replace("HealthCheck",string.Empty)} health check", ex);
		}
		
	}
}
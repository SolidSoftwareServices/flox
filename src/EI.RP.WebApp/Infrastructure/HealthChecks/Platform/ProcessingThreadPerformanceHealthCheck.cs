using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NLog;

namespace EI.RP.WebApp.Infrastructure.HealthChecks.Platform
{
	class ProcessingThreadPerformanceHealthCheck : ResidentialPortalHealthCheck
	{
	

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


		public ProcessingThreadPerformanceHealthCheck()
		{
		}

		protected override async Task<HealthCheckResult> ExecuteCheck(
			HealthCheckContext context,
			CancellationToken cancellationToken)
		{

			HealthCheckResult? result = null;
			try
			{
				var score = EvaluateScore(out var timeTaken);

				if (score < Score.JustEnough)
				{
					result= HealthCheckResult.Degraded($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}.Performance result:{score} .Time taken: {timeTaken}");
				}
				else
				{
					result= HealthCheckResult.Healthy($"Health of {GetType().Name.Replace("HealthCheck",string.Empty)}: The check indicates a healthy result. Performance result:{score}. Time taken:{timeTaken}");
				}
				
				
			}
			catch (Exception ex)
			{
				result= await GetErrorResult(ex);
			}
			

			return result.Value;

			Score EvaluateScore(out TimeSpan totalTime)
			{
				var sw=new Stopwatch();
				sw.Start();
				Payload(100000);
				sw.Stop();
				totalTime = sw.Elapsed;
				if (sw.Elapsed < TimeSpan.FromSeconds(0.2)) return Score.VeryGood;
				if (sw.Elapsed < TimeSpan.FromSeconds(0.75)) return Score.Good;
				if (sw.Elapsed < TimeSpan.FromSeconds(1.5)) return Score.JustEnough;
				if (sw.Elapsed < TimeSpan.FromSeconds(2)) return Score.Poor;
				return Score.VeryPoor;
				long Payload(int n)
				{
					var count=0;
					long a = 2;
					while(count<n)
					{
						long b = 2;
						var prime = 1;
						while(b * b <= a)
						{
							if(a % b == 0)
							{
								prime = 0;
								break;
							}
							b++;
						}
						if(prime > 0)
						{
							count++;
						}
						a++;
					}
					return (--a);
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
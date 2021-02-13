using System.Threading.Tasks;
using Castle.DynamicProxy;
using NLog;
using S3.CoreServices.System;

namespace S3.CoreServices.Profiling
{

	class ProfilerInterceptor : IInterceptor
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IProfiler _profiler;

		public ProfilerInterceptor(IProfiler profiler)
		{
			_profiler = profiler;
		}

		public void Intercept(IInvocation invocation)
		{
			var stepId = $"{invocation.TargetType.Name}.{invocation.Method.Name}";
			Logger.Trace(() => $"{stepId} - START");
			using (_profiler.RecordStep(stepId))
			{
				invocation.Proceed();
				var type = invocation.ReturnValue?.GetType();
				if (type != null
				    && (type == typeof(Task) || type.ImplementsOpenGeneric(typeof(Task<>))))
				{
					invocation.ReturnValue = InterceptAsync((dynamic) invocation.ReturnValue);
				}
			}

			Logger.Trace(() => $"{stepId} - END");
			
		}

		private  async Task InterceptAsync(Task task)
		{
			await task.ConfigureAwait(false);
		}

		private async Task<TResult> InterceptAsync<TResult>(Task<TResult> task)
		{
			var result = await task.ConfigureAwait(false);
			return result;
		}

		
	}
}
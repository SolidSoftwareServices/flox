using System;
using Autofac;
using S3.CoreServices.Platform;
using S3.CoreServices.Profiling;

namespace S3.Mvc.Core.Profiler.IoC
{

	[Obsolete("Move to general purpose")]
	public class ProfilerModule: Module 
	{
		
		protected override void Load(ContainerBuilder builder)
		{
			

			builder.Register<IProfiler>(ctx =>
				{
					var settings = ctx.Resolve<IPlatformSettings>();
					
					if (settings.IsProfilerEnabled)
					{
						return new MiniProfilerDecorator();
					}

					return new NoProfiler();
				})
				.As<IProfiler>();

			
		
		}

	}
}
using System;
using Autofac;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.Profiling;
using Microsoft.AspNetCore.Hosting;

namespace Ei.Rp.Mvc.Core.Profiler.IoC
{

	[Obsolete("Move to general purpose")]
	public class ProfilerModule: BaseModule 
	{
		
		protected override void Load(ContainerBuilder builder)
		{
			

			builder.Register<IProfiler>(ctx =>
				{
					var e = ctx.Resolve<IHostingEnvironment>();
					if (!e.IsOneOfTheReleaseEnvironments())
					{
						return new MiniProfilerDecorator();
					}

					return new NoProfiler();
				})
				.As<IProfiler>();

			
		
		}

	}
}
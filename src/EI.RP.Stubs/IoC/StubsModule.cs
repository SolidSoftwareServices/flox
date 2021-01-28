using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.Stubs.CoreServices.Http.Session;

namespace EI.RP.Stubs.IoC
{
	public class StubsModule:BaseModule
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterType<FakeSessionProviderStrategy>().AsImplementedInterfaces().WithInterfaceProfiling().SingleInstance();
			builder.Register(c=>SessionDataHolder.Instance).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterType<NoCacheProvider>().AsImplementedInterfaces().WithInterfaceProfiling();
		}
	}
}

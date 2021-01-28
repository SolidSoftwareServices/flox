using System;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EI.RP.Stubs.IoC
{
	public class IoCContainerBuilder
	{


		private readonly IModule _registrationsModuleInstance;

		public static IContainer From<TRegistrationsModule>()
			where TRegistrationsModule : IModule, new()
		{
			return new IoCContainerBuilder(new TRegistrationsModule()).CreateContainer();
		}
		public static IContainer From(IModule module)
		{
			return new IoCContainerBuilder(module).CreateContainer();
		}
		private IoCContainerBuilder(IModule registrationsModuleInstance)
		{
			if (registrationsModuleInstance == null) throw new ArgumentNullException(nameof(registrationsModuleInstance));
			_registrationsModuleInstance = registrationsModuleInstance;
		}
		public IContainer CreateContainer()
		{
			var builder = new ContainerBuilder();

			// ReSharper disable once CollectionNeverUpdated.Local
			var serviceCollection = new ServiceCollection();


			var containerBuilder = new ContainerBuilder();

			containerBuilder.Populate(serviceCollection);
			builder.RegisterModule(_registrationsModuleInstance);
			
			builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().SingleInstance();
			var container = builder.Build();

		

			return container;
		}


	}
}
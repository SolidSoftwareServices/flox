using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.Platform;
using EI.RP.CoreServices.Profiling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.ObjectPool;
using HostingEnvironmentExtensions = EI.RP.CoreServices.DeliveryPipeline.Environments.HostingEnvironmentExtensions;

namespace EI.RP.CoreServices.Infrastructure
{
	public class CoreModule : BaseModule
	{
		private readonly IPlatformSettings _platformSettings;
		private bool _addEncryption;

		private CoreModule(IPlatformSettings platformSettings)
		{
			_platformSettings = platformSettings;
		}

		public static CoreModule Configure(IPlatformSettings platformSettings = null)
		{
			return new CoreModule(platformSettings);
		}

		public CoreModule WithEncryptionFeature(bool enabled = true)
		{
			_addEncryption = enabled;
			return this;
		}

		protected override void Load(ContainerBuilder builder)
		{
			if (_platformSettings != null && _platformSettings.ProfileInDetail)
			{
				AutofacExtensions.DetailedProfilingEnabled = true;
				builder.RegisterType<ProfilerInterceptor>();
			}

			builder.RegisterType<ClientInfoResolver>().AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterType<AspNetUserSessionProviderStrategy>().As<IRealUserSessionProvider>()
				.WithInterfaceProfiling();
			builder.RegisterType<UserSessionProviderDecorator>().As<IUserSessionProvider>().InstancePerLifetimeScope()
				.WithInterfaceProfiling();

			builder.Register(c =>
			{
				var hostingEnvironment = c.Resolve<IHostingEnvironment>();
				return hostingEnvironment.IsProduction() ||
				       HostingEnvironmentExtensions.IsPreProduction(hostingEnvironment)
					? new NoConfigurableTestingItems()
					: (IConfigurableTestingItems) new ConfigurableTestingItems(hostingEnvironment);
			}).As<IConfigurableTestingItems>().SingleInstance().WithInterfaceProfiling();

			RegisterEncryption(builder);

			builder.RegisterType<DefaultHttpClientBuilder>().AsImplementedInterfaces().WithInterfaceProfiling();

		}

		private void RegisterEncryption(ContainerBuilder builder)
		{
			if (_addEncryption)
			{
				builder.RegisterType<EncryptTransformFactory>().As<IEncryptTransformFactory>().AsSelf()
					.WithClassProfiling();
				builder.RegisterType<DecryptTransformFactory>().As<IDecryptTransformFactory>().AsSelf()
					.WithClassProfiling();
				var encryptor = "Encryptor";
				var decryptor = "Decryptor";
				builder.Register(c => new DefaultObjectPool<Task<ICryptoTransform>>(c.Resolve<EncryptTransformFactory>()))
					.Named<ObjectPool<Task<ICryptoTransform>>>(encryptor).SingleInstance();

				builder.Register(c => new DefaultObjectPool<Task<ICryptoTransform>>(c.Resolve<DecryptTransformFactory>()))
					.Named<ObjectPool<Task<ICryptoTransform>>>(decryptor).SingleInstance();
				builder.Register(c => new DefaultEncryptionService(
							c.ResolveNamed<ObjectPool<Task<ICryptoTransform>>>(encryptor),
							c.ResolveNamed<ObjectPool<Task<ICryptoTransform>>>(decryptor)
						)
					)
					.AsImplementedInterfaces()
					.WithInterfaceProfiling();
			}
		}
	}
}

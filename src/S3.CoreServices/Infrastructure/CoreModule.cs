using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.ObjectPool;
using S3.CoreServices.Encryption;
using S3.CoreServices.IoC.Autofac;
using S3.CoreServices.Platform;
using S3.CoreServices.Profiling;

namespace S3.CoreServices.Infrastructure
{
	public class CoreModule : Module
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
			RegisterEncryption(builder);
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

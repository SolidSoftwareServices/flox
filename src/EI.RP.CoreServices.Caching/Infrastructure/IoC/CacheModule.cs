using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Caching.Redis;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.System.Async;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace EI.RP.CoreServices.Caching.Infrastructure.IoC
{
	public class CacheModule : BaseModule
	{
		public const string RedisInstanceName = "ResidentialPortal";
		private readonly CacheProviderType _cacheProviderType;

		public CacheModule():this(CacheProviderType.NoCache)
		{
		}
		public CacheModule(ICacheSettings cacheSettings):this(cacheSettings.CacheProviderType)
		{
		
		}

		private CacheModule(CacheProviderType cacheProviderType)
		{
			_cacheProviderType = cacheProviderType;
		}

		protected override void Load(ContainerBuilder builder)
		{
			RegisterCache(builder);
		}

		private void RegisterCache(ContainerBuilder builder)
		{
			builder.Register(c => new NoCacheProvider())
				.As<IInternalCacheProvider>()
				.SingleInstance()
				.WithInterfaceProfiling();

			builder.Register(c => new InMemoryCacheProvider(c.Resolve<ICacheSettings>(),
					new AsyncLazy<MemoryDistributedCache>(() => new MemoryDistributedCache(
						new OptionsWrapper<MemoryDistributedCacheOptions>(
							new MemoryDistributedCacheOptions {ExpirationScanFrequency = TimeSpan.FromSeconds(30)}))
					)))
				.As<IInternalCacheProvider>()
				.AsSelf()
				.SingleInstance()
				.WithInterfaceProfiling();

			builder.Register(c =>
				{
					//following 2 lines is a bootch job
					var env=c.Resolve<IHostingEnvironment>();
					//set the keys environment id
					CacheKeyScope.StageName = ((AppEnvironment)env.EnvironmentName).ResolveStageName();

					var settings = c.Resolve<ICacheSettings>();
					if (!settings.IsCacheEnabled)
						return c.Resolve<IEnumerable<IInternalCacheProvider>>()
							.Single(x => x.Type == CacheProviderType.NoCache);
					var providerType = settings.CacheProviderType;
					return c.Resolve<IEnumerable<IInternalCacheProvider>>()
						.Single(x => x.Type == providerType);
				}).As<ICacheProvider>()
				.SingleInstance()
				.WithInterfaceProfiling();


			RegisterRedisCacheType(builder);
		}

		private void RegisterRedisCacheType(ContainerBuilder builder)
		{
			if (_cacheProviderType == CacheProviderType.Redis)
			{
				builder.RegisterType<RedisServerProvider>().AsImplementedInterfaces().SingleInstance();

				builder.Register(c =>
				{
					var cacheSettings = c.Resolve<ICacheSettings>();
					return new AsyncLazy<RedisCache>(async () =>
					{
						var connStr = await cacheSettings.RedisConnectionString();
						var options = new OptionsWrapper<RedisCacheOptions>(new RedisCacheOptions
						{
							Configuration = connStr,
							InstanceName = RedisInstanceName
						});
						return new RedisCache(options);
					});
				}).AsSelf().SingleInstance();

				builder.Register(c => new RedisCacheProvider(
						c.Resolve<InMemoryCacheProvider>(),
						c.Resolve<IRedisCacheFacade>(),
						c.Resolve<AsyncLazy<RedisCacheOptions>>(),
						c.Resolve<IEncryptionService>()))
					.As<IInternalCacheProvider>()
					.SingleInstance();

				builder.RegisterType<RedisRemoteCacheFacade>()
					.AsImplementedInterfaces()
					.WithInterfaceProfiling();


				builder.Register(c =>
					{
						var cacheSettings = c.Resolve<ICacheSettings>();
						return new AsyncLazy<RedisCacheOptions>(async () => new RedisCacheOptions
						{
							Configuration = await cacheSettings.RedisConnectionString(),
							InstanceName = RedisInstanceName
						});
					})
					.AsSelf()
					.SingleInstance();
			}
		}
	}
}
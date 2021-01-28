using System.Threading.Tasks;
using S3.AspnetCore.SessionServices.TicketStores;
using S3.CoreServices.Caching.Infrastructure.IoC;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace S3.AspnetCore.SessionServices.Infrastructure
{
	public static class SessionConfigurationExtensions
	{
		

		public static CookieAuthenticationOptions ConfigureSessionStorage(this CookieAuthenticationOptions opts,IWebSessionSettings settings)
		{
			if (settings.SessionStorage == SessionStorageType.InMemory)
			{
				opts.SessionStore=new InMemoryTicketStore(settings);
			}
			else if (settings.SessionStorage == SessionStorageType.Redis)
			{
				var connectionString = settings.RedisConnectionString().GetAwaiter().GetResult();
				opts.SessionStore=new RedisTicketStore(new RedisCacheOptions
				{
					Configuration =connectionString,
					InstanceName = CacheModule.RedisInstanceName
				},settings);
			}
			return opts;
		}

		public static  IServiceCollection AddEiSession(this IServiceCollection services,
			IWebSessionSettings settings)
		{
			if (settings.SessionStorage == SessionStorageType.Redis)
			{
				services
					.AddDataProtection()
					.PersistKeysToStackExchangeRedis(
						() => ConnectionMultiplexer.Connect( settings.RedisConnectionString().GetAwaiter().GetResult()).GetDatabase(), 
						"ProtectedSessionKeys");
				services.AddStackExchangeRedisCache(option =>
				{
					option.Configuration = settings.RedisConnectionString().GetAwaiter().GetResult();
					option.InstanceName = CacheModule.RedisInstanceName;
				});

			}

			services.AddSession(opts =>
			{
				opts.IdleTimeout = settings.SessionTimeout;
				opts.Cookie.HttpOnly = true;
				opts.Cookie.IsEssential = true;
				opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;

			});
			return services;
		}
	}
}
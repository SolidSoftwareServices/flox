using System;
using System.Threading.Tasks;

namespace S3.AspnetCore.SessionServices.Infrastructure
{
	public interface IWebSessionSettings
	{	
		SessionStorageType SessionStorage{ get;}
		TimeSpan SessionTimeout { get; }

		Task<string> RedisConnectionString();
	}
}

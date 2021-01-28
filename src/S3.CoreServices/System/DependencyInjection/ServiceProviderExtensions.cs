using System;

namespace S3.CoreServices.System.DependencyInjection
{
	public static class ServiceProviderExtensions
	{
		public static TService Resolve<TService>(this IServiceProvider src, bool failIfNotRegistered = true)
		{
			var service = src.GetService(typeof(TService));

			if (service == null && failIfNotRegistered)
				throw new InvalidOperationException($"Could not locate service: {typeof(TService).FullName}");

			return (TService) service;
		}

	}
}
using System;
using System.Collections.Concurrent;
using System.Reflection;
using Simple.OData.Client;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Batches
{
	public static class BoundClientExtensions
	{

		private static readonly ConcurrentDictionary<Type, FieldInfo> Cache=new ConcurrentDictionary<Type, FieldInfo>();
		public static IBoundClient<TDto> SetBatchClient<TDto>(this IBoundClient<TDto> boundClient, IODataClient client)
			where TDto : class
		{
			Cache.GetOrAdd(typeof(TDto), (k)=>
			{
				var type = typeof(FluentClientBase<TDto, IBoundClient<TDto>>);
				return type.GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic);
			}).SetValue(boundClient, client);
			return boundClient;
		}
	}
}
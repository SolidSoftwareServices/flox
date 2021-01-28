using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace EI.RP.CoreServices.Caching.InMemory
{
	internal delegate Task  CacheKeyHandler(object sender, string[] keys);
	internal delegate Task  CacheKeyValueAddedHandler(object sender, string key,CachedItem value,DistributedCacheEntryOptions options);
}
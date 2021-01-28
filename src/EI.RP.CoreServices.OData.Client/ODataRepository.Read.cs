using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;
using EI.RP.CoreServices.OData.Client.Infrastructure.Batches;
using EI.RP.CoreServices.OData.Client.Infrastructure.SimpleDataClient;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using NLog;
using Simple.OData.Client;

namespace EI.RP.CoreServices.OData.Client
{
	public abstract partial class ODataRepository
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		
		
		private static readonly ConcurrentDictionary<Type,string> CollectionNames=new ConcurrentDictionary<Type, string>();

		public IFluentODataModelQuery<TDto> NewQuery<TDto>() where TDto : ODataDtoModel, new()
		{

			var collectionName = CollectionNames.GetOrAdd(typeof(TDto),(t)=>ObjectBuilder.Default.Build<TDto>().CollectionName());
			return new SimpleODataBoundClientDecorator<TDto>(this,Client.For<TDto>(collectionName));
		}


		public  async Task<IEnumerable<TDto>> GetMany<TDto>(IFluentODataModelQuery<TDto> query,IBatchAggregator batchAggregator) where TDto : ODataDtoModel,new()
		{
			var t1= ThrowIfTypeIsNotDeclaredInContainer<TDto>();
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(GetMany)} of {typeof(TDto).Name}"))
			{
				var internalQuery = (SimpleODataBoundClientDecorator<TDto>)query;
				IEnumerable<TDto> result=null;

				
				var func = (Func<IODataClient, Task>) (async c =>
				{
					result = (await internalQuery.BoundClient.SetBatchClient(c).FindEntriesAsync());
					
				});
				Task t2;
				if (batchAggregator != null)
				{
					t2= ((IBatchAggregatorEnlister)batchAggregator).Enlist(func);
					
				}
				else
				{
					t2= func(Client);
				}

				await Task.WhenAll(t1, t2);
				result = result != null
					? EnsureFilterApplied(result, internalQuery).Select(TrackDto).ToArray()
					: await GetMany(query);
				return result;
			}
		}

		public async Task<IEnumerable<TDto>> GetMany<TDto>(IFluentODataModelQuery<TDto> query, bool autobatch = true) where TDto : ODataDtoModel,new()
		{
			
			return autobatch
				?await ExecuteAutoBatch(batchAggregator=>GetMany(query, batchAggregator))
				: await GetMany(query,null);
		}

	


		public async Task<TDto> GetOne<TDto>(IFluentODataModelQuery<TDto> query, IBatchAggregator batchAggregator) where TDto : ODataDtoModel, new()
		{
			var t1=ThrowIfTypeIsNotDeclaredInContainer<TDto>();
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(GetOne)} of {typeof(TDto).Name}"))
			{
				var internalQuery = (SimpleODataBoundClientDecorator<TDto>)query;
				TDto result = null;

				var func = (Func<IODataClient, Task>)(async c => result = await internalQuery.BoundClient.SetBatchClient(c).FindEntryAsync());
				Task t2;
				if (batchAggregator != null)
				{
					t2= ((IBatchAggregatorEnlister)batchAggregator).Enlist(func);
				}
				else
				{
					t2= func(Client);
				}

				await Task.WhenAll(t1, t2);
				return TrackDto(result);
			}
		}
		public async Task<TDto> GetOne<TDto>(IFluentODataModelQuery<TDto> query, bool autobatch = true) where TDto : ODataDtoModel,new()
		{
			
			return autobatch
				? await ExecuteAutoBatch(batchAggregator => this.GetOne(query, batchAggregator))
				: await GetOne(query, null);

		}
		
		private TDto TrackDto<TDto>(TDto result) where TDto : ODataDtoModel
		{
			_changesTracker.TrackInstance(result);
			return result;
		}
		private IEnumerable<TDto> EnsureFilterApplied<TDto>(IEnumerable<TDto> original, SimpleODataBoundClientDecorator<TDto> query) where TDto : ODataDtoModel,new()
		{
			//since not all filters are always supported this makes sure they are applied
			var result = original as TDto[] ?? original.ToArray();
			var originalCount = result.Length;
			foreach (var expression in query.FilterExpressionsApplied.Where(x=>x.ensureIsApplied).Select(x=>x.expression))
			{
				result = result.Where(expression.Compile()).ToArray();
			}

			var actualCount = result.Length;
			if (actualCount != originalCount)
			{
				Logger.Warn(() => $"CHANGE TO IMPROVE PERFORMANCE: The api is not applying one or more filters requested in the following stack: {Environment.StackTrace}");
			}
			return result;
		}



		
	}
}
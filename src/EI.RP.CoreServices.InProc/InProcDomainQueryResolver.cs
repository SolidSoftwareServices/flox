using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.DependencyInjection;
using NLog;

namespace EI.RP.CoreServices.InProc
{
	class InProcDomainQueryResolver : IDomainQueryResolver
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IServiceProvider _container;
		private readonly IEventApiEventPublisher _eventsApiEventPublisher;
		private readonly IProfiler _profiler;
		private readonly ICacheProvider _cacheProvider;
		private readonly IUserSessionProvider _userSessionProvider;

		public InProcDomainQueryResolver(IServiceProvider container, IEventApiEventPublisher eventsApiEventPublisher,
			IProfiler profiler, ICacheProvider cacheProvider, IUserSessionProvider userSessionProvider)
		{
			_container = container;
			_eventsApiEventPublisher = eventsApiEventPublisher;
			_profiler = profiler;
			_cacheProvider = cacheProvider;
			_userSessionProvider = userSessionProvider;
		}

		public async Task<IEnumerable<TQueryResult>> FetchAsync<TQueryModel, TQueryResult>(TQueryModel query,
			bool byPassPipeline = false)
			where TQueryResult : class, IQueryResult where TQueryModel : IQueryModel
		{
			const string profileCategoryId = "DomainQuery";
#if FrameworkDevelopment
			using (_profiler.Profile(profileCategoryId, $"{typeof(TQueryModel).Name} - {query.ToString()}"))
#else
			using (_profiler.Profile(profileCategoryId, typeof(TQueryModel).Name))
#endif

			{
				IEnumerable<TQueryResult> result = new TQueryResult[0];
				var eventBuilder = _container.Resolve<IQueryEventBuilder<TQueryModel>>(failIfNotRegistered:false);

				Logger.Debug(() =>
					eventBuilder == null
						? $"No event builder defined for {typeof(TQueryModel).FullName}"
						: string.Empty);
				try
				{

					result = await _ExecuteQueryAsync<TQueryModel, TQueryResult>(query);
					if (!byPassPipeline)
					{
						using (_profiler.Profile(profileCategoryId, $"Publish Event of {typeof(TQueryModel).Name}"))
						{
							//if it was implemented for a query
							await _PublishSuccessfulOperationEvent(eventBuilder, query);
						}
					}
				}
				catch (Exception ex)
				{
					using (_profiler.Profile(profileCategoryId, $"Handle Domain Error of {typeof(TQueryModel).Name}"))
						await _HandleDomainError(query, ex, eventBuilder);
				}

				return result;
			}
		}

		private async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryModel, TQueryResult>(TQueryModel query)
			where TQueryResult : class, IQueryResult where TQueryModel : IQueryModel
		{

			var result=new TQueryResult[0];
			switch (query.CacheResults)
			{
				case QueryCacheResultsType.NoCache:
					result= (await ProviderFunc()).ToArray();
					break;
				
				case QueryCacheResultsType.UserSpecific:
					var keyContext = _userSessionProvider.ResolveUserBasedCacheKeyContext();
					result =(await _cacheProvider.GetOrAddAsync(query, ProviderFunc,
						keyContext)).ToArray();
					await CacheSingleItemsInCollection(result,keyContext,null);
					break;
				case QueryCacheResultsType.AllUsersShared:
					var maxDurationFromNow = TimeSpan.FromMinutes(10);
					result =(await _cacheProvider.GetOrAddAsync(query, ProviderFunc,maxDurationFromNow:maxDurationFromNow)).ToArray();
					await CacheSingleItemsInCollection(result,null,maxDurationFromNow);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;

			Task<IEnumerable<TQueryResult>> ProviderFunc()
			{
				return ((IQueryHandler<TQueryModel>)_container.GetService(typeof(IQueryHandler<TQueryModel>))).ExecuteQueryAsync<TQueryResult>(query);
			}

			async Task CacheSingleItemsInCollection(IEnumerable<TQueryResult> queryResults, string keyContext,
				TimeSpan? maxDurationFromNow)
			{
				var cacheIndividualRequestsFromCollectionHelper =
					_container.Resolve<ICacheSingleItemsInCollection<TQueryModel, TQueryResult>>(
						failIfNotRegistered: false);
				if (cacheIndividualRequestsFromCollectionHelper != null)
				{
					var items = cacheIndividualRequestsFromCollectionHelper.Resolve(query, queryResults);
					foreach (var item in items)
					{
						await _cacheProvider.GetOrAddAsync(item.Query, () => Task.FromResult(item.Value.ToOneItemArray().AsEnumerable()), keyContext,maxDurationFromNow);
					}
				}
			}
		}

		private async Task _HandleDomainError<TQueryModel>(TQueryModel query, Exception ex,
			IQueryEventBuilder<TQueryModel> eventBuilder) where TQueryModel : IQueryModel
		{
			//every domain exception must be aggregate exception
			var exceptionToRethrow =
				!(ex is AggregateException) ? new AggregateException(ex) : (AggregateException) ex;

			var innerExceptions = exceptionToRethrow.InnerExceptions.ToList();
			var changed = false;
			for (var i = 0; i < innerExceptions.Count; i++)
			{
				var e = innerExceptions[i];
				if (!(e is DomainException))
				{
					innerExceptions[i] = new DomainException(DomainError.Undefined, e);
					changed = true;
				}
			}

			if (changed)
			{
				exceptionToRethrow = new AggregateException(innerExceptions);
			}

			try
			{
				await _PublishOperationFailedEvent(eventBuilder, query, exceptionToRethrow);
			}
			catch (Exception e)
			{
				exceptionToRethrow = new AggregateException(exceptionToRethrow.InnerExceptions.Union(new[] {e}));
			}

			throw exceptionToRethrow;

		}

		private async Task _PublishOperationFailedEvent<TQueryModel>(IQueryEventBuilder<TQueryModel> eventBuilder,
			TQueryModel query, AggregateException exceptions) where TQueryModel : IQueryModel
		{
			if (eventBuilder != null)
			{
				var eventToPublish = await eventBuilder.BuildEventOnError(query, exceptions);

				if (eventToPublish != null)
				{
					await _eventsApiEventPublisher.Publish(eventToPublish);
					Logger.Debug($"Published event {eventToPublish}");
				}
				else
				{
					Logger.Debug($"Event builder of {typeof(TQueryModel).FullName} does not build events on error");
				}
			}
		}

		private async Task _PublishSuccessfulOperationEvent<TQueryModel>(IQueryEventBuilder<TQueryModel> eventBuilder,
			TQueryModel query) where TQueryModel : IQueryModel
		{
			if (eventBuilder != null)
			{
				var eventToPublish = await eventBuilder.BuildEventOnSuccess(query);
				if (eventToPublish != null)
				{
					await _eventsApiEventPublisher.Publish(eventToPublish);
					Logger.Debug($"Published event {eventToPublish}");
				}
				else
				{
					Logger.Debug($"Event builder of {typeof(TQueryModel).FullName} does not build events on success");
				}
			}
		}

	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.Batching;
using EI.RP.CoreServices.OData.Client.Infrastructure.Extensions;
using EI.RP.CoreServices.OData.Client.Infrastructure.Validation;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System.FastReflection;
using Microsoft.Extensions.Options;
using Simple.OData.Client;

namespace EI.RP.CoreServices.OData.Client
{
	public abstract partial class ODataRepository
	{
		#region Add

        public async Task<TDto> AddThenGet<TDto>(TDto newEntity, bool autobatch = true) where TDto : ODataDtoModel
		{
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(AddThenGet)} of {typeof(TDto).Name}"))
			{
				return await InsertEntryAsync(newEntity, true,autobatch);
			}
		}

		public async Task Add<TDto>(TDto newEntity, bool autobatch = true) where TDto : ODataDtoModel
		{
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(Add)} of {typeof(TDto).Name}"))
			{
				await InsertEntryAsync(newEntity, false,autobatch);
			}
		}

		protected virtual async Task<TDto> InsertEntryAsync<TDto>(TDto newEntity, bool withResult, bool autobatch ) where TDto : ODataDtoModel
		{
			var task=autobatch
				? ExecuteAutoBatch(batchAggregator => InsertEntryAsync(newEntity, withResult, batchAggregator))
				: InsertEntryAsync(newEntity, withResult,null);
			return await task;
		}

		protected virtual async Task<TDto> InsertEntryAsync<TDto>(TDto newEntity, bool withResult, IBatchAggregator batchAggregator) where TDto : ODataDtoModel
		{
			await ThrowIfTypeIsNotDeclaredInContainer(newEntity);
			_modelValidator.Validate(newEntity,ProxyModelOperation.Add);

			TDto updateable=null;

			var func = (Func<IODataClient, Task>)(async c =>
			{
				updateable = await Client
					.For<TDto>(newEntity.CollectionName())
					.Set(newEntity.ToDictionary())
					.InsertEntryAsync(withResult);
			});
			if(batchAggregator!=null)
			{
				try
				{
					await ((IBatchAggregatorEnlister) batchAggregator).Enlist(func);
				}
				catch (BatchWasClosedException)
				{
					await func(Client);
				}
			}
			else
			{
				await func(Client);
			}

			if (withResult && updateable != null)
			{
				_changesTracker.TrackInstance(updateable);
			}
			else
			{
				_changesTracker.TrackInstance(newEntity);
			}

			return updateable;
		}
		#endregion

		#region Update

		public async Task<TDto> UpdateThenGet<TDto>(TDto changedEntity, bool autobatch = true) where TDto : ODataDtoModel
		{
			if (!autobatch) throw new NotImplementedException();
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(UpdateThenGet)} of {typeof(TDto).Name}"))
			{
				return await UpdateEntryAsync(changedEntity, true);
			}
		}

		public async Task Update<TDto>(TDto changedEntity, bool autobatch = true) where TDto : ODataDtoModel
		{
			if (!autobatch) throw new NotImplementedException();
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(Update)} of {typeof(TDto).Name}"))
			{
				await UpdateEntryAsync(changedEntity, false);
			}
		}
		private async Task<TDto> UpdateEntryAsync<TDto>(TDto changedEntity, bool retrieveRemoteChangedObject) where TDto : ODataDtoModel
		{
			return await ExecuteAutoBatch(batchAggregator => UpdateEntryAsync(changedEntity, retrieveRemoteChangedObject, batchAggregator));
		}
		private async Task<TDto> UpdateEntryAsync<TDto>(TDto changedEntity, bool retrieveRemoteChangedObject, IBatchAggregator batchAggregator) where TDto :  ODataDtoModel
		{
			await ThrowIfTypeIsNotDeclaredInContainer(changedEntity);
			_modelValidator.Validate(changedEntity,ProxyModelOperation.Update);
			var changes = _changesTracker.GetChanges(changedEntity);
			if (!changes.Any())
			{
				return changedEntity;
			}

			var boundClient = FetchExistingEntity();

			boundClient = SetValue(boundClient, changes);

            TDto updateable=null;
			await ((IBatchAggregatorEnlister) batchAggregator)
				.Enlist((Func<IODataClient, Task>) (async c =>
				{
					updateable = await boundClient
						.UpdateEntryAsync(retrieveRemoteChangedObject);
				}));

			UpdateChangesTracker();

			return updateable;

			IBoundClient<TDto> SetValue(IBoundClient<TDto> bc, IDictionary<string, object> changesDictionary)
			{
				switch (changedEntity.UpdateMode())
				{

					case ODataUpdateType.FullModel:
						return bc.Set(changedEntity);
					case ODataUpdateType.OnlyChangedValues:
						return bc.Set(changesDictionary);
					default:
						throw new ArgumentOutOfRangeException($"{nameof(ODataUpdateType)}not supported");
				}
			}

			IBoundClient<TDto> FetchExistingEntity()
			{
				var client = Client
					.For<TDto>(changedEntity.CollectionName());
				IBoundClient<TDto> result;

				var uniqueId = changedEntity.UniqueId();

				if (uniqueId.Any())
				{
					result = client
						.Key(uniqueId);
				}
				else
				{
					//the behaviour of this has changed in the new versions
					result = client
						.Key(changedEntity);
				}

				return result;
			}

			void UpdateChangesTracker()
			{
				_changesTracker.Detach(changedEntity);

                var trackable= retrieveRemoteChangedObject && updateable != null ? updateable : changedEntity;
				
                _changesTracker.TrackInstance(trackable);

            }
        }

        #endregion


        #region Delete

        public async Task Delete<TDto>(TDto existingEntity, bool autobatch = true) where TDto : ODataDtoModel
        {
			if (!autobatch) throw new NotImplementedException();
			await DeleteEntryAsync(existingEntity);
            
        }

		private async Task DeleteEntryAsync<TDto>(TDto existingEntity, bool autobatch = true) where TDto : ODataDtoModel
		{
			 await ExecuteAutoBatch(batchAggregator => DeleteEntryAsync(existingEntity, batchAggregator));
		}

		private async Task DeleteEntryAsync<TDto>(TDto existingEntity, IBatchAggregator batchAggregator) where TDto : ODataDtoModel
		{
			await ThrowIfTypeIsNotDeclaredInContainer(existingEntity);
			using (Profiler.Profile(ProfileCategoryId, $"{nameof(Delete)} of {typeof(TDto).Name}"))
			{
				await ((IBatchAggregatorEnlister) batchAggregator)
					.Enlist((Func<IODataClient, Task>) (async c =>
						await Client
							.For<TDto>(existingEntity.CollectionName())
							.Key(existingEntity.ToDictionary())
							.DeleteEntryAsync()));

				_changesTracker.Detach(existingEntity);
			}


		}

		#endregion
    }
}
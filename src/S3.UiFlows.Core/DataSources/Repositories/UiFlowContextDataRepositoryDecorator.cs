using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using S3.UiFlows.Core.DataSources.Repositories.Adapters;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.DataSources.Repositories
{
	internal class UiFlowContextDataRepositoryDecorator : IUiFlowContextRepository
	{
		private readonly IRepositoryAdapter _decoratedRepository;
		private readonly IProfiler _profiler;
		private readonly ConcurrentHashSet<string> _removed = new ConcurrentHashSet<string>();

		internal ConcurrentDictionary<string, Task<UiFlowContextData>> Store { get; } = new ConcurrentDictionary<string, Task<UiFlowContextData>>();

		public UiFlowContextDataRepositoryDecorator(IRepositoryAdapter decoratedRepository,
			IProfiler profiler)
		{
			_decoratedRepository = decoratedRepository;
			_profiler = profiler;
		}

	
		public UiFlowContextData CreateNew()
		{
			var result = new UiFlowContextData();
			result.CurrentScreenStep = ScreenName.PreStart;
			Attach(result);
			return result;
		}

		public async Task<UiFlowContextData> Get(string flowHandler)
		{
			using (_profiler.RecordStep("UiFlowContext-LoadByFlowHandler"))
			{
				flowHandler = flowHandler.ToLowerInvariant();
				try
				{
					return await Store.GetOrAdd(flowHandler, async k =>
					{
						var contextData = await _decoratedRepository.LoadByFlowHandler(k);
						contextData.SetRepository(this);
						foreach (var metadata in contextData.ViewModels.Values)
							//TODO:REVIEW THIS CASTING
							metadata.UserData.Metadata = metadata;
						return contextData;
					});
				}
				catch (Exception ex)
				{
					throw new KeyNotFoundException(flowHandler, ex);
				}
			}
		}

		public async Task<UiFlowContextData> CreateSnapshotOfContext(UiFlowContextData source)
		{
			if (!Store.ContainsKey(source.FlowHandler))
				throw new ArgumentException("the context must be attached before creating an snapshot");

			var ctx = source.CloneAsNew(this);

			Attach(ctx);


			UpdateClonedSteps();

			await UpdateContainer();

			await UpdateFollowingSnapshot();

			return ctx;

			void UpdateClonedSteps()
			{
				foreach (var stepData in ctx.ViewModels.Values) stepData.FlowHandler = ctx.FlowHandler;
			}

			async Task UpdateContainer()
			{
				if (ctx.IsInContainer())
				{
					var container = await Get(ctx.ContainerFlowHandler);

					container = await CreateSnapshotOfContext(container);
					var currentStepData = container.GetCurrentStepData<UiFlowScreenModel>().Metadata;
					if (currentStepData != null) currentStepData.ContainedFlowHandler = ctx.FlowHandler;


					ctx.ContainerFlowHandler = container.FlowHandler;
				}
			}

			async Task UpdateFollowingSnapshot()
			{
				if (!string.IsNullOrWhiteSpace(source.NextFlowHandler))
				{
					Store.TryRemove(source.NextFlowHandler, out var deleted);
					_removed.Add(source.NextFlowHandler);
				}

				source.NextFlowHandler = ctx.FlowHandler;
			}
		}


		public async Task<UiFlowContextData> GetRootContainerContext(UiFlowContextData ctx)
		{
			if (ctx.IsInContainer())
			{
				var containerContext = await Get(ctx.ContainerFlowHandler);
				return await GetRootContainerContext(containerContext);
			}

			return ctx;
		}

		public async Task<UiFlowContextData> GetRootContainerContext(string flowHandler)
		{
			return await GetRootContainerContext(await Get(flowHandler));
		}

		public async Task<UiFlowContextData> GetCurrentSnapshot(string flowHandler)
		{
			var actual = await Get(flowHandler);
			while (actual?.NextFlowHandler != null) actual = await Get(actual.NextFlowHandler);

			return actual;
		}

		public async Task Flush()
		{
			foreach (var contextData in Store.Values) await _decoratedRepository.Save(await contextData);

			foreach (var removedHandler in _removed) await _decoratedRepository.Remove(removedHandler);

			Store.Clear();
			_removed.Clear();
		}

		public async Task<UiFlowContextData> GetLastSnapshot()
		{
			return(await GetAll()).LastOrDefault();
		}

		private void Attach(UiFlowContextData contextData)
		{
			contextData.SetRepository(this);
			Store.AddOrUpdate(contextData.FlowHandler,a=> Task.FromResult(contextData),  (a, b) => Task.FromResult(contextData));
		}

		public async Task<IEnumerable<UiFlowContextData>> GetAll()
		{
			return (await _decoratedRepository.GetAll()).OrderBy(x => x.CreatedOn).ToArray();
		}
	}
}
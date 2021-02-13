using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Metadata;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.DataSources
{
	[DebuggerDisplay("{FlowType}-{CurrentScreenStep}-({FlowHandler}) ->Next({NextFlowHandler})")]
	public sealed class UiFlowContextData : IUiFlowContextData
	{
		private string _flowHandler = FlowHandlerProvider.Next();
		private string _nextFlowHandler;
		[JsonIgnore]
		private IUiFlowContextRepository _flowContextRepository;

		/// <summary>
		///     Relevant for snapshots it contains the continuation flow
		/// </summary>
		public string NextFlowHandler
		{
			get => _nextFlowHandler;
			set
			{
#if DEBUG || FrameworkDevelopment
				//if (int.Parse(value) <= int.Parse(FlowHandler))
				//	throw new ArgumentException($"Invalid next:{value} cannot be smaller than {FlowHandler}");
#endif
				_nextFlowHandler = value;
			}
		}

		[JsonProperty]
		internal IDictionary<ScreenName, RuntimeScreenMetaData> ViewModels { get; set; } =
			new Dictionary<ScreenName, RuntimeScreenMetaData>();


		public bool HasStepData => GetCurrentStepData<UiFlowScreenModel>() != null;

		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public string FlowHandler
		{
			get => _flowHandler;
			set => _flowHandler = value.ToLowerInvariant();
		}

		public ScreenName CurrentScreenStep { get; set; }

		public string ContainerFlowHandler { get; set; }
		public string FlowType { get; set; }


		public TViewModel GetCurrentStepData<TViewModel>() where TViewModel : UiFlowScreenModel
		{
			return GetStepData<TViewModel>(CurrentScreenStep);
		}

		public void SetCurrentStepData<TViewModel>(TViewModel viewModel) where TViewModel : UiFlowScreenModel
		{
			SetStepData(CurrentScreenStep, viewModel);
		}

		public TViewModel GetStepData<TViewModel>(ScreenName screenStep) where TViewModel : UiFlowScreenModel
		{
			return !ViewModels.ContainsKey(screenStep) ? null : (TViewModel) ViewModels[screenStep].UserData;
		}

		public TViewModel GetStepData<TViewModel>() where TViewModel : UiFlowScreenModel
		{
			var candidates = ViewModels.Values.Where(x => x.UserData is TViewModel).ToArray();
			if (candidates.Length > 1)
				throw new InvalidOperationException(
					"Step must be specified since there are more than one Viewmodel of " + typeof(TViewModel).FullName);

			return (TViewModel) candidates.SingleOrDefault()?.UserData;
		}


		public void RemoveStepViewModel(ScreenName screenStep)
		{
			if (ViewModels.ContainsKey(screenStep)) ViewModels.Remove(screenStep);
		}

		public void SetStepData<TViewModel>(ScreenName screenStep, TViewModel stepData, bool updateIfExisting = true)
			where TViewModel : UiFlowScreenModel
		{
			if (screenStep == (ScreenName) "Step5ReviewAndComplete")
			{

			}
			if (!stepData.IsValidFor(screenStep))
				throw new InvalidOperationException($"{stepData.GetType().FullName} is not valid for {screenStep}");

			if (!stepData.Metadata.IsWellFormed(out var reason)) throw new InvalidOperationException(reason);
			if (!ViewModels.ContainsKey(screenStep))
			{
				ViewModels.Add(screenStep, stepData.Metadata);
			}
			else
			{
				if (updateIfExisting) ViewModels[screenStep] = stepData.Metadata;
			}
		}

		public void Reset()
		{
			//keep the pre-start as its the invocation data
			var keep = ViewModels.SingleOrDefault(x => x.Value.UserData.IsValidFor(ScreenName.PreStart));
			ViewModels.Clear();
			ViewModels.Add(keep);
		}

		internal void SetRepository(IUiFlowContextRepository repository)
		{
			this._flowContextRepository = repository;
		}
		public ContextError LastError { get; set; }

		public List<EventLogEntry> EventsLog { get; set; } = new List<EventLogEntry>();

		[JsonIgnore] public List<ScreenEvent> CurrentEvents { get; } = new List<ScreenEvent>();

		public bool IsInContainer()
		{
			return !string.IsNullOrWhiteSpace(ContainerFlowHandler);
		}

		public async Task<UiFlowScreenModel> GetCurrentStepContainedData(ContainedFlowQueryOption option)
		{
			UiFlowScreenModel result=null;
			var currentStepData = this.GetCurrentStepData<UiFlowScreenModel>();
			if (currentStepData != null)
			{
				switch (option)
				{
					case ContainedFlowQueryOption.Immediate:
					{
						var containedFlowHandler = currentStepData.GetContainedFlowHandler();
						if (containedFlowHandler != null)
						{
							var containedFlow = await _flowContextRepository.Get(containedFlowHandler);
							result = containedFlow.GetCurrentStepData<UiFlowScreenModel>();
						}
					}
						break;
					case ContainedFlowQueryOption.Last:
					{
						var containedFlowHandler = currentStepData.GetContainedFlowHandler();
						if (containedFlowHandler != null)
						{
							var containedFlow = await _flowContextRepository.Get(containedFlowHandler);
							result = await containedFlow.GetCurrentStepContainedData(option);
						}
						else
						{
							result = GetCurrentStepData<UiFlowScreenModel>();
						}
					}
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(option), option, null);
				}
			}

			return result;
		}

		public UiFlowContextData CloneAsNew(IUiFlowContextRepository repository)
		{
			_flowContextRepository = null;
			var @new = this.CloneDeep();
			@new._flowHandler = FlowHandlerProvider.Next();
			@new._nextFlowHandler = null;
			@new.CreatedOn = DateTime.UtcNow;
			@new._flowContextRepository = repository;
			return @new;
		}

		private static class FlowHandlerProvider
		{
#if DEBUG || FrameworkDevelopment
			private static int _current = 1;
#endif
			public static string Next()
			{
#if DEBUG || FrameworkDevelopment
				return Interlocked.Increment(ref _current).ToString();
#else
				return Guid.NewGuid().ToString().ToLowerInvariant();
#endif
			}
		}

		public class EventLogEntry
		{
			public ScreenName FromStep { get; set; }
			public ScreenEvent Event { get; set; }

			public ScreenName ToStep { get; set; }

			public override string ToString()
			{
				return $"{Event}: {FromStep} => {ToStep}";
			}
		}


		public class ContextError
		{
			public ScreenName OccurredOnStep { get; set; }
			public string ExceptionMessage { get; set; }
			public ScreenLifecycleStage LifecycleStage { get; set; }
		}
	}
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Facade.Metadata;
using S3.UiFlows.Core.Flows.Screens.ErrorHandling;
using S3.UiFlows.Core.Flows.Screens.Metadata;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.DefaultModels;
using Newtonsoft.Json;
using NLog;
using S3.UiFlows.Core.DataSources;

namespace S3.UiFlows.Core.Flows.Screens
{
	public abstract class UiFlowScreen:IUiFlowScreen
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();


		[JsonIgnore] private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

		[JsonIgnore] private bool _stepDataLoaded;

		//TODO: IGNORE??
		private  IReadOnlyDictionary<ScreenEvent, Func<ScreenEvent, IUiFlowContextData, Task>> _eventHandlers;
		internal IEnumerable<ScreenTransition> Transitions { get; set; }

		public abstract ScreenName ScreenStep { get; }

		public string GetStepName()
		{
			return ScreenStep;
		}

		/// <summary>
		///     Default name is the same as the step
		/// </summary>
		public virtual string ViewPath => GetType().Name;

		public ScreenLifecycleStage LifecycleStage { get; private set; }

		public abstract string IncludedInFlowTypeAsString();

		
		public async Task<UiFlowScreenModel> RefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel, IDictionary<string, object> stepViewCustomizations = null)
		{
			TraceBegin(nameof(RefreshStepDataAsync), contextData);
			
			var result = originalScreenModel;

			if (!_stepDataLoaded)
				await _syncLock.AsyncCriticalSection(async () =>
					{
						if (!_stepDataLoaded)
						{
							LifecycleStage = ScreenLifecycleStage.RefreshingStepData;
							_stepDataLoaded = true;

							var onRefreshStepDataAsync =
								await OnRefreshStepDataAsync(contextData, originalScreenModel, stepViewCustomizations);
							await OnScreenLoadCompletedAsync(contextData);
							result = onRefreshStepDataAsync;
							LifecycleStage = ScreenLifecycleStage.RefreshStepDataCompleted;
						}
					}
				);
			TraceEnd(nameof(RefreshStepDataAsync), contextData);
			
			return result;
		}


		public async Task<UiFlowScreenModel> CreateStepDataAsync(IUiFlowContextData contextData)
		{
			TraceBegin(nameof(CreateStepDataAsync), contextData);
			LifecycleStage = ScreenLifecycleStage.CreatingStepData;
			var result = await OnCreateStepDataAsync(contextData);

			_stepDataLoaded = true;
			await OnScreenLoadCompletedAsync(contextData);
			LifecycleStage = ScreenLifecycleStage.CreateStepDataCompleted;
			TraceEnd(nameof(CreateStepDataAsync), contextData);

			return result;
		}


		public bool ValidateTransitionAttempt(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData, out string errorMessage)
		{
			TraceBegin(nameof(HandleStepEvent), contextData, triggeredEvent);
			LifecycleStage = ScreenLifecycleStage.ValidatingTransition;
			var isValid = OnValidateTransitionAttempt(triggeredEvent, contextData, out errorMessage);
			LifecycleStage = isValid
				? ScreenLifecycleStage.ValidateTransitionCompleted
				: ScreenLifecycleStage.ValidateTransitionCompletedWithErrors;
			TraceEnd(nameof(HandleStepEvent), contextData, triggeredEvent);
			return isValid;
		}

		public async Task HandleStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			TraceBegin(nameof(HandleStepEvent), contextData, triggeredEvent);
			LifecycleStage = ScreenLifecycleStage.HandlingEvent;
			//TDO: inline next
			await OnHandlingStepEvent(triggeredEvent, contextData);
			var uiFlowStepData = contextData.GetCurrentStepData<UiFlowScreenModel>();
			if (uiFlowStepData != null) uiFlowStepData.Errors = new List<UiFlowUserInputError>();
			LifecycleStage = ScreenLifecycleStage.HandleEventCompleted;
			TraceEnd(nameof(HandleStepEvent), contextData, triggeredEvent);
		}

		

		public virtual bool MustExecuteAnotherTransition(out ScreenEvent screenEvent)
		{
			screenEvent = null;
			return false;
		}

		public async Task<IDictionary<string, object>> ResolveContainedFlowStartInfo(IUiFlowContextData contextData,
			IDictionary<string, object> stepViewCustomizations)
		{
			LifecycleStage = ScreenLifecycleStage.ResolvingContainedFlowStartInfo;
			var startInfo = await OnResolveContainedFlowStartInfo(contextData, stepViewCustomizations);
			LifecycleStage = ScreenLifecycleStage.ResolveContainedFlowStartInfoCompleted;
			return startInfo;
		}

		public IScreenFlowConfigurator DefineActionHandlersOnCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			TraceBegin(nameof(DefineActionHandlersOnCurrentScreen), contextData);
			LifecycleStage = ScreenLifecycleStage.DefiningTransitionsFromCurrentScreen;
			var cfg = OnConfiguringScreenEventHandlersAndNavigations(screenConfiguration, contextData);
			var internalScreenFlowConfigurator = ((IInternalScreenFlowConfigurator)cfg);
			internalScreenFlowConfigurator.AddErrorTransitionIfUndefined();
			this.Transitions = internalScreenFlowConfigurator.Transitions;
			this._eventHandlers = internalScreenFlowConfigurator.Handlers;
			LifecycleStage = ScreenLifecycleStage.DefineTransitionsFromCurrentScreenCompleted;
			TraceEnd(nameof(DefineActionHandlersOnCurrentScreen), contextData);
			return cfg;
		}



		protected virtual async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			return originalScreenModel;
		}

		protected virtual IScreenFlowConfigurator OnConfiguringScreenEventHandlersAndNavigations(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration;
		}


		protected virtual async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new EmptyScreenModel();
		}

		protected virtual bool OnValidateTransitionAttempt(ScreenEvent triggeredEvent,
			IUiFlowContextData contextData, out string errorMessage)
		{
			errorMessage = null;
			return true;
		}

		private async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			if (_eventHandlers.ContainsKey(triggeredEvent))
			{
				var eventHandler = _eventHandlers[triggeredEvent];
				await eventHandler(triggeredEvent, contextData);
			}
		}


		protected virtual Task OnScreenLoadCompletedAsync(IUiFlowContextData contextData)
		{
			return Task.CompletedTask;
		}

		protected virtual async Task<IDictionary<string, object>> OnResolveContainedFlowStartInfo(
			IUiFlowContextData contextData, IDictionary<string, object> stepViewCustomizations)
		{
			return stepViewCustomizations;
		}


		private void Trace(string methodName, IUiFlowContextData contextData, string stage, string suffix = null)
		{
			Logger.Trace(() =>
				$"{methodName}({ScreenStep}.{stage}) - {contextData.FlowType}({contextData.FlowHandler}) - {suffix}");
		}

		private void TraceBegin(string methodName, IUiFlowContextData contextData, string suffix = null)
		{
			Trace(methodName, contextData, "BEGIN",suffix);
		}

		private void TraceEnd(string methodName, IUiFlowContextData contextData, string suffix = null)
		{
			Trace(methodName, contextData, "END",suffix);
		}


	}
	public abstract class UiFlowScreen<TFlowType> : UiFlowScreen, IUiFlowScreen<TFlowType>
	{

	

		public override string IncludedInFlowTypeAsString()
		{
			return IncludedInFlowType.ToString();
		}

		public abstract TFlowType IncludedInFlowType { get; }



	
	}
}
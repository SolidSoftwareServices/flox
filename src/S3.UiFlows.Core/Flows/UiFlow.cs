using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using S3.CoreServices.Profiling;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Facade.Metadata;
using S3.UiFlows.Core.Flows.Runtime;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.ErrorHandling;
using S3.UiFlows.Core.Flows.Screens.Metadata;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.DefaultModels;
using S3.UiFlows.Core.Infrastructure.StateMachine;
using NLog;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Registry;

namespace S3.UiFlows.Core.Flows
{
	internal sealed class UiFlow : IUiFlow
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IUiFlowContextRepository _contextRepository;

		private readonly IProfiler _profiler;
		private readonly IFlowRuntimeInfoResolver _flowRuntimes;
		private readonly IScreenMetadataResolver _screenMetadataResolver;
		private readonly IFlowInitializerMetadataResolver _flowInitializerMetadataResolver;
		private readonly IFlowsRegistry _flowsRegistry;

		private readonly List<IInternalScreenFlowConfigurator> _screensConfigurators =
			new List<IInternalScreenFlowConfigurator>();

		private readonly IInternalStateMachine _stateMachine;
		private string _flowTypeId;
		
		

		public UiFlow(
			IUiFlowContextRepository contextRepository,
			IInternalStateMachine stateMachine, IProfiler profiler, IFlowRuntimeInfoResolver flowRuntimes,
			IScreenMetadataResolver screenMetadataResolver,
			IFlowInitializerMetadataResolver flowInitializerMetadataResolver,IFlowsRegistry flowsRegistry)
		{

			_stateMachine = stateMachine;
			_profiler = profiler;
			_flowRuntimes = flowRuntimes;
			_screenMetadataResolver = screenMetadataResolver;
			_flowInitializerMetadataResolver = flowInitializerMetadataResolver;
			_flowsRegistry = flowsRegistry;
			_contextRepository = contextRepository;
		}

		internal IEnumerable<IInternalScreenFlowConfigurator> ScreenConfigurations => _screensConfigurators;

		public string FlowTypeId
		{
			get => _flowTypeId;
			set => _flowTypeId = value.ToLowerInvariant();
		}


		public (string ViewName, IUiFlowContextData ContextData) CurrentState { get; private set; }

		public async Task<UiFlowScreenModel> StartNew(string containerFlowHandler,
			IDictionary<string, object> flowInputData)
		{
			using (_profiler.Profile(FlowTypeId, nameof(StartNew)))
			{
				var newContext = await CreateNewContextData(containerFlowHandler);
				newContext.FlowType = FlowTypeId;
				var trigger = ScreenEvent.Start;
				var continueWithTrigger = trigger;
				var initializationStep = _flowRuntimes.GetFlowInfo(FlowTypeId).InitializationStep;
				if (initializationStep != null)
				{
					if (!initializationStep.Authorize()) return new UiFlowStepUnauthorized();

					try
					{
						continueWithTrigger =
							await initializationStep.InitializeContext(newContext, flowInputData, trigger);
					}
					catch (Exception ex)
					{
						newContext.LastError = new UiFlowContextData.ContextError
							{
								OccurredOnStep = newContext.CurrentScreenStep, 
								ExceptionMessage = ex.Message,
								LifecycleStage=ScreenLifecycleStage.FlowInitialization
						};
						return await _Execute(ScreenEvent.ErrorOccurred, newContext);
					}
				}

				return await _Execute(continueWithTrigger, newContext);
			}
		}

		public async Task<UiFlowScreenModel> Execute(ScreenEvent transitionTrigger,
			UiFlowScreenModel screenModel = null,
			Action<IEnumerable<UiFlowUserInputError>, UiFlowScreenModel> onErrors = null)
		{
			using (_profiler.Profile(FlowTypeId, nameof(Execute)))
			{
				var contextData = await GetContextData(screenModel);

				return await _Execute(transitionTrigger, contextData, onErrors);
			}
		}

		//TODO: FLOW HANDLER POSSIBLE REDUNDANT HERE
		public async Task<UiFlowScreenModel> GetCurrentStepData(string flowHandler,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			using (_profiler.Profile(FlowTypeId, $"{nameof(GetCurrentStepData)}"))
			{
				UiFlowContextData contextData;

				if (string.IsNullOrWhiteSpace(flowHandler)
				    || (contextData = await _contextRepository.Get(flowHandler)) == null)
					throw new InvalidOperationException();

				RestoreStateMachine(contextData);

				var screen = GetCurrentScreen(contextData);
				if (screen == null) throw new InvalidOperationException();


				//TODO: encryption service to be used in the model attributes and urls ONLY
				var stepData = contextData.GetCurrentStepData<UiFlowScreenModel>();
				stepData = await screen.RefreshStepDataAsync(contextData, stepData, stepViewCustomizations);
				contextData.SetStepData(contextData.CurrentScreenStep, stepData);

				CurrentState = ($"{screen.ViewPath}", contextData);
				if (contextData.IsInContainer())
				{
					var containerContext = await _contextRepository.Get(contextData.ContainerFlowHandler);
					var containerStep = containerContext.GetCurrentStepData<UiFlowScreenModel>();
					if (containerStep.Metadata.ContainedFlowHandler != flowHandler)
						containerStep.Metadata.ContainedFlowHandler = flowHandler;
				}


				//refreshed to the equivalent stepdata we just stored
				var currentStepData = contextData.GetCurrentStepData<UiFlowScreenModel>();
				return currentStepData;
			}
		}

		private IUiFlowScreen GetCurrentScreen(UiFlowContextData contextData)
		{
			return _flowRuntimes.GetFlowInfo(FlowTypeId).Screens[contextData.CurrentScreenStep];
		}

		public async Task<IUiFlowScreen> GetCurrentScreen(string flowHandler)
		{
			UiFlowContextData contextData;

			if (string.IsNullOrWhiteSpace(flowHandler)
			    || (contextData = await _contextRepository.Get(flowHandler)) == null)
				throw new InvalidOperationException();
			RestoreStateMachine(contextData);

			var screen = GetCurrentScreen(contextData);
			return screen;
		}

		public async Task<AppMetadata.FlowMetadata> GetMetadata()
		{
			RestoreStateMachine(_contextRepository.CreateNew());


			//TODO: REMOVE FROM HERE if possible
			var flowRuntime = _flowRuntimes.GetFlowInfo(FlowTypeId);

			var getScreensMetadata = flowRuntime
				.Screens
				.Values
				.Select(x => _screenMetadataResolver.GetMetadata(x));


			var initializerMetadata = _flowInitializerMetadataResolver.GetMetadata(flowRuntime.InitializationStep);
			return new AppMetadata.FlowMetadata
			{
				FlowName = FlowTypeId,
				//add initializer
				Screens = (await Task.WhenAll(getScreensMetadata)).ToList(),
				Initializer= await initializerMetadata
				
			};

		}

		public async Task SetCurrentStepData(UiFlowScreenModel screenModel)
		{
			using (_profiler.Profile(FlowTypeId, $"{nameof(SetCurrentStepData)}"))
			{
				var contextData = await _contextRepository.Get(screenModel.Metadata.FlowHandler);
				if (contextData == null) throw new InvalidOperationException("Invalid flow handler");
				RestoreStateMachine(contextData);

				contextData.SetCurrentStepData(screenModel);
			}
		}

		public string ToDotGraph()
		{
			RestoreStateMachine(_contextRepository.CreateNew());
			var dotGraph = _stateMachine.ToDotGraph();
			return dotGraph;
		}

		internal UiFlow RestoreStateMachine(UiFlowContextData contextData)
		{
			_stateMachine.BuildNew(contextData.CurrentScreenStep);
			var configurator = _stateMachine.Configure(ScreenName.PreStart);
			var flowRuntime= _flowRuntimes.GetFlowInfo(FlowTypeId);
			flowRuntime.InitializationStep.OnDefiningAdditionalInitialStateTransitions(
				configurator, contextData);
			_screensConfigurators.Add(configurator);
			foreach (var screenStep in flowRuntime.Screens.Keys.Where(x => x != ScreenName.PreStart))
			{
				configurator = _stateMachine.Configure(screenStep);

				flowRuntime.Screens[screenStep]
					.DefineActionHandlersOnCurrentScreen(configurator, contextData);

				configurator
					.OnEntry(() => OnStateEntry(contextData));
				_screensConfigurators.Add(configurator);
			}

			CurrentState = (null, contextData);
			return this;
		}

		private async Task OnStateEntry(UiFlowContextData contextData)
		{
			contextData.CurrentScreenStep = _stateMachine.CurrentStep;

			var screen = _flowRuntimes.GetFlowInfo(FlowTypeId).Screens[contextData.CurrentScreenStep];

			await AddViewModelToContext(screen);
			CurrentState = ($"{screen.ViewPath}", contextData);

			if (screen.MustExecuteAnotherTransition(out var transitionTrigger))
				await _Execute(transitionTrigger, contextData);

			async Task AddViewModelToContext(IUiFlowScreen target)
			{
				if (!contextData.HasStepData)
				{
					var stepData = await target.CreateStepDataAsync(contextData);
					if (stepData != null)
					{
						stepData.Metadata.FlowHandler = contextData.FlowHandler;
						stepData.Metadata.FlowType = contextData.FlowType;
						stepData.Metadata.FlowScreenName = stepData.GetType().FullName;

						contextData.SetStepData(target.ScreenNameId, stepData, false);
					}
				}
				else
				{
					var stepData = contextData.GetCurrentStepData<UiFlowScreenModel>();
					stepData = await target.RefreshStepDataAsync(contextData, stepData);
					contextData.SetStepData(target.ScreenNameId, stepData);
				}
			}
		}

		private async Task<UiFlowScreenModel> _Execute(ScreenEvent transitionTrigger, UiFlowContextData contextData,
			Action<IEnumerable<UiFlowUserInputError>, UiFlowScreenModel> onErrors = null)
		{
			contextData = await _contextRepository.CreateSnapshotOfContext(contextData);

			RestoreStateMachine(contextData);

			var initialStep = contextData.CurrentScreenStep;
			contextData.CurrentEvents.Add(transitionTrigger);
			string errorMessage = null;
			IUiFlowScreen screen=null;

			try
			{
				if (transitionTrigger != ScreenEvent.ErrorOccurred &&
				    contextData.CurrentScreenStep != ScreenName.PreStart)
				{
					screen = _flowRuntimes.GetFlowInfo(FlowTypeId).Screens[contextData.CurrentScreenStep];
					if (screen != null)
					{
						if (!screen.ValidateTransitionAttempt(transitionTrigger, contextData, out errorMessage))
							throw new AggregateException(new ValidationException(errorMessage));

						await InvokeOnStepCompleting(transitionTrigger, screen, contextData);
					}
				}

				await _stateMachine.ExecuteTrigger(transitionTrigger);
			}
			catch (Exception ex)
			{
				if (ex.GetType() != typeof(ValidationException)) Logger.Error(() => ex.ToString());
				var e = ex is AggregateException ? ex.InnerException : ex;
				try
				{
					screen = _flowRuntimes.GetFlowInfo(FlowTypeId).Screens[contextData.CurrentScreenStep];
				}
				catch
				{

				}
				contextData.LastError = new UiFlowContextData.ContextError
					{
						OccurredOnStep = contextData.CurrentScreenStep, 
						ExceptionMessage = e.Message,
						LifecycleStage = screen?.LifecycleStage??ScreenLifecycleStage.Unknown
				};
				await _stateMachine.ExecuteTrigger(ScreenEvent.ErrorOccurred);

				if (errorMessage == null)
				{
					if(!(ex is AggregateException))
						errorMessage = ex.Message;
					else
					{
						errorMessage = string.Join(Environment.NewLine,
							((AggregateException) ex).InnerExceptions.Select(x => x.Message));
					}
					
				}

				onErrors?.Invoke(new[] {new UiFlowUserInputError {ErrorMessage = errorMessage }},
					contextData.GetCurrentStepData<UiFlowScreenModel>());
			}

			contextData.EventsLog.Add(
				new UiFlowContextData.EventLogEntry
				{
					FromStep = initialStep,
					Event = transitionTrigger,
					ToStep = contextData.CurrentScreenStep
				}
			);
			contextData.CurrentEvents.Remove(transitionTrigger);
			return contextData.GetCurrentStepData<UiFlowScreenModel>();
		}

		private async Task InvokeOnStepCompleting(ScreenEvent transitionTrigger, IUiFlowScreen screen,
			UiFlowContextData contextData)
		{
			await screen.HandleStepEvent(transitionTrigger, contextData);
		}

		private async Task<UiFlowContextData> CreateNewContextData(string containerFlowHandler = null)
		{
			var contextData = _contextRepository.CreateNew();
			if (containerFlowHandler != null)
			{
				var container = await _contextRepository.Get(containerFlowHandler);
				var currentInContainer = container.GetCurrentStepData<UiFlowScreenModel>();

				contextData.ContainerFlowHandler = containerFlowHandler;

				currentInContainer.Metadata.ContainedFlowHandler = contextData.FlowHandler;
			}

			return contextData;
		}

		private async Task<UiFlowContextData> GetContextData(UiFlowScreenModel screenModel)
		{
			var contextData = await _contextRepository.Get(screenModel.Metadata.FlowHandler);
			contextData.SetCurrentStepData(screenModel);

			return contextData;
		}
	}
}
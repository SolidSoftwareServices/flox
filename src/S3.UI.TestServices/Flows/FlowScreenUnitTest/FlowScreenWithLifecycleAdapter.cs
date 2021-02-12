using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using S3.CoreServices.System.FastReflection;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Metadata;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Registry;
using S3.UiFlows.Mvc.Infrastructure;


namespace S3.UI.TestServices.Flows.FlowScreenUnitTest
{

	public sealed class FlowScreenWithLifecycleAdapter<TFlowScreen> : IFlowComponentAdapter
		where TFlowScreen : class, IUiFlowScreen
	{
		private readonly IUiFlowScreen _target;

		//TODO: MOCK
		private readonly UiFlowContextData _uiFlowContextData;

		internal FlowScreenWithLifecycleAdapter(TFlowScreen target, IFixture fixture)
		{
			Fixture = fixture;
			_target = target;

			SetRegistry();
			_uiFlowContextData = new UiFlowContextData
			{
				CurrentScreenStep = GetStep()
			};

			void SetRegistry()
			{
				var items = typeof(TFlowScreen).Namespace.Split('.').SkipLast(2);
				var flowsRootNamespace = string.Join('.', items);
				_target.SetPropertyValueFast(nameof(UiFlowScreen.Registry),
					new FlowsRegistry(typeof(TFlowScreen).Assembly, flowsRootNamespace,string.Empty));
			}
		}

		private IFixture Fixture { get; }

		public string GetFlowType()
		{
			return _target.IncludedInFlowType;
		}

		public string GetViewPath()
		{
			return _target.ViewPath;
		}

		public async Task<TStepData> GetStepData<TStepData>() where TStepData : UiFlowScreenModel
		{
			return _uiFlowContextData.GetStepData<TStepData>();
		}

		public async Task<TStepData> GetCurrentStepData<TStepData>() where TStepData : UiFlowScreenModel
		{
			return _uiFlowContextData.GetStepData<TStepData>(GetStep());
		}

		public ScreenName GetStep()
		{
			return _target.ScreenStep;
		}

		public FlowScreenWithLifecycleAdapter<TFlowScreen> SetStepData<TStepData>(TStepData stepData)
			where TStepData : UiFlowScreenModel
		{
			return SetStepData(stepData, GetStep());
		}

		public FlowScreenWithLifecycleAdapter<TFlowScreen> SetStepData<TStepData>(TStepData stepData,
			ScreenName forStep) where TStepData : UiFlowScreenModel
		{
			stepData.Metadata.FlowScreenName = forStep;
			stepData.Metadata.FlowHandler = Fixture.Create<string>();
			_uiFlowContextData.SetStepData(forStep, stepData);
			return this;
		}

		public UiFlowScreenModel CreateStepData()
		{
			return _target.CreateStepDataAsync(_uiFlowContextData).GetAwaiter().GetResult();
		}

		public UiFlowScreenModel RefreshStepData(IDictionary<string, object> routeParameters = null)
		{
			return _target.RefreshStepDataAsync(_uiFlowContextData,
				_uiFlowContextData.GetCurrentStepData<UiFlowScreenModel>(), routeParameters).GetAwaiter().GetResult();
		}

		public ScreenName ExecuteErrorEvent(string errorMessage)
		{
			_uiFlowContextData.LastError = new UiFlowContextData.ContextError
				{
					OccurredOnStep = _uiFlowContextData.CurrentScreenStep,
					ExceptionMessage = errorMessage
					,LifecycleStage = ScreenLifecycleStage.Unknown
				};
			return RunNavigation(ScreenEvent.ErrorOccurred);
		}

		public ScreenName ExecuteEvent(ScreenEvent eventToTrigger)
		{
			var cfg = new TestFlowScreenConfigurator(_target);
			_target.DefineActionHandlersOnCurrentScreen(cfg, _uiFlowContextData);
			_target.HandleStepEvent(eventToTrigger, _uiFlowContextData).Wait();
			return RunNavigation(eventToTrigger);
		}

		public ScreenName RunNavigation(ScreenEvent eventToTrigger)
		{
			var navigations = new TestFlowScreenConfigurator(_target);
			_target.DefineActionHandlersOnCurrentScreen(navigations, _uiFlowContextData);
			return navigations.Execute(eventToTrigger);
		}

		/// <summary>
		///     It runs the validation
		/// </summary>
		/// <param name="eventToTrigger"></param>
		/// <param name="errorMessage"></param>
		/// <returns>null if the event is excluded from the validation</returns>
		public bool? RunValidation(ScreenEvent eventToTrigger, out string errorMessage)
		{
			if (GetCurrentStepData<UiFlowScreenModel>().GetAwaiter().GetResult().DontValidateEvents
				.Any(x => x == eventToTrigger))
			{
				errorMessage = null;
				return null;
			}

			return _target.ValidateTransitionAttempt(eventToTrigger, _uiFlowContextData, out errorMessage);
		}
	}
}
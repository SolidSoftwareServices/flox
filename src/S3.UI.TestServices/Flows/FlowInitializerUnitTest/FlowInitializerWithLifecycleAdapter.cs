using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using S3.CoreServices.System.FastReflection;
using S3.UI.TestServices.Flows.Shared;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Registry;
using S3.UiFlows.Mvc.Infrastructure;


namespace S3.UI.TestServices.Flows.FlowInitializerUnitTest
{

	public sealed class FlowInitializerWithLifecycleAdapter<TInitializer>: IFlowComponentAdapter
		where TInitializer : class, IUiFlowInitializationStep
	{
		private readonly IUiFlowInitializationStep _target;
		private bool _initialized;
		//TODO: MOCK
		private readonly UiFlowContextData _uiFlowContextData;
		internal FlowInitializerWithLifecycleAdapter(IUiFlowInitializationStep target)
		{
			_target = target;
			SetRegistry();
			_uiFlowContextData = new UiFlowContextData
			{
				CurrentScreenStep = ScreenName.PreStart
			};

			void SetRegistry()
			{
				var items = typeof(TInitializer).Namespace.Split('.').SkipLast(2);
				var flowsRootNamespace = string.Join('.', items);
				_target.SetPropertyValueFast(nameof(UiFlowScreen.Registry),
					new FlowsRegistry(typeof(TInitializer).Assembly, flowsRootNamespace, string.Empty));
			}
		}
		
		public string GetFlowType()
		{
			return _target.InitializerOfFlowType;
		}

		public async Task<ScreenEvent> Initialize(ExpandoObject input = null)
		{
			ThrowIfInitialized();
			
			var stepEvent = await _target.InitializeContext(_uiFlowContextData, input, ScreenEvent.Start);
			_initialized = true;
			return stepEvent;
		}

		private void ThrowIfInitialized()
		{
			if(_initialized) throw new InvalidOperationException("Already initialized");
		}
		private void ThrowIfNotInitialized()
		{
			if (!_initialized) throw new InvalidOperationException("The Adapter needs to be initialized first");
		}
		public bool Authorize(bool authorizationRequiresInitializationToRun=true)
		{
			if (authorizationRequiresInitializationToRun)
			{
				ThrowIfNotInitialized();
			}
			return _target.Authorize();
		}

		public async Task<TStepData> GetStepData<TStepData>() where TStepData : UiFlowScreenModel
		{
			return _uiFlowContextData.GetStepData<TStepData>();
		}

		public ScreenName RunNavigation(ScreenEvent eventToTrigger,UiFlowContextData contextData=null)
		{
			var navigations = new TestFlowCreateStepDataConfigurator(_target);
			_target.OnDefiningAdditionalInitialStateTransitions(navigations, contextData ?? _uiFlowContextData?? new UiFlowContextData());

			return navigations.Execute(eventToTrigger);
		}

		private ScreenName GetStepName()=> ScreenName.PreStart;

		public FlowInitializerWithLifecycleAdapter<TInitializer> SetStepData<TStepData>(TStepData stepData) where TStepData : UiFlowScreenModel
		{
			stepData.Metadata.FlowScreenName = ScreenName.PreStart;
			stepData.Metadata.FlowHandler = Guid.NewGuid().ToString();
			_uiFlowContextData.SetStepData(GetStepName(),stepData,true);
			return this;
		}
	}
}
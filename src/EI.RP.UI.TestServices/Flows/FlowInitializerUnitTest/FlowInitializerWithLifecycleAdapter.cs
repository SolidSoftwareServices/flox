using System;
using System.Diagnostics;
using System.Dynamic;
using System.Threading.Tasks;
using EI.RP.UI.TestServices.Flows.Shared;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;


namespace EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public sealed class FlowInitializerWithLifecycleAdapter<TInitializer,TFlowType>: IFlowComponentAdapter
		where TInitializer : class, IUiFlowInitializationStep
	{
		private readonly IUiFlowInitializationStep<TFlowType> _target;
		private bool _initialized;
		//TODO: MOCK
		private readonly UiFlowContextData _uiFlowContextData;
		internal FlowInitializerWithLifecycleAdapter(IUiFlowInitializationStep<TFlowType> target)
		{
			_target = target;
			_uiFlowContextData = new UiFlowContextData
			{
				CurrentScreenStep = ScreenName.PreStart
			};
		}
		
		public TFlowType GetFlowType()
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

		public FlowInitializerWithLifecycleAdapter<TInitializer, TFlowType> SetStepData<TStepData>(TStepData stepData) where TStepData : UiFlowScreenModel
		{
			stepData.Metadata.FlowScreenName = ScreenName.PreStart;
			stepData.Metadata.FlowHandler = Guid.NewGuid().ToString();
			_uiFlowContextData.SetStepData(GetStepName(),stepData,true);
			return this;
		}
	}
}
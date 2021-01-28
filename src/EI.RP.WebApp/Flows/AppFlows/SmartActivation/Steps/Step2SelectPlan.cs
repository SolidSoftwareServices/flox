using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using Newtonsoft.Json;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps
{
	public class Step2SelectPlan : SmartActivationScreen
	{
		protected string StepTitle => string.Join(" | ", "2. Price Plans", Title);

		public static class StepEvent
		{
			
			public static readonly ScreenEvent PlanSelected = new ScreenEvent(nameof(Step2SelectPlan), nameof(PlanSelected));
			
		}

		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IDomainCommandDispatcher _domainCommandDispatcher;

		public Step2SelectPlan(IDomainQueryResolver domainQueryResolver,
			IDomainCommandDispatcher domainCommandDispatcher)
		{
			_domainQueryResolver = domainQueryResolver;
			_domainCommandDispatcher = domainCommandDispatcher;
		}

		public override ScreenName ScreenStep => SmartActivationStep.Step2SelectPlan;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootScreenModel = contextData.GetStepData<SmartActivationFlowInitializer.StepsSharedData>();

			var screenModel = new ScreenModel
			{
				SourceFlowType = rootScreenModel.SourceFlowType
			};
			await SetPlans(contextData, screenModel);
			SetTitle(StepTitle, screenModel);

			return screenModel;
		}

		private async Task SetPlans(IUiFlowContextData contextData, ScreenModel screenModel)
		{
			screenModel.Plans = await _domainQueryResolver.GetSmartActivationPlans(contextData
				.GetStepData<SmartActivationFlowInitializer.StepsSharedData>().AccountNumber);
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventNavigatesTo(StepEvent.PlanSelected,SmartActivationStep.Step3PaymentDetails);
		}

		protected override async Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			var stepData = contextData.GetCurrentStepData<ScreenModel>();
			if (triggeredEvent == StepEvent.PlanSelected)
			{
				HandlePlanSelected();
			}


			void HandlePlanSelected()
			{
				if (stepData.SelectedPlanName == null )
					throw new InvalidOperationException("No plan selected");
				
				var plan = stepData.Plans.SingleOrDefault(x => x.PlanName == stepData.SelectedPlanName);
				
				var root = contextData.GetStepData<SmartActivationFlowInitializer.StepsSharedData>();
				
				root.SelectedPlanName = plan?.PlanName ?? throw new InvalidOperationException("Selected plan was not found");
				root.SelectedPlanFreeDay = stepData.SelectedFreeDay;
			}
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData, UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var rootScreenModel = contextData.GetStepData<SmartActivationFlowInitializer.StepsSharedData>();
			var stepData = contextData.GetCurrentStepData<ScreenModel>();

			stepData.Reset();
			stepData.SourceFlowType = rootScreenModel.SourceFlowType;
			await SetPlans(contextData, stepData);
			return await base.OnRefreshStepDataAsync(contextData, originalScreenModel, stepViewCustomizations);
		}

		public sealed class ScreenModel : UiFlowScreenModel
		{
			public ScreenModel() : base(false)
			{
			}

			public ResidentialPortalFlowType SourceFlowType { get; set; }

			[JsonIgnore]
			public IEnumerable<SmartPlan> Plans { get; set; }

			public string SelectedPlanName { get; set; }

			public DayOfWeek? SelectedFreeDay{ get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == SmartActivationStep.Step2SelectPlan;
			}

			public void Reset()
			{
				SelectedPlanName = null;
				SelectedFreeDay = null;
			}
		}
	}
}
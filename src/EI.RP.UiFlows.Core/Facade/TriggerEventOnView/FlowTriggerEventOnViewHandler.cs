using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Facade.FlowResultResolver;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.DefaultModels;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.UiFlows.Core.Facade.TriggerEventOnView
{
	class FlowTriggerEventOnViewHandler<TResult> : IFlowTriggerEventOnViewHandler<TResult>
	{
		private readonly IAppUiFlowsCollection _flows;
		private readonly IUiFlowContextRepository _contextRepository;
		private readonly IFlowResultResolverRequestHandler<TResult> _resultResolver;

		public FlowTriggerEventOnViewHandler(IAppUiFlowsCollection flows, IUiFlowContextRepository contextRepository, IFlowResultResolverRequestHandler<TResult> resultResolver)
		{
			_flows = flows;
			_contextRepository = contextRepository;
			_resultResolver = resultResolver;
		}

		public async Task<TResult> Execute(TriggerEventOnView<TResult> input)
		{
			TResult result;
			var model = input.Model;


			var flow = await _flows.GetByFlowHandler(model.FlowHandler);
			if (flow == null)
			{
				result= await _resultResolver.Execute(input.ToFlowResolverRequest(new UiFlowStepUnauthorized()));
			}
			else
			{
				var inputErrors=new UiFlowUserInputError[0];
				if (model.DontValidateEvents.All(x => x != input.Event))
				{
					inputErrors = input.OnExecuteValidation(model).ToArray();
				}

				var actual = await flow.GetCurrentStepData(model.FlowHandler);
				if (actual != null)
				{
					model = actual.UpdateWithChangesFrom(model, (s, d) => { s.Metadata.DateCreated = actual.Metadata.DateCreated; });
				}

				if (!inputErrors.Any())
				{
					model = await flow.Execute(input.Event, model, (errors, screenModel)=>
					{
						//TODO: prototype implementation --> REVISIT
						var userInputErrors = errors as UiFlowUserInputError[] ?? errors.ToArray();
						screenModel.Errors = userInputErrors;
						foreach (var error in userInputErrors)
						{
							input.OnAddModelError(error.MemberName, error.ErrorMessage);
						}
					});
				}
				else
				{
					model.Errors = inputErrors;
					await flow.SetCurrentStepData(model);
				}
				result= await _resultResolver.Execute(input.ToFlowResolverRequest(model));
				//TODO: MOVE OUTSIDE BLOCK???
				await _contextRepository.Flush();
			}


			return result;
		}

		
	}
}
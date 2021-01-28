using System;
using System.Diagnostics;
using System.Linq;
using EI.RP.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EI.RP.UiFlows.Mvc.AppFeatures
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public class UiFlowControllerRouteConvention<TFlowTypesEnum,TFlowsController> : IControllerModelConvention
	where TFlowsController: IUiFlowController
	{
		public void Apply(ControllerModel controller)
		{
			RegisterUiFlowControllerRoutes(controller);

		}

		private static void RegisterUiFlowControllerRoutes(ControllerModel controller)
		{

			if (controller.ControllerType == typeof(TFlowsController))
			{
				controller.Selectors.Clear();
				Enum.GetValues(typeof(TFlowTypesEnum)).Cast<TFlowTypesEnum>().All(flowType =>
				{
					controller.Selectors.Add(BuildSelectorModel($"{flowType}/[action]"));
					return true;
				});
			}
		}

		private static SelectorModel BuildSelectorModel(string template)
		{
			return new SelectorModel
			{
				//currently supported convention enum name is the route
				AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(template)),

			};
		}
	}
}
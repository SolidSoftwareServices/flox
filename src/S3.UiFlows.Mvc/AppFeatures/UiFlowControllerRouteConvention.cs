using System;
using System.Diagnostics;
using System.Linq;
using S3.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace S3.UiFlows.Mvc.AppFeatures
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	internal class UiFlowControllerRouteConvention<TFlowsController> : IControllerModelConvention
		where TFlowsController : IUiFlowController
	{
		private readonly FlowsRegistry _registry;

		public UiFlowControllerRouteConvention(FlowsRegistry registry)
		{
			_registry = registry;
		}

		public void Apply(ControllerModel controller)
		{
			RegisterUiFlowControllerRoutes(controller);

		}

		private void RegisterUiFlowControllerRoutes(ControllerModel controller)
		{
			if (controller.ControllerType == typeof(TFlowsController))
			{
				controller.Selectors.Clear();
				foreach (var flowType in _registry.AllFlows.Select(x=>x.Name))
				{
					controller.Selectors.Add(BuildSelectorModel($"{flowType}/[action]"));
				}
			}
		}

		private SelectorModel BuildSelectorModel(string template)
		{
			return new SelectorModel
			{
				//currently supported convention enum name is the route
				AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(template)),

			};
		}
	}
}
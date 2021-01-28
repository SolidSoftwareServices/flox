using System;
using System.Collections.Generic;
using System.Linq;
using S3.CoreServices.System;
using S3.Mvc.Core.Controllers;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
#if !FrameworkDeveloper
	using System.Diagnostics;
#endif
namespace S3.UiFlows.Mvc.AppFeatures
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public class UiFlowViewLocationExpander<TFlowTypesEnum> : IViewLocationExpander
	{
		private const string UiFlowTypeKey = "UiFlowType";

		private static readonly IEnumerable<string> NotSharedFlowSpecificComponents = Enum
			.GetValues(typeof(TFlowTypesEnum))
			.Cast<TFlowTypesEnum>()
			.Select(x =>
			{

				var ns = typeof(TFlowTypesEnum).Namespace
					.Replace($"{typeof(TFlowTypesEnum).Assembly.GetName().Name}.", string.Empty).Split('.').ToList();
				ns.Add(x.ToString());
				ns.Add("Components");
				return $"/{string.Join('/', ns)}/{{0}}/{{0}}{RazorViewEngine.ViewExtension}";

			})
			.ToArray();

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			if (context.ControllerName != null &&
			    context.ControllerName.Equals(typeof(UiFlowController).GetNameWithoutSuffix()))
			{
				var strVal = context.ActionContext.HttpContext.Request.Path.ToString().Split('/')
					.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
				if (!string.IsNullOrWhiteSpace(strVal)) context.Values[UiFlowTypeKey] = strVal;
			}
		}

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
			IEnumerable<string> viewLocations)
		{
			//TODO: calculate by exploration on load 

			//TODO: 
			if (context.Values.TryGetValue(UiFlowTypeKey, out var value))
				viewLocations = new List<string>(NotSharedFlowSpecificComponents)
				{
					$"/Flows/AppFlows/{value}/Views/{{0}}{RazorViewEngine.ViewExtension}",
					$"/Flows/AppFlows/{{0}}{RazorViewEngine.ViewExtension}",
					$"/Flows/SharedFlowComponents/Main/{{0}}/{{0}}{RazorViewEngine.ViewExtension}",
					$"/Views/{value}/{{0}}{RazorViewEngine.ViewExtension}",
				};


			return viewLocations;
		}
	}
}
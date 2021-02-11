using System;
using System.Collections.Generic;
using System.Linq;
using S3.Mvc.Core.Controllers;
using S3.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using S3.UiFlows.Core.Registry;

namespace S3.UiFlows.Mvc.AppFeatures
{
	internal class UiFlowViewLocationExpander : IViewLocationExpander
	{

		public UiFlowViewLocationExpander(IFlowsRegistry registry)
		{
			if (registry == null) throw new ArgumentNullException(nameof(registry));

			NotSharedFlowSpecificComponents = new Lazy<IEnumerable<string>>(() =>
			{
				return registry.AllFlows
					.Select(x => $"{x.ComponentsUrlPath}/{{0}}/{{0}}{RazorViewEngine.ViewExtension}")
					.ToArray();
			});
		}

		private const string UiFlowTypeKey = "UiFlowType";

		internal Lazy<IEnumerable<string>> NotSharedFlowSpecificComponents { get; }

		

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
				viewLocations = new List<string>(NotSharedFlowSpecificComponents.Value)
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
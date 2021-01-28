using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Ei.Rp.Web.DebugTools
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public class UiFlowDebugToolsViewLocationExpander: IViewLocationExpander
	{
		public void PopulateValues(ViewLocationExpanderContext context)
		{
			
		}

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			var lst = viewLocations.ToList();
			lst.Add($"/Flows/AppFlows/Components/FlowViewer/{{0}}{RazorViewEngine.ViewExtension}");

			return lst;

		}
	}
}
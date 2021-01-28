using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using EI.RP.UiFlows.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace EI.RP.UiFlows.Mvc.AppFeatures
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public class UiFlowControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
	{
		public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
		{
			void Populate(TypeInfo typeInfo)
			{
				feature.Controllers.Remove(typeInfo);
				feature.Controllers.Add(typeInfo);
			}

			Populate(typeof(UiFlowController).GetTypeInfo());
		}
	}
}

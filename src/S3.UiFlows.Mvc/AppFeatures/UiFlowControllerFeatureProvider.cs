using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using S3.UiFlows.Mvc.Components.Deferred;
using S3.UiFlows.Mvc.Controllers;

namespace S3.UiFlows.Mvc.AppFeatures
{

	public class UiFlowControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
	{
		public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
		{
			Populate(typeof(DeferredComponentPartialsController).GetTypeInfo());
			Populate(typeof(UiFlowController).GetTypeInfo());


			void Populate(TypeInfo typeInfo)
			{
				feature.Controllers.Remove(typeInfo);
				feature.Controllers.Add(typeInfo);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using Newtonsoft.Json;

namespace EI.RP.UiFlows.Core.Facade.Metadata
{
	class FlowInitializerMetadataResolver : IFlowInitializerMetadataResolver
	{
		public async Task<AppMetadata.FlowMetadata.FlowStepMetadata> GetMetadata(IUiFlowInitializationStep initializationStep)
		{
			return new AppMetadata.FlowMetadata.FlowStepMetadata
			{
				Data = await ResolveScreenDataMetadata(initializationStep),
				Navigations = await ResolveUserNavigationsMetadata(initializationStep)
			};
		}

		private async Task<IEnumerable<AppMetadata.FlowMetadata.NavigationMetadata>> ResolveUserNavigationsMetadata(IUiFlowInitializationStep uiFlowScreen)
		{
			throw new NotSupportedException("navigations metadata resolver needs to be implemented first");
			return new AppMetadata.FlowMetadata.NavigationMetadata[0];
		}

		private async Task<AppMetadata.FlowMetadata.ScreenDataModelMetadata> ResolveScreenDataMetadata(IUiFlowInitializationStep uiFlowScreen)
		{
			var type = uiFlowScreen.GetType();
			Type dataType = null;
			while (type!=null&&(dataType = type.GetNestedTypes().SingleOrDefault(x => x.BaseTypes().Contains(typeof( InitialFlowScreenModel)))) == null)
			{
				type = type.BaseType;
			}

			AppMetadata.FlowMetadata.ScreenDataModelMetadata resolveScreenDataMetadata = null;
			if (dataType != null)
			{
				resolveScreenDataMetadata = new AppMetadata.FlowMetadata.ScreenDataModelMetadata
				{
					TypeName = dataType.FullName,
					Properties = dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public |
					                                    BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.DeclaredOnly)
						.Select(x => new AppMetadata.FlowMetadata.ScreenDataModelMetadata.ScreenProperty
						{
							TypeName = x.PropertyType.FullName,
							Name = x.Name,
							IsSerialized = x.GetCustomAttribute<JsonIgnoreAttribute>() == null
						}).ToArray()
				};
			}
			return resolveScreenDataMetadata;
		}
	}
}
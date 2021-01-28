using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using Newtonsoft.Json;

namespace EI.RP.UiFlows.Core.Facade.Metadata
{
	class ScreenMetadataResolver : IScreenMetadataResolver
	{
	

		public async Task<AppMetadata.FlowMetadata.FlowStepMetadata> GetMetadata(IUiFlowScreen uiFlowScreen)
		{
			var result = new AppMetadata.FlowMetadata.FlowStepMetadata
			{
				Data=await ResolveScreenDataMetadata(uiFlowScreen),
				Navigations=await ResolveUserNavigationsMetadata((UiFlowScreen)uiFlowScreen)
			};
			//TODO: resolve navigations
			


			return result;
		}

	

		private async Task<List<AppMetadata.FlowMetadata.NavigationMetadata>> ResolveUserNavigationsMetadata(UiFlowScreen uiFlowScreen)
		{
			return uiFlowScreen.Transitions.Select(x => new AppMetadata.FlowMetadata.NavigationMetadata
			{
				DestinationScreen = x.DestinationScreen,
				EventName = x.EventName,
				DisplayName = x.DisplayName,
				
			}).ToList();
		}

		private async Task<AppMetadata.FlowMetadata.ScreenDataModelMetadata> ResolveScreenDataMetadata(
			IUiFlowScreen uiFlowScreen)
		{
			var type = uiFlowScreen.GetType();
			Type dataType = null;
			while (type!=null && (dataType = type.GetNestedTypes().SingleOrDefault(x => x.Implements<IUiFlowScreenModel>())) == null && type.BaseType != null)
			{
				type = type.BaseType;
			}

			AppMetadata.FlowMetadata.ScreenDataModelMetadata resolveScreenDataMetadata=null;
			if (dataType != null)
			{
				resolveScreenDataMetadata = new AppMetadata.FlowMetadata.ScreenDataModelMetadata
				{
					TypeName = dataType.FullName,
					Properties = dataType.GetProperties(BindingFlags.Instance | BindingFlags.Public |
					                                    BindingFlags.GetProperty | BindingFlags.SetProperty|BindingFlags.DeclaredOnly)
						.Select(x => new AppMetadata.FlowMetadata.ScreenDataModelMetadata.ScreenProperty
						{
							TypeName = x.PropertyType.FullName,
							Name = x.Name,
							IsSerialized= x.GetCustomAttribute<JsonIgnoreAttribute>()==null
						}).ToArray()
				};
			}
			return resolveScreenDataMetadata;

		}
	}
}
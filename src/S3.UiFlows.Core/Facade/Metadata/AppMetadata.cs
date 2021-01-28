using System;
using System.Collections.Generic;

namespace S3.UiFlows.Core.Facade.Metadata
{
	public class AppMetadata
	{
		public List<FlowMetadata> Flows { get; internal set; } = new List<FlowMetadata>();
		public class FlowMetadata
		{
			public List<FlowStepMetadata> Screens { get; internal set; } = new List<FlowStepMetadata>();
			public string FlowName { get; set; }
			public FlowStepMetadata Initializer { get; set; }


			public class FlowStepMetadata
			{

				public ScreenDataModelMetadata Data { get; set; }
				public IEnumerable<NavigationMetadata> Navigations { get; set; }

			}

			public class ScreenDataModelMetadata
			{
				public IEnumerable<ScreenProperty> Properties { get; set; }
				public string TypeName { get; set; }

				public class ScreenProperty
				{
					public string TypeName { get; set; }
					public string Name { get; set; }
					public bool IsSerialized { get; set; }
				}

			}
			public class NavigationMetadata
			{
				public string DestinationScreen { get; set; }
				public string EventName { get; set; }

				public string DisplayName { get; set; }
			}

		}
	}
}
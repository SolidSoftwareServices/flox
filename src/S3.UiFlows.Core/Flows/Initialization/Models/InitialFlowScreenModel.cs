using System;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using Newtonsoft.Json;

namespace S3.UiFlows.Core.Flows.Initialization.Models
{
	public abstract class InitialFlowScreenModel : UiFlowScreenModel
	{
#if DEBUG ||FRAMEWORKDEVELOPMENT

		protected InitialFlowScreenModel()
		{
			var type = GetType();
			if (type != typeof(InitialFlowEmptyScreenModel) && !type.Implements(typeof(IFlowInputContract)))
				throw new NotImplementedException(
					$"{type.FullName} must implement {typeof(IFlowInputContract).FullName}");
		}
#endif
		public string CallbackFlowHandler { get; set; }
		public string CallbackFlowEvent { get; set; }

		[JsonIgnore]
		public bool MustReturnToCaller => !string.IsNullOrWhiteSpace(CallbackFlowHandler) &&
		                                  !string.IsNullOrWhiteSpace(CallbackFlowEvent);

		public override bool IsValidFor(ScreenName screenStep)
		{
			return screenStep == ScreenName.PreStart;
		}
	}
}
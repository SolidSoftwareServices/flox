using Newtonsoft.Json;
using S3.UiFlows.Core.Flows.Initialization.Models;

namespace S3.UiFlows.Core.Flows.Screens.Models.Interop
{
	public abstract class StartFlowScreenModel<TFlowStartData, TResult> : UiFlowScreenModel, IStartFlowScreenModel
		where TFlowStartData : InitialFlowScreenModel
	{
		protected StartFlowScreenModel(string startFlowType, TFlowStartData startData = null, bool asContained = false,
			bool isContainer = false) : this(isContainer)
		{
			StartFlowType = startFlowType;
			StartData = startData;
			AsContained = asContained;
		}

		protected StartFlowScreenModel(bool isContainer = false) : base(isContainer)
		{
		}

		public TFlowStartData StartData { get; set; }

		public TResult CalledFlowResult { get; set; }

		public string StartFlowType { get; set; }

		public object StartDataAsObject()
		{
			return StartData;
		}

		public void SetFlowResult(object result)
		{
			CalledFlowResult = (TResult) result;
		}

		public bool AsContained { get; set; }
		public string CallbackFromFlowHandler { get; set; }
	}

	/// <summary>
	///     used only as an agnostic deserializable support in the assembly
	/// </summary>
	public class StartFlowScreenModel : StartFlowScreenModel<InitialFlowScreenModel, object>
	{
		public StartFlowScreenModel(
			string startFlowType,
			InitialFlowScreenModel startData = null,
			bool asContained = false) : base(startFlowType, startData, asContained)
		{
		}

		[JsonConstructor]
		private StartFlowScreenModel()
		{
		}
	}
}
namespace EI.RP.UiFlows.Core.Flows.Screens.Models.DefaultModels
{
	/// <summary>
	///     The minimum flow step data to be used when there is no step data, it allows avoiding to generate artificial step
	///     info
	/// </summary>
	public class EmptyScreenModel : UiFlowScreenModel
	{
		public override bool IsValidFor(ScreenName screenStep)
		{
			return true;
		}
	}
}
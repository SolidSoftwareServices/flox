namespace S3.UiFlows.Core.Flows.Screens.Models.Containers
{
	public class EmptyContainerScreenModel : UiFlowScreenModel
	{
		public EmptyContainerScreenModel() : base(true)
		{
		}

		public override bool IsValidFor(ScreenName screenStep)
		{
			return true;
		}
	}
}
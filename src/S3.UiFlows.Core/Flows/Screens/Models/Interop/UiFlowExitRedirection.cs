namespace S3.UiFlows.Core.Flows.Screens.Models.Interop
{
	/// <summary>
	///     Redirects outside of the flow
	/// </summary>
	public class UiFlowExitRedirection : UiFlowScreenModel
	{
		public UiFlowExitRedirection(string controllerName, string actionName)
		{
			ControllerName = controllerName.EndsWith("Controller")
				? controllerName.Substring(0, controllerName.Length - 10)
				: controllerName;
			ActionName = actionName;
		}

		public string ControllerName { get; }
		public string ActionName { get; }


		public override bool IsValidFor(ScreenName screenStep)
		{
			return true;
		}
	}
}
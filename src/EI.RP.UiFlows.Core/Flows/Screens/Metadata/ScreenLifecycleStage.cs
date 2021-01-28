namespace EI.RP.UiFlows.Core.Flows.Screens.Metadata
{
	/// <summary>
	/// Events are sorted in the order they happen
	/// </summary>
	
	public enum ScreenLifecycleStage
	{
		Unknown=0,
		FlowInitialization,
		
		CreatingStepData,
		CreateStepDataCompleted,
		RefreshingStepData,
		RefreshStepDataCompleted,
		ValidatingTransition,
		ValidateTransitionCompleted,
		ValidateTransitionCompletedWithErrors,

		HandlingEvent,
		HandleEventCompleted,
		ResolvingContainedFlowStartInfo,
		ResolveContainedFlowStartInfoCompleted,
		DefiningTransitionsFromCurrentScreen,
		DefineTransitionsFromCurrentScreenCompleted,
	}
}
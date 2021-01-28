namespace S3.UiFlows.Core.Facade.Delegates
{
	public delegate TResult OnNewContainedScreenDelegate<TResult>(string containerHandler, string containerType, string flowType, object startInfo);
}
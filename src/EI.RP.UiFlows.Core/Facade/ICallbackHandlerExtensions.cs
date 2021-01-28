namespace EI.RP.UiFlows.Core.Facade
{
	internal static class ICallbackHandlerExtensions
	{
		public static ICallbackHandler<TResult> CopyCallbacksFrom<TResult>(this ICallbackHandler<TResult> destination,
			ICallbackHandler<TResult> from)
		{
			destination.OnExecuteEvent = from.OnExecuteEvent;
			destination.OnExecuteRedirection = from.OnExecuteRedirection;
			destination.OnExecuteUnauthorized = from.OnExecuteUnauthorized;
			destination.OnRedirectToRoot = from.OnRedirectToRoot;
			destination.OnRedirectToCurrent = from.OnRedirectToCurrent;
			destination.OnNewContainedScreen = from.OnNewContainedScreen;
			destination.OnStartNewFlow = from.OnStartNewFlow;
			destination.OnAddModelError = from.OnAddModelError;

			return destination;
		}
	}
}
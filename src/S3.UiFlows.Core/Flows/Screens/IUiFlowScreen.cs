using System.Collections.Generic;
using System.Threading.Tasks;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Flows.Screens.Metadata;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Infrastructure.DataSources;

namespace S3.UiFlows.Core.Flows.Screens
{
	
	public interface IUiFlowScreen
	{
		ScreenName ScreenStep { get; }

		/// <summary>
		///     Retrieves the screen view path
		/// </summary>
		string ViewPath { get; }

		ScreenLifecycleStage LifecycleStage { get; }

		/// <summary>
		///     It performs a validation when a transition from the current step has been requested and before the step completes
		/// </summary>
		/// <param name="triggeredEvent"></param>
		/// <param name="contextData"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		bool ValidateTransitionAttempt(ScreenEvent triggeredEvent, IUiFlowContextData contextData,
			out string errorMessage);

		string IncludedInFlowTypeAsString();

		Task<UiFlowScreenModel> CreateStepDataAsync(IUiFlowContextData contextData);

		/// <summary>
		///     It defines the navigation from a Given screen
		/// </summary>
		/// <param name="screenConfiguration"></param>
		/// <param name="contextData"></param>
		/// <returns></returns>
		IScreenFlowConfigurator DefineTransitionsFromCurrentScreen(IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData);

		/// <summary>
		///     it gets executed when the data needs to be refreshed
		/// </summary>
		/// <param name="contextData"></param>
		/// <param name="originalScreenModel"></param>
		/// <param name="stepViewCustomizations"></param>
		/// <returns>the updated stepdata</returns>
		Task<UiFlowScreenModel> RefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null);

		/// <summary>
		///     It handles the completion of the screen
		/// </summary>
		/// <param name="triggeredEvent"></param>
		/// <param name="contextData"></param>
		Task HandleStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData);

		/// <summary>
		///     It defines if the screen must execute directly another transition as a result of a condition
		/// </summary>
		/// <param name="screenEvent"></param>
		/// <returns></returns>
		bool MustExecuteAnotherTransition(out ScreenEvent screenEvent);

		string GetStepName();

		Task<IDictionary<string, object>> ResolveContainedFlowStartInfo(IUiFlowContextData contextData,
			IDictionary<string, object> stepViewCustomizations);
	}

	public interface IUiFlowScreen<TFlowType> : IUiFlowScreen
	{
		/// <summary>
		///     it returns the flow type where this screen lives
		/// </summary>
		TFlowType IncludedInFlowType { get; }
	}
}
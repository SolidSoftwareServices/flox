using System.Collections.Generic;
using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.Infrastructure.DataSources
{
	public interface IUiFlowContextData
	{
		ScreenName CurrentScreenStep { get; set; }
		string FlowType { get; set; }
		string FlowHandler { get; }

		UiFlowContextData.ContextError LastError { get; set; }

		//TODO: REFACTOR visibility
		List<UiFlowContextData.EventLogEntry> EventsLog { get; set; }
		List<ScreenEvent> CurrentEvents { get; }
		string ContainerFlowHandler { get; set; }
		TFlowStepData GetCurrentStepData<TFlowStepData>() where TFlowStepData : UiFlowScreenModel;
		void SetCurrentStepData<TFlowStepData>(TFlowStepData viewModel) where TFlowStepData : UiFlowScreenModel;
		TFlowStepData GetStepData<TFlowStepData>(ScreenName screenStep) where TFlowStepData : UiFlowScreenModel;
		TFlowStepData GetStepData<TFlowStepData>() where TFlowStepData : UiFlowScreenModel;


		void RemoveStepViewModel(ScreenName screenStep);

		void SetStepData<TFlowStepData>(ScreenName screenStep, TFlowStepData stepData, bool updateIfExisting = true)
			where TFlowStepData : UiFlowScreenModel;

		void Reset();
		bool IsInContainer();
		Task<UiFlowScreenModel> GetCurrentStepContainedData(ContainedFlowQueryOption option);
	}

	public enum ContainedFlowQueryOption
	{
		/// <summary>
		/// the immediately contained flow
		/// </summary>
		Immediate,

		/// <summary>
		/// the ultimately contained flow
		/// </summary>
		Last
	}
}
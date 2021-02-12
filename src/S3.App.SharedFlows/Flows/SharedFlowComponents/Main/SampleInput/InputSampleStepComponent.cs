using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Components;

namespace S3.App.Flows.SharedFlowComponents.Main.SampleInput
{
	[ViewComponent(Name= Name)]
	public class InputSampleStepComponent : FlowStepComponent<InputComponentInput, ViewModel>
	{
		private readonly IComponentViewModelBuilder<InputComponentInput, ViewModel> _viewModelBuilder;
		public const string Name = "SampleInput";
		public InputSampleStepComponent(IComponentViewModelBuilder<InputComponentInput, ViewModel> viewModelBuilder) : base(Name)
		{
			_viewModelBuilder = viewModelBuilder;
		}

		protected override async Task<ViewModel> ResolveComponentDataAsync(InputComponentInput input, UiFlowScreenModel screenModel)
		{
			return await _viewModelBuilder.Resolve(input, screenModel);
		}

		public class ViewModelBuilder : IComponentViewModelBuilder<InputComponentInput, ViewModel>
		{
			public async Task<ViewModel> Resolve(InputComponentInput componentInput, UiFlowScreenModel screenModelContainingTheComponent)
			{
				return new ViewModel
				{
					StringValue = componentInput.InputParameter
				};

			}
		}
	}
}
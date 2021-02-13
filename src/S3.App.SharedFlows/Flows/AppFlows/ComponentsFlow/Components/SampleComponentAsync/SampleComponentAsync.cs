using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3.CoreServices.Platform;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Components;

namespace S3.App.Flows.AppFlows.ComponentsFlow.Components.SampleComponentAsync
{
	[ViewComponent(Name= SampleComponent.SampleComponent.Name)]
	public class SampleComponentAsync : FlowStepComponent<SampleComponentInputAsync, SampleComponentViewModelAsync>
	{
		private readonly IComponentViewModelBuilder<SampleComponentInputAsync, SampleComponentViewModelAsync> _viewModelBuilder;
		public const string Name = nameof(SampleComponentAsync);
		public SampleComponentAsync(IPlatformSettings platformSettings,
			IComponentViewModelBuilder<SampleComponentInputAsync, SampleComponentViewModelAsync> viewModelBuilder)
			: base(
				
				"SampleComponentAsync",
				platformSettings
				, "CustomLoadingView.html"
			)
		{
			_viewModelBuilder = viewModelBuilder;
		}

		protected override bool DeferComponentLoad { get; } = true;

		protected override async Task<SampleComponentViewModelAsync> ResolveComponentDataAsync(SampleComponentInputAsync input, UiFlowScreenModel screenModel)
		{
			await Task.Delay(2500);

			return await _viewModelBuilder.Resolve(input, screenModel);
		}

		public class ViewModelBuilder : IComponentViewModelBuilder<SampleComponentInputAsync, SampleComponentViewModelAsync>
		{
			public async Task<SampleComponentViewModelAsync> Resolve(SampleComponentInputAsync componentInput,
				UiFlowScreenModel screenModelContainingTheComponent)
			{
				return new SampleComponentViewModelAsync
				{
					Value = componentInput.InputParameter,
					DataFromRestoredScreenModel=screenModelContainingTheComponent.FlowScreenName
				};
			}
		}
	}
}
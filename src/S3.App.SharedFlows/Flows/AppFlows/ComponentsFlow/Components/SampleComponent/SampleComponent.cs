using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Components;

namespace S3.App.Flows.AppFlows.ComponentsFlow.Components.SampleComponent
{
	[ViewComponent(Name= SampleComponent.Name)]
	public class SampleComponent : FlowStepComponent<SampleComponentInput, SampleComponentViewModel>
	{
		private readonly IComponentViewModelBuilder<SampleComponentInput, SampleComponentViewModel> _viewModelBuilder;
		public const string Name = "SampleComponent";
		public SampleComponent(IComponentViewModelBuilder<SampleComponentInput, SampleComponentViewModel> viewModelBuilder) : base(Name)
		{
			_viewModelBuilder = viewModelBuilder;
		}

		protected override async Task<SampleComponentViewModel> ResolveComponentDataAsync(SampleComponentInput input, UiFlowScreenModel screenModel)
		{
			return await _viewModelBuilder.Resolve(input, screenModel);
		}

		public class ViewModelBuilder : IComponentViewModelBuilder<SampleComponentInput, SampleComponentViewModel>
		{

			
			public async Task<SampleComponentViewModel> Resolve(SampleComponentInput componentInput, UiFlowScreenModel screenModelContainingTheComponent)
			{
				await Task.Yield();
				return await Task.Run(async () =>
				{
					await Task.Yield();
					Thread.Sleep(new Random().Next(1,5000));
					await Task.Yield();
					return new SampleComponentViewModel
					{
						Value = componentInput.InputParameter
					};
				});


			}
		}
	}
}
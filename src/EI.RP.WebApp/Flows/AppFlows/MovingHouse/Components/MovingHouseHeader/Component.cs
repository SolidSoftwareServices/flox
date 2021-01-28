using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;

using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.WebApp.DomainModelExtensions;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Flows.AppFlows.MovingHouse.Components.MovingHouseHeader
{
	[ViewComponent(Name = Name)]
	public class Component : FlowStepComponent<InputModel, ViewModel>
	{
		private readonly IComponentViewModelBuilder<InputModel, ViewModel> _viewModelBuilder;
		public const string Name = "MovingHouseHeader";


		public Component(IComponentViewModelBuilder<InputModel, ViewModel> viewModelBuilder) : base(Name)
		{
			_viewModelBuilder = viewModelBuilder;
		}

		protected override async Task<ViewModel> ResolveComponentDataAsync(InputModel input, UiFlowScreenModel screenModel)
		{
			return await _viewModelBuilder.Resolve(input, screenModel);
		}

		public class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
		{
			public async Task<ViewModel> Resolve(InputModel componentInput,
				UiFlowScreenModel screenModelContainingTheComponent=null)
			{
				if (!componentInput.StepNumber.IsBetween(0, 6))
					throw new ArgumentOutOfRangeException($"{nameof(InputModel.StepNumber)}");

				return new ViewModel
				{
					HeaderText = componentInput.MovingHouseType.ToDescriptionText(),
					PrnText = componentInput.MovingHouseType.ToPrnText(),
					CurrentStepNumber = componentInput.StepNumber,
					ShowProcess = componentInput.StepNumber.IsBetween(1,5),
				};
			}

		}
	}
}
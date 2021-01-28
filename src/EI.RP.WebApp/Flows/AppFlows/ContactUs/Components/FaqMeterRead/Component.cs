﻿using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.Components.FaqMeterRead
{
	[ViewComponent(Name = Name)]
	public class Component : FlowStepComponent<InputModel, ViewModel>
	{
		private readonly IComponentViewModelBuilder<InputModel, ViewModel> _builder;
		public const string Name = "FaqMeterRead";


		public Component(IComponentViewModelBuilder<InputModel, ViewModel> builder) : base(Name)
		{
			_builder = builder;
		}

		protected override async Task<ViewModel> ResolveComponentDataAsync(InputModel input, UiFlowScreenModel screenModel)
		{
			return await _builder.Resolve(input, screenModel);
		}
	}
}
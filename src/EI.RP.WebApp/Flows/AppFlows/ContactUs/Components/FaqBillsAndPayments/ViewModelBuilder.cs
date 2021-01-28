﻿using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.ContactUs.Components.FaqBillsAndPayments
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
	
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			return new ViewModel
			{
			};
		}
	}
}
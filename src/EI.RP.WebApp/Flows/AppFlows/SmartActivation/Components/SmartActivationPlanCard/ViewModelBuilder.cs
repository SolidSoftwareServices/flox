using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Components.SmartActivationPlanCard
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			var inputPlan = componentInput.Plan;
			var firstNotBold = inputPlan.PlanFeatures.FirstOrDefault(x => x.Contains('%'));
			var firstNotBoldIndex = firstNotBold != null ? inputPlan.PlanFeatures.IndexOf(firstNotBold) : -1;
			return new ViewModel()
			{
				ID = inputPlan.ID,
				PlanName = inputPlan.PlanName,
				PlanType = inputPlan.PlanType,
				BonusText = inputPlan.BonusDescription,
				Features = inputPlan.PlanFeatures.Select((featureText, index) => new ViewModel.PlanFeature
				{
					Highlighted = index < firstNotBoldIndex,
					Text = featureText
				}).ToArray(),
				Prices = inputPlan.PlanPrices,
				EstimatedAnnualBill = inputPlan.EstimatedAnnualBill,
				FirstYearCostPerKWh =  inputPlan.FirstYearCostPerKwh,
				EABDescription =  inputPlan.EABDescription,
				Position = inputPlan.OrderIndex,
				FreeDayOfElectricityChoice = inputPlan.FreeDayOfElectricityChoice,
				ContainerModel = screenModelContainingTheComponent,
				SelectedFreeDay = inputPlan.FreeDayOfElectricityChoice?.FirstOrDefault(),
				FreeDayOfElectricityDescription = inputPlan.FreeDayOfElectricityDescription,
				FullPricingInformation = inputPlan.FullPricingInformation
			};
		}
	}
}

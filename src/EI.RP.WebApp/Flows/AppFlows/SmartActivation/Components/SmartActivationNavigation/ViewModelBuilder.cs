using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Mvc.Components;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Components.SmartActivationNavigation
{
	internal class ViewModelBuilder : IComponentViewModelBuilder<InputModel, ViewModel>
	{
		private readonly IUserSessionProvider _userSessionProvider;

		public ViewModelBuilder(IUserSessionProvider userSessionProvider)
		{
			_userSessionProvider = userSessionProvider;
		}

		public async Task<ViewModel> Resolve(InputModel componentInput, UiFlowScreenModel screenModelContainingTheComponent = null)
		{
			return new ViewModel
			{
				IsAgentUser = _userSessionProvider.CurrentUserClaimsPrincipal.IsInRole(ResidentialPortalUserRole.AgentUser),
				SourceFlowType = componentInput.SourceFlowType,
				ShowCancel = componentInput.ShowCancel,
				FlowLocation = componentInput.SourceFlowType == ResidentialPortalFlowType.Accounts 
					? FlowActionTagHelper.StartFlowLocation.NotContained
					: componentInput.FlowLocation
			};
		}
	}
}

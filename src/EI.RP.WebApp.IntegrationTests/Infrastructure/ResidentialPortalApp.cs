using System;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Sut;
using EI.RP.UiFlows.Core.Infrastructure.DataSources.Repositories.Strategies;
using EI.RP.UiFlows.Mvc.Controllers;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;

namespace EI.RP.WebApp.IntegrationTests.Infrastructure
{
	internal partial class ResidentialPortalApp : SutAppBase<ResidentialPortalApp, Startup>
	{
		private readonly InMemoryUiFlowContextDataRepository _contextDataRepository;

		private ResidentialPortalApp(ResidentialPortalDeploymentType runningAsDeploymentType)
		{
			RunningAsDeploymentType = runningAsDeploymentType;
			DomainFacade = new DomainFacade();

			_contextDataRepository = new InMemoryUiFlowContextDataRepository();
		}

		public ResidentialPortalDeploymentType RunningAsDeploymentType { get; }

		public DomainFacade DomainFacade { get; }

		public IFixture Fixture { get; } = new Fixture().CustomizeDomainTypeBuilders();

		public string ResolveTitle(string title)
		{
			return $"{title} | Electric Ireland";
		}

		protected override void Reset()
		{
			DomainFacade.Reset();
			_contextDataRepository.Reset();
			base.Reset();
		}

		public async Task<ISutApp> WithValidSessionFor(string userName,
			ResidentialPortalUserRole userRole)
		{
			//TIME IS HIGH because is aways the first request
			using (Profiler.ProfileTest(nameof(WithValidSessionFor)))
			{
				DomainFacade.SetValidSessionFor(userName, userRole);
				switch (RunningAsDeploymentType)
				{
					case ResidentialPortalDeploymentType.Public:
						await ToAccounts();
						break;
					case ResidentialPortalDeploymentType.Internal:
						await StartFlow(ResidentialPortalFlowType.BusinessPartnersSearch);
						break;
					default:
						throw new NotSupportedException();
				}
			}

			return this;
		}


		public async Task<ResidentialPortalApp> ToLoginPage(string relativeUrl = "")
		{
			return await ToUrl(relativeUrl);
		}

		public async Task<ResidentialPortalApp> ActivateRegistration(string requestId, string activationKey)
		{
			return await ToUrl($"Login/ActivateRegistration?requestId={requestId}&activationKey={activationKey}");
		}

		public async Task<ResidentialPortalApp> ActivateForgotPassword(string requestId, string activationKey)
		{
			// Change to dynamic URL builder?
			return await ToUrl($"Login/ForgetPassword?requestId={requestId}&activationKey={activationKey}");
		}

		public async Task<ResidentialPortalApp> ToAccounts()
		{
			var flowType = ResidentialPortalFlowType.Accounts;
			return await StartFlow(flowType);
		}

		public async Task<ResidentialPortalApp> ToFirstAccount()
		{
			var flowType = ResidentialPortalFlowType.Accounts;
			return await (await StartFlow(flowType)).CurrentPageAs<AccountSelectionPage>().SelectFirstAccount();
		}

		public async Task<ResidentialPortalApp> ToAccount(string accountNumber)
		{
			var flowType = ResidentialPortalFlowType.Accounts;
			return await (await StartFlow(flowType)).CurrentPageAs<AccountSelectionPage>().SelectAccount(accountNumber);
		}

		public async Task<ResidentialPortalApp> ToAgentBusinessPartnerSearch()
		{
			return await StartFlow(ResidentialPortalFlowType.Agent);
		}

		private async Task<ResidentialPortalApp> StartFlow(ResidentialPortalFlowType flowType)
		{
			return await ToUrl($"{flowType}/{nameof(IUiFlowController.Init)}");
		}


		public AppUserConfigurator ConfigureUser(string userName, string userPassword)
		{
			return ConfigureUser(userName, userPassword, ResidentialPortalUserRole.OnlineUserRoi);
		}

		public AppUserConfigurator ConfigureUser(string userName, string userPassword, ResidentialPortalUserRole role)
		{
			return new AppUserConfigurator(DomainFacade, userName, userPassword, role);
		}
	}
}
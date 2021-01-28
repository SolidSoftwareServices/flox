using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.WebApp.Flows.AppFlows.AccountDashboard.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using Newtonsoft.Json;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.Steps
{
	public class AccountDashboard : UiFlowContainerScreen<ResidentialPortalFlowType>
	{
		private readonly IDomainQueryResolver _domainQueryResolver;
		private readonly IUserSessionProvider _userSessionProvider;

		public AccountDashboard(IDomainQueryResolver domainQueryResolver,
			IUserSessionProvider userSessionProvider)
		{
			_domainQueryResolver = domainQueryResolver;
			_userSessionProvider = userSessionProvider;
		}

		public override ResidentialPortalFlowType IncludedInFlowType =>
			ResidentialPortalFlowType.AccountDashboard;

		public static class StepEvent
		{
		}

		public override ScreenName ScreenStep =>
			AccountDashboardScreenStep.MyAccountDashboard;

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var rootData = contextData.GetStepData<AccountDashboardFlowInitializer.RootScreenModel>(ScreenName.PreStart);
			var data = await MapStepData(contextData, new ScreenModel(), true);

			await SetTitle(contextData, data);

			return data;
		}

		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel,
			IDictionary<string, object> stepViewCustomizations = null)
		{
			var refreshedStepData = originalScreenModel.CloneDeep();

			await SetTitle(contextData, (ScreenModel)refreshedStepData);

			return await MapStepData(contextData, (ScreenModel) refreshedStepData, false);
		}

		private async Task<ScreenModel> MapStepData(IUiFlowContextData contextData, ScreenModel data, bool isCreatingStep)
		{
			var rootData = contextData.GetStepData<AccountDashboardFlowInitializer.RootScreenModel>(ScreenName.PreStart);

			data.Account = await _domainQueryResolver.GetAccountInfoByAccountNumber(rootData.AccountNumber);

			if (isCreatingStep)
			{
				SetInitialContainedFlow();
			}

			data.IsSmartActivation = ResolveSmartActivation();
			data.ShowHeader = ResolveHeaderVisibility();
			data.ShowSubNavigation = ResolveSubNavigationVisibility();
			data.IsAgentUser = _userSessionProvider.CurrentUserClaimsPrincipal.IsInRole(ResidentialPortalUserRole.AgentUser);

			return data;

			void SetInitialContainedFlow()
			{
				if (data.Account.ClientAccountType == ClientAccountType.EnergyService)
				{
					data.SetContainedFlow(ResidentialPortalFlowType.EnergyServicesAccountOverview);

				}
				else if (data.Account.ClientAccountType == ClientAccountType.Electricity ||
				         data.Account.ClientAccountType == ClientAccountType.Gas)
				{
					data.SetContainedFlow(rootData.InitialFlow, rootData.InitialFlowStartType);
				}
				else
				{
					throw new NotSupportedException();
				}
			}

			bool ResolveSmartActivation()
			{
				return data.GetContainedFlow<ResidentialPortalFlowType>() == ResidentialPortalFlowType.SmartActivation;
			}

			bool ResolveHeaderVisibility()
			{
				var hiddenHeaderFor = new[]
				{
					ResidentialPortalFlowType.ChangePassword,
					ResidentialPortalFlowType.UserContactDetails,
					ResidentialPortalFlowType.TermsInfo,
					ResidentialPortalFlowType.Help,
					ResidentialPortalFlowType.ContactUs,
					ResidentialPortalFlowType.ProductAndServices,
					ResidentialPortalFlowType.CompetitionEntry,
					ResidentialPortalFlowType.PromotionEntry,
					ResidentialPortalFlowType.SmartActivation
				};

				return hiddenHeaderFor.All(x => x != data.GetContainedFlow<ResidentialPortalFlowType>());
			}

			bool ResolveSubNavigationVisibility()
			{
                var hiddenHeaderFor = new[]
                {
                    ResidentialPortalFlowType.ChangePassword,
                    ResidentialPortalFlowType.UserContactDetails,
                    ResidentialPortalFlowType.TermsInfo,
                    ResidentialPortalFlowType.Help,
                    ResidentialPortalFlowType.ContactUs,
                    ResidentialPortalFlowType.ProductAndServices,
                    ResidentialPortalFlowType.CompetitionEntry,
                    ResidentialPortalFlowType.PromotionEntry,
                    ResidentialPortalFlowType.SmartActivation
                };

                return !(hiddenHeaderFor.Any(x => x == data.GetContainedFlow<ResidentialPortalFlowType>()) || 
                         data.Account.ClientAccountType == ClientAccountType.EnergyService);
			}

		}

		protected override async Task<IDictionary<string, object>> OnResolveContainedFlowStartInfo(
			IUiFlowContextData contextData, IDictionary<string, object> stepViewCustomizations)
		{
			var stepData = contextData.GetCurrentStepData<ScreenModel>();
			var containedFlowType = stepData.GetContainedFlow<ResidentialPortalFlowType>();
			if (containedFlowType == null)
				return await base.OnResolveContainedFlowStartInfo(contextData, stepViewCustomizations);
			object result;
			switch (containedFlowType)
			{
				case ResidentialPortalFlowType.NoFlow:
				case ResidentialPortalFlowType.Help:
				case ResidentialPortalFlowType.ChangePassword:
                case ResidentialPortalFlowType.ProductAndServices:
                case ResidentialPortalFlowType.PromotionEntry:
					return await base.OnResolveContainedFlowStartInfo(contextData, stepViewCustomizations);
				case ResidentialPortalFlowType.AccountsPaymentConfiguration:
					var secondaryKey = stepViewCustomizations.Keys.SingleOrDefault(x =>
						x.Equals(nameof(IAccountsPaymentConfigurationFlowInput.SecondaryAccountNumber),
							StringComparison.InvariantCultureIgnoreCase));
					result = new
					{
						StartType = stepData.GetContainedFlowStartType(),
						stepData.Account.AccountNumber,
						SecondaryAccountNumber = secondaryKey != null ? stepViewCustomizations[secondaryKey] : null
					};
					break;
				case ResidentialPortalFlowType.AddGasAccount:
					result = new
					{
						ElectricityAccountNumber = stepData.Account.AccountNumber,
					};
					break;
				case ResidentialPortalFlowType.MakeAPayment:
				case ResidentialPortalFlowType.MeterReadings:
				case ResidentialPortalFlowType.TermsInfo:
					result = new
					{
						StartType = stepData.GetContainedFlowStartType(),
						stepData.Account.AccountNumber,
					};
					break;
				case ResidentialPortalFlowType.MovingHouse:
					result = new
					{
						InitiatedFromAccountNumber = stepData.Account.AccountNumber,
					};
					break;
				case ResidentialPortalFlowType.AccountAndMeterDetails:
				case ResidentialPortalFlowType.ContactUs:
				case ResidentialPortalFlowType.Usage:
				case ResidentialPortalFlowType.ElectricityAndGasAccountOverview:
				case ResidentialPortalFlowType.EnergyServicesAccountOverview:
				case ResidentialPortalFlowType.EnergyServiceBillsAndPayments:
				case ResidentialPortalFlowType.RequestRefund:
				case ResidentialPortalFlowType.UserContactDetails:
                case ResidentialPortalFlowType.CompetitionEntry:
				case ResidentialPortalFlowType.SmartActivation:
				case ResidentialPortalFlowType.Plan:
					result = new
					{
						stepData.Account.AccountNumber,
					};
					break;
				default:
					throw new ArgumentOutOfRangeException(containedFlowType.ToString());
			}

			return stepViewCustomizations.MergeObjects(true, result).ToExpandoObject();

		}

		private static async Task SetTitle(IUiFlowContextData contextData, ScreenModel model)
		{
			var last = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Last))
				?.ScreenTitle;
			var immediate = (await contextData.GetCurrentStepContainedData(ContainedFlowQueryOption.Immediate))
				?.ScreenTitle;

			model.ScreenTitle = last ?? immediate;
		}

		public sealed class ScreenModel : UiFlowScreenModel
		{
			public ScreenModel() : base(true)
			{
			}

			[JsonIgnore] public AccountInfo Account { get; set; }

			public bool IsSmartActivation { get; set; }
			public bool ShowHeader { get; set; }
			public bool ShowSubNavigation { get; set; }
			public bool IsAgentUser { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == AccountDashboardScreenStep.MyAccountDashboard;
			}
		}
	}
}
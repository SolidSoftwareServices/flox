using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps;
using System;
using System.Linq;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.AccountsPaymentConfiguration.Infrastructure.StepsDataBuilder
{
	class RootDataBuilder
	{
		private readonly AppUserConfigurator _appUserConfigurator;

		public RootDataBuilder(AppUserConfigurator appUserConfigurator)
		{
			_appUserConfigurator = appUserConfigurator;
		}

		public AccountsPaymentConfigurationFlowInitializer.RootScreenModel Create()
		{
			var fixture = _appUserConfigurator.DomainFacade.ModelsBuilder;
			var accountConfigurator = GetAccountConfigurator();
			var result = fixture.Build<AccountsPaymentConfigurationFlowInitializer.RootScreenModel>()
				.With(x => x.AccountNumber, accountConfigurator.Model.AccountNumber)
				.With(x=>x.FlowResult,fixture
					.Build<AccountsPaymentConfigurationResult>()
					.With(x=>x.ConfigurationSelectionResults
						,fixture.Build<AccountsPaymentConfigurationResult.AccountConfigurationInfo>()
							.With(x=>x.ConfigurationCompleted,false)
							.CreateMany(1))
					.Create())
				.Create();
			return result;
		}

		public ShowPaymentsHistory.ScreenModel CreateStepModel()
		{
			var accountConfigurator = GetAccountConfigurator();
			var stepData = _appUserConfigurator.DomainFacade.ModelsBuilder.Build<ShowPaymentsHistory.ScreenModel>()
				.With(x => x.AccountNumber, accountConfigurator.Model.AccountNumber)
				.Create();
			return stepData;
		}

		private CommonElectricityAndGasAccountConfigurator GetAccountConfigurator()
		{
			if (_appUserConfigurator.Accounts.Count() != 1)
			{
				throw new NotSupportedException();
			}

			var accountConfigurator =
				(CommonElectricityAndGasAccountConfigurator)_appUserConfigurator.ElectricityAccount() ??
				_appUserConfigurator.GasAccount();
			return accountConfigurator;
		}
	}
}
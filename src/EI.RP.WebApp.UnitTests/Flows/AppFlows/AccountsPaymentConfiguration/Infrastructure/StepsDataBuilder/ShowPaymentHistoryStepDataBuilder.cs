using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using System;
using System.Linq;
using ScreenModel = EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps.ShowPaymentsHistory.ScreenModel;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.AccountsPaymentConfiguration.Infrastructure.StepsDataBuilder
{
	class ShowPaymentHistoryStepDataBuilder
	{
		private readonly CommonElectricityAndGasAccountConfigurator _accountConfigurator;

		public ShowPaymentHistoryStepDataBuilder(AppUserConfigurator appUserConfigurator)
		{
			_accountConfigurator = appUserConfigurator.ElectricityAndGasAccountConfigurators.Single();
		}

		public ScreenModel Create(AppUserConfigurator appUserConfigurator)
		{
			var builder = _accountConfigurator.DomainFacade.ModelsBuilder;

			return builder
				.Build<ScreenModel>()
				.With(x => x.AccountNumber, _accountConfigurator.Model.AccountNumber)
				.With(x => x.PaymentMethod, _accountConfigurator.Model.PaymentMethod)
				.With(x => x.AccountClosed, false)
				.With(x => x.HasEqualMonthlyPayments, true)
				.With(x => x.DueDate, DateTime.Today)
				.With(x => x.IsDue, true)
				.Create();
		}
	}
}
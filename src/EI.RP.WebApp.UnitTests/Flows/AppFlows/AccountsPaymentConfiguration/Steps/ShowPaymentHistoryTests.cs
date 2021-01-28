using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.AccountsPaymentConfiguration.Infrastructure.StepsDataBuilder;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.AccountsPaymentConfiguration.Steps
{
	[TestFixture]
	internal class ShowPaymentHistoryTests
	{
		private FlowScreenTestConfigurator<ShowPaymentsHistory, ResidentialPortalFlowType>
			NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			//creates the domain layer facade
			if (domainFacade == null) domainFacade = new DomainFacade();
			//creates a flo test configurator
			return new FlowScreenTestConfigurator<ShowPaymentsHistory, ResidentialPortalFlowType>(domainFacade
					.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver);
		}
		internal class CaseModel
		{
			public AccountBillingActivity AccountBillingActivity { get; set; }
			public bool HasOverduePayment { get; set; }
			public bool isBillOverDue { get; set; }
			public DateTime DueDate { get; private set; }

			public override string ToString()
			{
				var isBillOverdue = this.DueDate < DateTime.Now;
				return $"When_BillsOverDue_{isBillOverDue}_HasOverduePayments_{this.HasOverduePayment}";
			}
		}

		public static IEnumerable<TestCaseData> CanResolveCases()
		{
			EuroMoney overDueAmount = 250.00;
			EuroMoney dueAmount = 50.00;

			var cases = new[]
			{
				new CaseModel { AccountBillingActivity =  new AccountBillingActivity(AccountBillingActivity.ActivitySource.Payment)
				{
					DueDate =  DateTime.Now,
					Amount = overDueAmount,
					CurrentBalanceAmount = overDueAmount,
					Description= "bill",
					InvoiceStatus= InvoiceStatus.AutomaticCollection,
					OriginalDate = DateTime.Now,
					RemainingAmount = overDueAmount,
					DueAmount = overDueAmount,
					NextBillDate = DateTime.Now.AddMonths(1),
				},
					HasOverduePayment = true
				},

				new CaseModel { AccountBillingActivity =  new AccountBillingActivity(AccountBillingActivity.ActivitySource.Payment)
				{
					DueDate = DateTime.Today.AddDays(-12),
					Amount = overDueAmount,
					CurrentBalanceAmount = overDueAmount,
					Description = "bill",
					InvoiceStatus = InvoiceStatus.AutomaticCollection,
					OriginalDate = DateTime.Today.AddDays(-12),
					RemainingAmount = overDueAmount,
					DueAmount = overDueAmount,
					NextBillDate = DateTime.Now.AddMonths(1),
				},
					HasOverduePayment = true
				},

				new CaseModel { AccountBillingActivity =  new AccountBillingActivity(AccountBillingActivity.ActivitySource.Payment)
				{
					DueDate = DateTime.Now.AddMonths(1),
					Amount = dueAmount,
					CurrentBalanceAmount = dueAmount,
					Description = "bill",
					InvoiceStatus = InvoiceStatus.AutomaticCollection,
					OriginalDate = DateTime.Now.AddMonths(1),
					RemainingAmount = dueAmount,
					DueAmount = dueAmount,
					NextBillDate = DateTime.Now.AddMonths(2),
				},
					HasOverduePayment = false
				}
			};
			foreach (var caseItem in cases)
			{
				var testCaseData = new TestCaseData(caseItem);
				testCaseData.SetName(caseItem.ToString());
				yield return new TestCaseData(caseItem);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public void ItCanSetCorrectData(CaseModel caseModel)
		{
			//Arrange
			var appUserConfigurator = new AppUserConfigurator(new DomainFacade());
			appUserConfigurator.AddElectricityAccount(configureDefaultDevice: false);
			appUserConfigurator.Execute();

			var rootDataBuilder = new RootDataBuilder(appUserConfigurator);
			var stepDataBuilder = new ShowPaymentHistoryStepDataBuilder(appUserConfigurator);
			var stepData = stepDataBuilder.Create(appUserConfigurator);
			var hasEqualMonthlyPayments = true;
			var equalizerPaymentSetupQueryResult = new EqualizerPaymentSetupInfo
			{
				CanSetUpEqualizer = true
			};
			var accountInfoQueryResult = new AccountInfo { IsOpen = true };
			var getBillingInfoQueryResult = new GeneralBillingInfo { CurrentBalanceAmount = EuroMoney.Zero };
			var getInvoicesByAccountNumberQueryResult = new AccountBillingActivity(AccountBillingActivity.ActivitySource.Payment) { InvoiceStatus = InvoiceStatus.Paid };
			var accountBilliingActivitiesList = new List<AccountBillingActivity>();
			accountBilliingActivitiesList.Add(caseModel.AccountBillingActivity);

			appUserConfigurator.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<EqualizerPaymentSetupInfoQuery, EqualizerPaymentSetupInfo>(It.IsAny<EqualizerPaymentSetupInfoQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(equalizerPaymentSetupQueryResult.ToOneItemArray().AsEnumerable()));
			appUserConfigurator.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<GeneralBillingInfoQuery, GeneralBillingInfo>(It.IsAny<GeneralBillingInfoQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(getBillingInfoQueryResult.ToOneItemArray().AsEnumerable()));
			appUserConfigurator.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<AccountInfoQuery, AccountInfo>(It.IsAny<AccountInfoQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(accountInfoQueryResult.ToOneItemArray().AsEnumerable()));
			appUserConfigurator.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(It.IsAny<AccountBillingActivityQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(getInvoicesByAccountNumberQueryResult.ToOneItemArray().AsEnumerable()));
			appUserConfigurator.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(It.IsAny<AccountBillingActivityQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(accountBilliingActivitiesList.AsEnumerable()));

			NewScreenTestConfigurator(appUserConfigurator.DomainFacade)
				.NewTestCreateStepDataRunner()
				.WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
				.WhenCreated()
				.ThenTheStepDataIs<WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps.ShowPaymentsHistory.ScreenModel>(actual =>
				{
					Assert.AreEqual(hasEqualMonthlyPayments, actual.HasEqualMonthlyPayments);
					if (caseModel.HasOverduePayment && actual.PaymentMethod.IsOneOf(
						PaymentMethodType.DirectDebit,
						PaymentMethodType.AlternativePayer,
						PaymentMethodType.DirectDebit,
						PaymentMethodType.DirectDebitNotAvailable,
						PaymentMethodType.Manual))
					{
						Assert.IsTrue(actual.OverduePayments.ToList().Count > 0);
					}
					else
					{
						Assert.IsTrue(actual.OverduePayments.ToList().Count == 0);
					}
				});

			NewScreenTestConfigurator(appUserConfigurator.DomainFacade)
				.NewTestRefreshStepDataRunner()
				.GivenTheCurrentStepDataBeforeRefreshIs(rootDataBuilder.CreateStepModel())
				.WhenRefreshed()
				.ThenTheStepDataIs<WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps.ShowPaymentsHistory.ScreenModel>(actual =>
				{
					Assert.AreEqual(hasEqualMonthlyPayments, actual.HasEqualMonthlyPayments);
				});
		}
	}
}
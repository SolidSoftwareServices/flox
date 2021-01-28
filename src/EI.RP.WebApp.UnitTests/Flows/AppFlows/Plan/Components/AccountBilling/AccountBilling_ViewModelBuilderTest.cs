using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;
using EI.RP.TestServices;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows.Plan.Components.AccountBilling;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Components.AccountBilling
{
	[TestFixture]
	internal class
		AccountBilling_ViewModelBuilderTest : UnitTestFixture<PlanPageComponentTestContext<ViewModelBuilder>,
			ViewModelBuilder>
	{
		[Theory]
		public async Task ItMapsPaperbillSettings(bool hasPaperbill)
		{
			var actual = await Context
				.WithPaperBill(hasPaperbill)
				.Sut.Resolve(new InputModel
				{
					AccountNumber = Context.AccountNumber					 
				});
			Assert.AreEqual(hasPaperbill, actual.PaperBill.HasPaperBill);
		}

		[Test]
		public async Task ItMapsContainersPaperbillEvents()
		{
			var on = (ScreenEvent) $"e.{Guid.NewGuid().ToString()}";
			var off = (ScreenEvent) $"e.{Guid.NewGuid().ToString()}";
			var actual = await Context
				.Sut.Resolve(new InputModel
				{
					AccountNumber = Context.AccountNumber,
					SwitchOnPaperBillEvent = on,
					SwitchOffPaperBillEvent = off
				});

			Assert.AreEqual(on, actual.PaperBill.SwitchOnPaperBillEvent);
			Assert.AreEqual(off, actual.PaperBill.SwitchOffPaperBillEvent);
		}

		public static IEnumerable<TestCaseData> ItMapsPaymentMethodCases()
		{
			foreach (var method in PaymentMethodType.AllValues)
			{
				yield return new TestCaseData(method).Returns(method);
			}
		}

		[TestCaseSource(nameof(ItMapsPaymentMethodCases))]
		public async Task<PaymentMethodType> ItMapsPaymentMethod(PaymentMethodType paymentMethod)
		{
			var actual = await Context
				.WithPaymentMethod(paymentMethod)
				.Sut.Resolve(new InputModel
				{
					AccountNumber = Context.AccountNumber,
				});
			return actual.PaymentMethod;
		}
		public static IEnumerable<TestCaseData> ItShowsPaperBillSectionCorrectlyCases()
		{
			return PlanDetailsComponentCasesBuilder.CreateTestCasesForResult("PaperBill",ResolveExpectedResult);

			bool ResolveExpectedResult(ClientAccountType selectedAccount, bool isOpen, bool isAlreadySmart, bool canBeSmart)
			{
				return isOpen &&  selectedAccount.IsElectricity();
			}
		}

		[TestCaseSource(nameof(ItShowsPaperBillSectionCorrectlyCases))]
		public async Task<bool> ItShowsPaperBillSectionCorrectly(bool hasElectricity, bool hasGas, bool isAlreadySmart,
			bool canBeSmart, ClientAccountType selectedAccount, bool isOpen, PaymentMethodType paymentType)
		{
			var actual = await Context
				.WithPaymentMethod(paymentType)
				.WithAccounts(hasElectricity, hasGas, isElectricityOpen: isOpen)
				.WithSelectedAccount(selectedAccount)
				.WithSmartConfiguration(isAlreadySmart, canBeSmart)
				.Sut.Resolve(new InputModel { AccountNumber = Context.AccountNumber });

			return actual.PaperBill.IsVisible;

		}
		public static IEnumerable<TestCaseData> ItMapsCanEnableMonthlyPaymentsCases()
		{
			return PlanDetailsComponentCasesBuilder.CreateTestCasesForResult("MonthlyPayments",ResolveExpectedResult);

			bool ResolveExpectedResult(ClientAccountType selectedAccount, bool isOpen, bool isAlreadySmart, bool canBeSmart)
			{
				return selectedAccount.IsElectricity() && isOpen && isAlreadySmart ;
			}
		}

		[TestCaseSource(nameof(ItMapsCanEnableMonthlyPaymentsCases))]
		public async Task<bool> ItMapsCanEnableMonthlyPayments(bool hasElectricity, bool hasGas, bool isAlreadySmart,
			bool canBeSmart, ClientAccountType selectedAccount, bool isOpen,PaymentMethodType paymentType)
		{
			var actual = await Context
				.WithPaymentMethod(paymentType)
				.WithAccounts(hasElectricity, hasGas, isElectricityOpen: isOpen)
				.WithSelectedAccount(selectedAccount)
				.WithSmartConfiguration(isAlreadySmart, canBeSmart)
				.Sut.Resolve(new InputModel {AccountNumber = Context.AccountNumber});

			return actual.MonthlyBilling.CanEnableMonthlyPayments;

		}
	}
}
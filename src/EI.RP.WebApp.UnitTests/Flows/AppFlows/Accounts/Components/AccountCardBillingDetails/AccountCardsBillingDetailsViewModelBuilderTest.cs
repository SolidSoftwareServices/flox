using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.FastReflection;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardBillingDetails;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Accounts.Components.AccountCardBillingDetails
{
	[TestFixture]
	internal class
		AccountCardsBillingDetailsViewModelBuilderTest : UnitTestFixture<AccountCardsBillingDetailsViewModelBuilderTest.TestContext,
			ViewModelBuilder>
	{

		public static IEnumerable<TestCaseData> ItResolves_Property_CanPayNow_Cases()
		{
			var fixture = new Fixture().CustomizeDomainTypeBuilders();
			var boolValues = new[] {true, false};
			var amounts = new EuroMoney[] {-10.0, 0.0, 10.0};
			foreach (var costCanBeEstimated in boolValues)
			foreach (var accountType in ClientAccountType.AllValues.Cast<ClientAccountType>())
			foreach (var paymentMethodType in PaymentMethodType.AllValues.Cast<PaymentMethodType>())
			foreach (var currentBalance in amounts)
			{
				var inputModel = new InputModel
				{
					AccountNumber = fixture.Create<string>(),
					AccountType = accountType
				};
				var expectedResult = !costCanBeEstimated && paymentMethodType.IsOneOf(PaymentMethodType.DirectDebit,PaymentMethodType.Manual) && currentBalance.ToDecimal().Value>=0.0M;
				yield return new TestCaseData(inputModel,costCanBeEstimated,paymentMethodType,currentBalance)
						.SetName($"costCanBeEstimated:{costCanBeEstimated} accountType:{accountType} paymentType:{paymentMethodType} currentBalance:{currentBalance}")
						.Returns(expectedResult)
					;
			}
		}
		[TestCaseSource(nameof(ItResolves_Property_CanPayNow_Cases))]
		public async Task<bool> ItResolves_Property_CanPayNow(InputModel inputModel,bool costCanBeEstimated,PaymentMethodType paymentMethodType,EuroMoney currentBalance)
		{
			var actual = await Context
				.WithInput(inputModel)
				.With(x => x.CostCanBeEstimated = costCanBeEstimated)
				.With(x => x.PaymentMethod = paymentMethodType)
				.With(x => x.CurrentBalanceAmount = currentBalance)
				.Sut
				.Resolve(inputModel);


			return actual.CanPayNow;
		}

		

		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			
			private readonly List<Action<LatestBillInfo>> _configurators=new List<Action<LatestBillInfo>>();
			private InputModel _inputModel;

			public TestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
			{ }

			public TestContext WithInput(InputModel inputModel)
			{
				_inputModel = inputModel;
				
				return this;
			}

			public TestContext With(Action<LatestBillInfo> modelConfigurator)
			{
				_configurators.Add(modelConfigurator);
				return this;
			}

			protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
			{
				var domainModel=Fixture.Create<LatestBillInfo>();
				_configurators.ForEach(x=>x(domainModel));

				var domainFacade=new DomainFacade();
				domainFacade.QueryResolver.ExpectQuery(new LatestBillQuery
				{
					AccountNumber = _inputModel.AccountNumber
				}, domainModel.ToOneItemArray());



				domainFacade.SetUpMocker(autoMocker);
				return base.BuildSut(autoMocker);
			}
		}
	}
}
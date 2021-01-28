using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EI.RP.DomainServices.InternalShared.ContractSales;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using Ei.Rp.DomainModels.SmartActivation;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;
using AutoFixture;
using Moq;

namespace EI.RP.DomainServices.UnitTests.Commands.SmartActivation.ActivateSmartMeter
{
	internal class ActivateSmartMeterCommandHandlerTest : CommandHandlerTest<ActivateSmartMeterCommandHandler, ActivateSmartMeterCommand>
	{
		internal class CaseModel
		{
			public bool IsMonthlyBilling { get; set; }
			public DayOfWeek? SelectedPlanFreeDay { get; set; }
			public bool SetupDirectDebit { get; set; }
			public string CaseName { get; set; }
		}

		private static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new[]
			{
				new CaseModel { IsMonthlyBilling = true, SelectedPlanFreeDay = null, SetupDirectDebit = false, CaseName = "MonthlyBilling" },
				new CaseModel { IsMonthlyBilling = true, SelectedPlanFreeDay = null, SetupDirectDebit = true, CaseName = "MonthlyBillingWithNewDirectDebit" },
				new CaseModel { IsMonthlyBilling = false, SelectedPlanFreeDay = null, SetupDirectDebit = false, CaseName = "BiMonthlyBilling" },
				new CaseModel { IsMonthlyBilling = false, SelectedPlanFreeDay = null, SetupDirectDebit = true, CaseName = "BiMonthlyBillingWithNewDirectDebit" },
				new CaseModel { IsMonthlyBilling = true, SelectedPlanFreeDay = DayOfWeek.Saturday, SetupDirectDebit = false, CaseName = "MonthlyBillingFreeDaySaturday" },
				new CaseModel { IsMonthlyBilling = true, SelectedPlanFreeDay = DayOfWeek.Sunday, SetupDirectDebit = false, CaseName = "MonthlyBillingFreeDaySunday" },
			};

			foreach (var caseItem in cases)
			{
				yield return new TestCaseData(caseItem)
					.SetName(caseItem.CaseName);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task ActivateSmartMeterCommandCanExecuteMethods(CaseModel caseModel)
		{
			IFixture Fixture = new Fixture().CustomizeDomainTypeBuilders();
			var mprn = Fixture.Create<ElectricityPointReferenceNumber>();
			var electricityAccountNumber = Context.Fixture.Create<long>().ToString();
			var smartPlan = Fixture.Create<SmartPlan>();
			Random r = new Random();
			var monthlyBillingSelectedDay = r.Next(1, 28);

			List<SetUpDirectDebitDomainCommand> commandsToExecute = null;
			if (caseModel.SetupDirectDebit)
			{
				commandsToExecute = new List<SetUpDirectDebitDomainCommand>();

				var newSetupDDCommand = new SetUpDirectDebitDomainCommand(electricityAccountNumber, "nameOnBankAccount", "existingIban",
							"IE15AIBK93208681900777", "businessPartner", ClientAccountType.Electricity, PaymentMethodType.DirectDebit);

				commandsToExecute.Add(newSetupDDCommand);

			}

			var cmd = ArrangeAndGetCommand(mprn,
										   electricityAccountNumber,
										   smartPlan,
										   caseModel.SelectedPlanFreeDay,
										   caseModel.IsMonthlyBilling,
										   monthlyBillingSelectedDay,
										   commandsToExecute);

			await Context.Sut.ExecuteAsync(cmd);

			Assert();

			void Assert()
			{
				var contractSalesCommand = Context.AutoMocker.GetMock<IContractSaleCommand>();
				contractSalesCommand.Verify(x =>
						x.ActivateSmartMeter(cmd), Times.Once);
				contractSalesCommand.VerifyNoOtherCalls();
			}
		}

		ActivateSmartMeterCommand ArrangeAndGetCommand(ElectricityPointReferenceNumber mprn,
														string electricityAccountNumber,
														SmartPlan selectedPlan,
														DayOfWeek? selectedPlanFreeDay,
														bool monthlyBilling,
														int monthlyBillingSelectedDay,
														IEnumerable<SetUpDirectDebitDomainCommand> commandsToExecute)
		{
			return new ActivateSmartMeterCommand(mprn,
												 electricityAccountNumber,
												 selectedPlan,
												 selectedPlanFreeDay,
												 monthlyBilling,
												 monthlyBillingSelectedDay,
												 commandsToExecute);
		}

	}
}

using System.Threading.Tasks;
using AutoFixture;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Billing.ChangePaperBillChoice
{
	internal class ChangePaperBillChoiceCommandHandlerTest : CommandHandlerTest<ChangePaperBillChoiceCommandHandler,
		ChangePaperBillChoiceCommand>
	{
		[Test]
		[Theory]
		public async Task ItChangesPaperBillOptionOfBusinessAgreementWhenNeeded(bool existingChoice, bool newChoice)
		{
			var cmd = ArrangeAndGetCommand();

			await Context.Sut.ExecuteAsync(cmd);

			Assert();

			void Assert()
			{
				var dataService = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();
				if (existingChoice == newChoice)
				{
					dataService.Verify(
						x => x.UpdateThenGet(It.IsAny<BusinessAgreementDto>(), true), Times.Never);
				}
				else
				{
					dataService.Verify(
						x => x.UpdateThenGet(It.Is<BusinessAgreementDto>(b => b.PaperBill == cmd.NewChoice), true), Times.Once);
				}
			}

			ChangePaperBillChoiceCommand ArrangeAndGetCommand()
			{
				var accountNumber = Context.Fixture.Create<string>();
				var currentChoice = existingChoice ? PaperBillChoice.On : PaperBillChoice.Off;
				var setChoice = newChoice ? PaperBillChoice.On : PaperBillChoice.Off;

				var existingBizAgreement = Context.Fixture
					.Build<BusinessAgreementDto>()
					.With(x => x.PaperBill, currentChoice)

					//performance
					.Without(x=>x.AccountAddress)
					.Without(x=>x.AlternativePayee)
					.Without(x=>x.AlternativePayerBuAg)
					.Without(x=>x.BillToAccountAddress)
					.Without(x=>x.CollectiveParent)
					.Without(x=>x.ContractItems)
					.Without(x=>x.Account)
					.Without(x=>x.AccountAddress)
					.Without(x=>x.OutgoingAlternativePayeeBankAccount)
					.Create();

				Context.CrmUmcRepoMock.Value.MockQuerySingle(existingBizAgreement);

				return new ChangePaperBillChoiceCommand(accountNumber, setChoice);
			}
		}
	}
}
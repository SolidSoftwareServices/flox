using System.Threading.Tasks;
using EI.RP.DomainServices.InternalShared.ContractSales;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using AutoFixture;
using Moq;
using NUnit.Framework;
using EI.RP.DomainServices.Commands.Contracts.ChangeSmartPlanToStandard;

namespace EI.RP.DomainServices.UnitTests.Commands.Contracts.ChangeSmartPlanToStandard
{
	internal class ChangeSmartPlanToStandardCommandHandlerTest : CommandHandlerTest<ChangeSmartPlanToStandardCommandHandler, ChangeSmartPlanToStandardCommand>
	{
		[Test]
		public async Task ChangeSmartPlanToStandardCommandExecuteMethods()
		{
			var electricityAccountNumber = Context.Fixture.Create<long>().ToString();

			var cmd = ArrangeAndGetCommand(electricityAccountNumber);
			await Context.Sut.ExecuteAsync(cmd);

			Assert(electricityAccountNumber);
		}

		void Assert(string electricityAccountNumber)
		{
			var contractSalesCommand = Context.AutoMocker.GetMock<IContractSaleCommand>();
			contractSalesCommand.Verify(x =>
					x.ExecuteChangeSmartPlanToStandardContractSale(electricityAccountNumber), Times.Once);
			contractSalesCommand.VerifyNoOtherCalls();
		}

		ChangeSmartPlanToStandardCommand ArrangeAndGetCommand(string electricityAccountNumber)
		{
			return new ChangeSmartPlanToStandardCommand(electricityAccountNumber);
		}
	}
}

using System.Collections;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;
using EI.RP.DomainServices.Validation;
using EI.RP.TestServices;
using FluentValidation;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Banking.DirectDebit.SetUpDirectDebit
{
	internal class SetUpDirectDebitCommandValidatorTests : UnitTestFixture<SetUpDirectDebitCommandValidatorTests.TestContext, SetUpDirectDebitCommandValidator>
	{
		public class TestContext : UnitTestContext<SetUpDirectDebitCommandValidator>
		{
			protected override SetUpDirectDebitCommandValidator BuildSut(AutoMocker autoMocker)
			{
				var mockReservedIbanService = new Mock<IReservedIbanService>();
				mockReservedIbanService.Setup(x => x.IsReservedIban("IE36AIBK93208681900087")).Returns(true);
				mockReservedIbanService.Setup(x => x.IsReservedIban("IE15AIBK93208681900777")).Returns(false);
				return new SetUpDirectDebitCommandValidator(mockReservedIbanService.Object);
			}
		}

		public static IEnumerable CanValidateAsyncTestCases()
		{
			var fixture = new Fixture().CustomizeDomainTypeBuilders();
			yield return new TestCaseData(new SetUpDirectDebitDomainCommand("accountNumber", "nameOnBankAccount", "existingIban",
				"IE15AIBK93208681900777", "businessPartner", ClientAccountType.Electricity, PaymentMethodType.DirectDebit), false).SetName("ValidPath");
		}

		[TestCaseSource(nameof(CanValidateAsyncTestCases))]
		public void CanValidateAsync(SetUpDirectDebitDomainCommand domainCommand, bool throws)
		{
			Task Payload()
			{
				return Context.Sut.ValidateAsync(domainCommand);
			}

			if (throws)
				Assert.ThrowsAsync<ValidationException>(Payload);
			else
				Assert.DoesNotThrowAsync(Payload);
		}
	}
}
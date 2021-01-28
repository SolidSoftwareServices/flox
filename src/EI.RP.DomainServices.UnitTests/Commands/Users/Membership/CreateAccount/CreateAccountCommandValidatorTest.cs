using System.Collections;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.Users.Membership.CreateAccount;
using EI.RP.TestServices;
using FluentValidation;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Users.Membership.CreateAccount
{
	internal class CreateAccountCommandValidatorTest : UnitTestFixture<CreateAccountCommandValidatorTest.TestContext,
		CreateAccountCommandValidator>
	{
		public class TestContext : UnitTestContext<CreateAccountCommandValidator>
		{
		}


		public static IEnumerable CanValidateAsyncTestCases()
		{
			var fixture = new Fixture().CustomizeDomainTypeBuilders();

			var cmd = fixture.Create<CreateAccountCommand>();
			
			yield return new TestCaseData(cmd, false).SetName("HappyPath");

			
		}

		[TestCaseSource(nameof(CanValidateAsyncTestCases))]
		public void CanValidateAsync(CreateAccountCommand domainCommand, bool throws)
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
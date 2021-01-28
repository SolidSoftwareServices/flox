using System.Collections;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.DataModels.Sap.CrmUmcUrm.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Commands.Users.Membership.CreateAccount;
using EI.RP.TestServices;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Users.Membership.CreateAccount
{
	internal class CreateAccountCommandHandlerTest : UnitTestFixture<CreateAccountCommandHandlerTest.TestContext,
		CreateAccountCommandHandler>
	{
		public class TestContext : UnitTestContext<CreateAccountCommandHandler>
		{
		}

		public static IEnumerable CanExecuteTestCases()
		{
			UserRequestDto BuildExpectedRequest(CreateAccountCommand commandData)
			{
				var result = new UserRequestDto();
				result.ID = string.Empty;
				result.BusinessAgreementID = commandData.AccountNumber.ToString();
				result.EmailAddress = commandData.UserEmail;
				result.PhoneNumber = commandData.ContactPhoneNumber;
				result.UsrCategory = UserCategory.SignUpUser;
				result.PoD = commandData.MPRNGPRNLast6DigitsOf;

				result.AccountOwnerFlag = commandData.AccountOwnerFlag;
				result.TermsConditionsFlag = commandData.TermsConditionsFlag;
				result.Birthday = commandData.Birthday;
				result.AccountOwnerFlag = commandData.AccountOwnerFlag;
				result.TermsConditionsFlag = commandData.TermsConditionsFlag;


				//as it was in the previous system
				var emailStr = commandData.UserEmail.ToString();
				if (emailStr.Length > 40)
				{
					result.UserName = emailStr.Substring(0, 40);
					if (emailStr.EndsWith(".")) result.UserName = emailStr.Substring(0, 39);
				}
				else
				{
					result.UserName = commandData.UserEmail;
				}

				return result;
			}

			var fixture = new Fixture().CustomizeDomainTypeBuilders();

			var cmd = fixture.Create<CreateAccountCommand>();
			yield return new TestCaseData(cmd, BuildExpectedRequest(cmd)).SetName("HappyPath");
		}


		[TestCaseSource(nameof(CanExecuteTestCases))]
		public async Task CanExecute(CreateAccountCommand domainCommand, UserRequestDto expectedRequest)
		{
			if (domainCommand != null)
			{
				await Context.Sut.ExecuteAsync(domainCommand);

				Context.AutoMocker.Verify<ISapRepositoryOfCrmUmcUrm>(x => x.Add(expectedRequest, true), Times.Once);
			}
		}
	}
}
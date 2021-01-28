using System;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Membership;
using EI.RP.DomainServices.Commands.Users.Membership.CreatePassword;
using EI.RP.DomainServices.Queries.Membership.CreatePasswordRequestResults;
using EI.RP.TestServices.SpecimenGeneration;
using EI.RP.UI.TestServices.Http;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Tests
{
	[TestFixture]
	class UserRegistrationTests : WebAppPageTests<RegistrationCreatePasswordPage>
	{

		private readonly IFixture Fixture = new Fixture().CustomizeFrameworkBuilders();
		private const string ValidPassword = "Test1123";
		protected override async Task TestScenarioArrangement()
		{
		}

		[Test]
		public async Task CanRegisterUser()
		{
			var requestId = Fixture.Create<string>();
			var activationKey = Fixture.Create<string>();

			var dateOfBirth = DateTime.Today.Subtract(TimeSpan.FromDays(5000));
			SetUpMock(requestId,"00", dateOfBirth);

			const string pwd = "Test1123";
			var createPasswordCommand = new CreatePasswordCommand(pwd,
				activationKey, requestId, null, pwd);

			App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(createPasswordCommand);
			var passwordPage = (await App.ActivateRegistration(requestId,activationKey)).CurrentPageAs<RegistrationCreatePasswordPage>();
			passwordPage.Password1.Value = passwordPage.Password2.Value = pwd;
			passwordPage.DayOfBirth.Value = dateOfBirth.Day.ToString();
			passwordPage.MonthOfBirth.Value = dateOfBirth.Month.ToString();
			passwordPage.YearOfBirth.Value = dateOfBirth.Year.ToString();
			(await passwordPage.ClickOnSubmitButton()).CurrentPageAs<LoginPage>();

			
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(createPasswordCommand);
		}

		[Test]
		public async Task CanSeePasswordValidationError()
		{
			var requestId = Fixture.Create<string>();
			var activationKey = Fixture.Create<string>();

			var dateOfBirth = DateTime.Today.Subtract(TimeSpan.FromDays(5000));
			SetUpMock(requestId,"00", dateOfBirth);

			var dummyPassword = Fixture.Create<string>();
			var passwordPage = (await App.ActivateRegistration(requestId, activationKey)).CurrentPageAs<RegistrationCreatePasswordPage>();
			passwordPage.Password1.Value = passwordPage.Password2.Value = dummyPassword.Substring(0, Math.Min(7, dummyPassword.Length));
			passwordPage.DayOfBirth.Value = dateOfBirth.Day.ToString();
			passwordPage.MonthOfBirth.Value = dateOfBirth.Month.ToString();
			passwordPage.YearOfBirth.Value = dateOfBirth.Year.ToString();
			var page = (await passwordPage.ClickOnSubmitButton()).CurrentPageAs<RegistrationCreatePasswordPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreatePasswordCommand>();
			Assert.AreEqual("Please enter a valid password", page.PasswordError?.TextContent);

			passwordPage.Password1.Value = $"{ValidPassword}{ValidPassword}";
			passwordPage.Password2.Value = ValidPassword;
			page = (await passwordPage.ClickOnSubmitButton()).CurrentPageAs<RegistrationCreatePasswordPage>();
			Assert.AreEqual("The passwords need to match", page.ConfirmPasswordError?.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreatePasswordCommand>();
		}

		[Test]
		public async Task CanSeeEmptyDateOfBirthValidationError()
		{
			var requestId = Fixture.Create<string>();
			var activationKey = Fixture.Create<string>();

			var dateOfBirth = DateTime.Today.Subtract(TimeSpan.FromDays(5000));
			SetUpMock(requestId,"00", dateOfBirth);

			var passwordPage = (await App.ActivateRegistration(requestId, activationKey)).CurrentPageAs<RegistrationCreatePasswordPage>();
			var page = (await passwordPage.ClickOnSubmitButton()).CurrentPageAs<RegistrationCreatePasswordPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreatePasswordCommand>();
			Assert.AreEqual("Please enter a valid date", page.DateOfBirthError?.TextContent);
		}

		[Test]
		public async Task CanSeeDateOfBirthValidationErrorForInvalidDate()
		{
			var requestId = Fixture.Create<string>();
			var activationKey = Fixture.Create<string>();

			var dateOfBirth = DateTime.Today.Subtract(TimeSpan.FromDays(5000));
			SetUpMock(requestId,"00", dateOfBirth);

			var passwordPage = (await App.ActivateRegistration(requestId, activationKey)).CurrentPageAs<RegistrationCreatePasswordPage>();
			passwordPage.Password1.Value = passwordPage.Password2.Value = ValidPassword;
			passwordPage.DayOfBirth.Value = dateOfBirth.AddDays(1).Day.ToString();
			passwordPage.MonthOfBirth.Value = dateOfBirth.Month.ToString();
			passwordPage.YearOfBirth.Value = dateOfBirth.AddYears(1).Year.ToString();
			var page = (await passwordPage.ClickOnSubmitButton()).CurrentPageAs<RegistrationCreatePasswordPage>();
			Assert.AreEqual("Date of birth does not match", page.DateOfBirthError?.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreatePasswordCommand>();
		}

		[Test]
		public async Task CanSeeInvalidDateOfBirthValidationError()
		{
			var requestId = Fixture.Create<string>();
			var activationKey = Fixture.Create<string>();

			var dateOfBirth = DateTime.Today.Subtract(TimeSpan.FromDays(5000));
			SetUpMock(requestId,"00", dateOfBirth);

			var passwordPage = (await App.ActivateRegistration(requestId, activationKey)).CurrentPageAs<RegistrationCreatePasswordPage>();
			passwordPage.Password1.Value = passwordPage.Password2.Value = ValidPassword;
			passwordPage.DayOfBirth.Value = "30";
			passwordPage.MonthOfBirth.Value = "02";
			passwordPage.YearOfBirth.Value = "2020";
			var page = (await passwordPage.ClickOnSubmitButton()).CurrentPageAs<RegistrationCreatePasswordPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreatePasswordCommand>();
			Assert.AreEqual("Please enter a valid date", page.DateOfBirthError?.TextContent);
		}

		[Test]
		public async Task CanRegisterUser_When_RegistrationLinkExpired()
		{
			var requestId = Fixture.Create<string>();
			var activationKey = Fixture.Create<string>();
			var dateOfBirth = DateTime.Today.Subtract(TimeSpan.FromDays(5000));
			SetUpMock(requestId, "??", dateOfBirth);

			(await App.ActivateRegistration(requestId, activationKey)).CurrentPageAs<RegistrationLinkExpiredPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreatePasswordCommand>();
		}

		private void SetUpMock(string requestId, string statusCode, DateTime dateOfBirth)
		{
			App.DomainFacade
				.QueryResolver
				.ExpectQuery(new CreatePasswordRequestResultQuery
				{
					RequestId = requestId
				}, new CreatePasswordRequestResult()
				{
					DateOfBirth = dateOfBirth,

					StatusCode = statusCode
				}.ToOneItemArray());
		}
	}
}
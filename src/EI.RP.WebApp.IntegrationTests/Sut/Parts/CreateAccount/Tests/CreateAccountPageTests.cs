using System;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Users.Membership.CreateAccount;
using EI.RP.TestServices.SpecimenGeneration;
using EI.RP.UI.TestServices.Http;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Tests
{
	[TestFixture]
	class CreateAccountPageTests : WebAppPageTests<CreateAccountPage>
	{

		protected override async Task TestScenarioArrangement()
		{

			var loginPage = (await App.ToLoginPage("")).CurrentPageAs <LoginPage>();
			Sut = (await loginPage.ClickOnCreateAccountLink()).CurrentPageAs<CreateAccountPage>();
		}

		[Test]
		public async Task CanCreateAccount()
		{
			var fixture = new Fixture().CustomizeFrameworkBuilders();

			Sut.AccountNumber.Value = DateTime.UtcNow.Ticks.ToString().Substring(0,9);
			Sut.Mprn.Value = DateTime.UtcNow.Ticks.ToString().Substring(0, 6);
			Sut.Email.Value = fixture.Create<EmailAddress>().ToString();
			Sut.PhoneNumber.Value = "987987987";
			Sut.DayOfBirth.Value = "1";
			Sut.MonthOfBirth.Value = "10";
			Sut.YearOfBirth.Value = "2000";
			Sut.CheckBoxAccountOwner.IsChecked = true;
			Sut.CheckBoxAcceptTermsConditions.IsChecked = true;

			var expectedCommand = new CreateAccountCommand(Sut.AccountNumber.Value,
				Sut.Email.Value,
				Sut.Mprn.Value.Trim(),
				Sut.PhoneNumber.Value.Trim(),
				DateTime.Parse($"{Sut.DayOfBirth.Value}/{Sut.MonthOfBirth.Value}/{Sut.YearOfBirth.Value}"),
				Sut.CheckBoxAccountOwner.IsChecked,
				Sut.CheckBoxAcceptTermsConditions.IsChecked);

			(await Sut.ClickOnCreateAccount()).CurrentPageAs<CreateAccountLinkSentPage>();

			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(expectedCommand);
		}

		[Test]
		public async Task CanSeeDateOfBirthError()
		{
			var fixture = new Fixture().CustomizeFrameworkBuilders();

			Sut.AccountNumber.Value = DateTime.UtcNow.Ticks.ToString().Substring(0,9);
			Sut.Mprn.Value = DateTime.UtcNow.Ticks.ToString().Substring(0, 6);
			Sut.Email.Value = fixture.Create<EmailAddress>().ToString();
			Sut.PhoneNumber.Value = "987987987";
			Sut.DayOfBirth.Value = "31";
			Sut.MonthOfBirth.Value = "12";
			Sut.YearOfBirth.Value = "1899";
			Sut.CheckBoxAccountOwner.IsChecked = true;
			Sut.CheckBoxAcceptTermsConditions.IsChecked = true;
			
			var page = (await Sut.ClickOnCreateAccount()).CurrentPageAs<CreateAccountPage>();
			Assert.IsTrue(page.DateOfBirthError.IsVisibleError());
			Assert.AreEqual("Please enter a valid date of birth", page.DateOfBirthError?.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreateAccountCommand>();

			Sut.DayOfBirth.Value = "1";
			Sut.MonthOfBirth.Value = "1";
			Sut.YearOfBirth.Value = DateTime.Today.Year.ToString();
			page = (await Sut.ClickOnCreateAccount()).CurrentPageAs<CreateAccountPage>();
			Assert.IsTrue(page.DateOfBirthError.IsVisibleError());
			Assert.AreEqual("Please enter a valid date of birth", page.DateOfBirthError?.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreateAccountCommand>();
		}

		[Test]
		public async Task CanSeeMPRNGPRNError()
		{
			var fixture = new Fixture().CustomizeFrameworkBuilders();

			Sut.AccountNumber.Value = DateTime.UtcNow.Ticks.ToString().Substring(0, 9);
			Sut.Mprn.Value = "abcdeg";
			Sut.Email.Value = fixture.Create<EmailAddress>().ToString();
			Sut.PhoneNumber.Value = "987987987";
			Sut.DayOfBirth.Value = "31";
			Sut.MonthOfBirth.Value = "12";
			Sut.YearOfBirth.Value = "1899";
			Sut.CheckBoxAccountOwner.IsChecked = true;
			Sut.CheckBoxAcceptTermsConditions.IsChecked = true;

			var page = (await Sut.ClickOnCreateAccount()).CurrentPageAs<CreateAccountPage>();
			Assert.IsTrue(page.MprnError.IsVisibleError());
			Assert.AreEqual("Please enter a valid MPRN or GPRN", page.MprnError?.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreateAccountCommand>();

			Sut.Mprn.Value = "12345";
			page = (await Sut.ClickOnCreateAccount()).CurrentPageAs<CreateAccountPage>();
			Assert.IsTrue(page.MprnError.IsVisibleError());
			Assert.AreEqual("Please enter a valid MPRN or GPRN", page.MprnError?.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreateAccountCommand>();

			Sut.Mprn.Value = "";
			page = (await Sut.ClickOnCreateAccount()).CurrentPageAs<CreateAccountPage>();
			Assert.IsTrue(page.MprnError.IsVisibleError());
			Assert.AreEqual("Please enter a valid MPRN or GPRN", page.MprnError?.TextContent);
			App.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted<CreateAccountCommand>();
		}

		[Ignore("Review js support")]
		[Test]
		public async Task CanSeeValidationErrors()
		{
			Assert.IsFalse(Sut.AccountNumberError.IsVisibleError());
			Sut.AccountNumber.DoFocus();

			Assert.IsFalse(Sut.MprnError.IsVisibleError());
			Sut.Mprn.DoFocus();
			Assert.IsTrue(Sut.AccountNumberError.IsVisibleError());

			Assert.IsFalse(Sut.EmailError.IsVisibleError());
			Sut.Email.DoFocus();
			Assert.IsTrue(Sut.MprnError.IsVisibleError());

			Assert.IsFalse(Sut.PhoneNumberError.IsVisibleError());
			Sut.PhoneNumber.DoFocus();
			Assert.IsTrue(Sut.EmailError.IsVisibleError());

			Assert.IsTrue(Sut.PhoneNumberError.IsVisibleError());

			Assert.IsFalse(Sut.DateOfBirthError.IsVisibleError());
			Sut.DayOfBirth.DoFocus();
			Sut.MonthOfBirth.DoFocus();
			Sut.YearOfBirth.DoFocus();
			Assert.IsTrue(Sut.DateOfBirthError.IsVisibleError());
		}

		[Ignore("TODO")]
		[Test]
		public async Task CanSeeServerErrors()
		{
			throw new NotImplementedException();

		}

		[Test]
		public async Task CanSeeComponents()
		{
			Assert.IsTrue(Sut.AccountNumber != null);
			Assert.IsTrue(Sut.AccountNumber.GetAttribute("inputmode").Contains("numeric"));

			Assert.IsTrue(Sut.Mprn != null);
			Assert.IsTrue(Sut.Mprn.GetAttribute("inputmode").Contains("numeric"));

			Assert.IsTrue(Sut.Email != null);
			Assert.IsTrue(Sut.PhoneNumber != null);

			Assert.IsTrue(Sut.DayOfBirth != null);
			Assert.IsTrue(Sut.DayOfBirth.GetAttribute("inputmode").Contains("numeric"));

			Assert.IsTrue(Sut.MonthOfBirth != null);
			Assert.IsTrue(Sut.MonthOfBirth.GetAttribute("inputmode").Contains("numeric"));

			Assert.IsTrue(Sut.YearOfBirth != null);
			Assert.IsTrue(Sut.YearOfBirth.GetAttribute("inputmode").Contains("numeric"));

			Assert.IsTrue(Sut.CheckBoxAccountOwner != null);
			Assert.IsTrue(Sut.CheckBoxAcceptTermsConditions != null);
		}

	}
}
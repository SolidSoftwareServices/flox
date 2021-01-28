using AngleSharp.Html.Dom;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Commands.Users.CompetitionEntry;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.CompetitionEntry;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CompetitionEntry
{
	[TestFixture]
	internal class WhenInPageCompetitionEntryTest : MyAccountCommonTests<CompetitionEntryPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount()
				.WithInvoices(3);
			UserConfig.Execute();

			var app = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToCompetitionEntry();
			Sut = app.CurrentPageAs<CompetitionEntryPage>();
		}

		[Test]
		public async Task CanSeeComponentItems()
		{
			Assert.NotNull(Sut.CompetitionEntryImage);
			Assert.NotNull(Sut.CompetitionName);
			Assert.NotNull(Sut.CompetitionHeading);
			Assert.NotNull(Sut.CompetitionDescription);
			Assert.NotNull(Sut.CompetitionDescription1);
			Assert.NotNull(Sut.CompetitionDescription2);
			Assert.NotNull(Sut.CompetitionQuestion);
			Assert.NotNull(Sut.Answer1CheckBox);
			Assert.NotNull(Sut.CompetitionAnswer1);
			Assert.NotNull(Sut.Answer2CheckBox);
			Assert.NotNull(Sut.CompetitionAnswer2);
			Assert.NotNull(Sut.Answer3CheckBox);
			Assert.NotNull(Sut.CompetitionAnswer3);
			Assert.NotNull(Sut.CompetitionAnswerValidationError);
			Assert.IsEmpty(Sut.CompetitionAnswerValidationError.TextContent);
			Assert.NotNull(Sut.CompetitionContactConsentCheckbox);
			Assert.NotNull(Sut.CompetitionContactConsentValidationError);
			Assert.IsEmpty(Sut.CompetitionContactConsentValidationError.TextContent);
			Assert.IsTrue(Sut.CompetitionContactConsent?.TextContent.Contains("Yes, I would like to opt-in to email marketing communications to enter the Electric Ireland customer draw *"));
			Assert.IsTrue(Sut.SubmitCompetitionEntryButton?.TextContent.Contains("Enter"));
		}

		[Test]
		public async Task ShowErrorMessageWhenSubmittingWithoutInput()
		{
			var confirmationPage = await App.ClickOnElement(Sut.SubmitCompetitionEntryButton);
			var pageWithError = confirmationPage.CurrentPageAs<CompetitionEntryPage>();
			Assert.IsTrue(pageWithError.CompetitionAnswerValidationError?.TextContent.Contains("You need to select your answer before you can enter"));
			Assert.IsTrue(pageWithError.CompetitionContactConsentValidationError?.TextContent.Contains("You must opt-in to email marketing communications to enter the Electric Ireland customer draw."));

			pageWithError.Answer1CheckBox.IsChecked = true;
			pageWithError.CompetitionContactConsentCheckbox.IsChecked = false;
			confirmationPage = await App.ClickOnElement(pageWithError.SubmitCompetitionEntryButton);
			pageWithError = confirmationPage.CurrentPageAs<CompetitionEntryPage>();
			Assert.IsEmpty(pageWithError.CompetitionAnswerValidationError.TextContent);
			Assert.IsTrue(pageWithError.CompetitionContactConsentValidationError?.TextContent.Contains("You must opt-in to email marketing communications to enter the Electric Ireland customer draw."));

			pageWithError.Answer1CheckBox.IsChecked = true;
			pageWithError.CompetitionContactConsentCheckbox.IsChecked = false;
			confirmationPage = await App.ClickOnElement(pageWithError.SubmitCompetitionEntryButton);
			pageWithError = confirmationPage.CurrentPageAs<CompetitionEntryPage>();
			Assert.IsEmpty(pageWithError.CompetitionAnswerValidationError.TextContent);
			Assert.IsTrue(pageWithError.CompetitionContactConsentValidationError?.TextContent.Contains("You must opt-in to email marketing communications to enter the Electric Ireland customer draw."));

			pageWithError.Answer1CheckBox.IsChecked = false;
			pageWithError.CompetitionContactConsentCheckbox.IsChecked = false;
			confirmationPage = await App.ClickOnElement(pageWithError.SubmitCompetitionEntryButton);
			pageWithError = confirmationPage.CurrentPageAs<CompetitionEntryPage>();
			Assert.IsTrue(pageWithError.CompetitionAnswerValidationError?.TextContent.Contains("You need to select your answer before you can enter"));
			Assert.IsTrue(pageWithError.CompetitionContactConsentValidationError?.TextContent.Contains("You must opt-in to email marketing communications to enter the Electric Ireland customer draw."));
		}

		[Test]
		public async Task CanSubmitAnswer()
		{
			Sut.Answer1CheckBox.IsChecked = true;
			Sut.CompetitionContactConsentCheckbox.IsChecked = true;
			var account = UserConfig.ElectricityAndGasAccountConfigurators.Single();
			var cmd = BuildCommand(DateTime.Now.Subtract(TimeSpan.FromMinutes(600)));

			App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd,
				commandComparer: CommandComparerToFiddleWithTheSetDateTime);
			var confirmationPage = await App.ClickOnElement(Sut.SubmitCompetitionEntryButton);
			var page = confirmationPage.CurrentPageAs<CompetitionEntrySubmittedPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

			Assert.NotNull(page.CompetitionEntryImage);
			Assert.NotNull(page.CompetitionEntrySuccessMessage);
			Assert.NotNull(page.CompetitionEntrySuccessBackToAccounts);


			CompetitionEntryCommand BuildCommand(DateTime dateTime)
			{
				return new CompetitionEntryCommand(UserConfig.UserName,
					account.Model.AccountNumber,
					account.Model.FirstName,
					account.Model.LastName,
					account.UserContactDetails.ContactEMail,
					account.UserContactDetails.PrimaryPhoneNumber,
					Sut.CompetitionName.Value,
					dateTime,
					Sut.Answer1CheckBox.Value,
					account.Model.Partner,
					false,
					"255.255.255.255"
				);
			}
			bool CommandComparerToFiddleWithTheSetDateTime(CompetitionEntryCommand expected, CompetitionEntryCommand actual)
			{
				if (expected == null) return false;
				if (expected.Equals(actual)) return true;

				var withDateTimeCheat = BuildCommand(actual.CompetitionEntryDate);
				if (withDateTimeCheat.Equals(actual))
				{
					//update command for the assertion
					cmd = withDateTimeCheat;
					return true;
				}

				return false;
			}
		}

		[Test]
		public async Task ShowErrorOnTheSameFlowAfterSubmission()
		{
			var app = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var cp = app.CurrentPageAs<MyAccountElectricityAndGasPage>();
			app = await cp.ToCompetitionEntry();
			var page = app.CurrentPageAs<CompetitionEntryPage>();
			var flowUrl = page.Document.Url;

			page.Answer1CheckBox.IsChecked = true;
			page.CompetitionContactConsentCheckbox.IsChecked = true;
			var account = UserConfig.ElectricityAndGasAccountConfigurators.Single();
			var cmd = new CompetitionEntryCommand(UserConfig.UserName,
				account.Model.AccountNumber,
				account.Model.FirstName,
				account.Model.LastName,
				account.UserContactDetails.ContactEMail,
				account.UserContactDetails.PrimaryPhoneNumber,
				page.CompetitionName.Value,
				DateTime.Now,
				page.Answer1CheckBox.Value,
				account.Model.Partner,
				false,
				"255.255.255.255"
			);

			App.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);
			var responseOfConfirmationPage = await App.ClickOnElement(page.SubmitCompetitionEntryButton);
			var confirmationPage = responseOfConfirmationPage.CurrentPageAs<CompetitionEntrySubmittedPage>();
			App.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);


			var existingParticipationEntry = new Ei.Rp.DomainModels.Competitions.CompetitionEntry
			{
				Answer = "PreviousAnswer"
			};
			App.DomainFacade.QueryResolver.Current.Setup(x =>
					x.FetchAsync<CompetitionQuery, Ei.Rp.DomainModels.Competitions.CompetitionEntry>(It.IsAny<CompetitionQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(existingParticipationEntry.ToOneItemArray().AsEnumerable()));

			var test = await App.ToUrl(flowUrl);
			page = test.CurrentPageAs<CompetitionEntryPage>();

			Assert.IsTrue(page.CompetitionEntrySuccessMessage.TextContent.Contains("It looks like you've already entered this competition. Don't worry, you're in the mix to win!"));

			Assert.NotNull(page.CompetitionEntryImage);
			Assert.NotNull(page.CompetitionHeading);
			Assert.NotNull(page.CompetitionDescription);
			Assert.NotNull(page.CompetitionDescription1);
			Assert.NotNull(page.CompetitionDescription2);
			Assert.NotNull(page.CompetitionEntrySuccessBackToAccounts);
		}

		[Test]
		public async Task CanSeeAllowedMenuItemsInCompetitionPage()
		{
			Assert.NotNull(Sut.ChangePasswordProfileMenuItem);
			Assert.NotNull(Sut.LogoutProfileMenuItem);
		}

		[Test]
		public async Task CannotSeeForbiddenMenuItemsInCompetitionPage()
		{
			Assert.Null(Sut.MyDetailsProfileMenuItem);
			Assert.Null(Sut.MarketingProfileMenuItem);
		}
	}
}

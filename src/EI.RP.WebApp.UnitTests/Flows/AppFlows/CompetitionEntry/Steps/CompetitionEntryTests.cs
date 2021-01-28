using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Commands.Users.CompetitionEntry;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.CompetitionEntry.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.CompetitionEntry.Infrastructure.StepsDataBuilder;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Http;
using Microsoft.AspNetCore.Http;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.CompetitionEntry.Steps
{
	[TestFixture]
	public class CompetitionEntryTests
	{
		private static IFixture Fixture = new Fixture();
		private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.CompetitionEntry.Steps.CompetitionEntry, ResidentialPortalFlowType> NewScreenTestConfigurator(
			bool anonymousUser = false)
		{
			return NewScreenTestConfigurator(anonymousUser
				? new DomainFacade()
				: new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
		}

		private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.CompetitionEntry.Steps.CompetitionEntry, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<WebApp.Flows.AppFlows.CompetitionEntry.Steps.CompetitionEntry, ResidentialPortalFlowType>(
					domainFacade.ModelsBuilder)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		[Test]
		public void FlowTypeIsCorrect()
		{
			Assert.AreEqual(ResidentialPortalFlowType.CompetitionEntry, NewScreenTestConfigurator().Adapter.GetFlowType());
		}

		private static AppUserConfigurator ConfigureDomain(DomainFacade domainFacade = null)
		{
			var cfg = new AppUserConfigurator(domainFacade ?? new DomainFacade())
				.SetValidSession();
			cfg.AddElectricityAccount(configureDefaultDevice: false);

			cfg.DomainFacade.QueryResolver.Current.Setup(x => x.FetchAsync<CompetitionQuery, Ei.Rp.DomainModels.Competitions.CompetitionEntry>(It.IsAny<CompetitionQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(Enumerable.Empty<Ei.Rp.DomainModels.Competitions.CompetitionEntry>()));

			cfg.Execute();
			return cfg;
		}


		[Test]
		public async Task Can_Get_CorrectConfig()
		{
			var cfg = ConfigureDomain();
			var rootDataBuilder = new RootDataBuilder(cfg);
			
			var competitionName = Fixture.Create<string>();
			var competitionHeading = Fixture.Create<string>();
			var competitionDescription = Fixture.Create<string>();
			var competitionDescription1 = Fixture.Create<string>();
			var competitionDescription2 = Fixture.Create<string>();
			var competitionDescription3 = Fixture.Create<string>();
			var competitionQuestionPart1 = Fixture.Create<string>();
			var competitionQuestionPart2 = Fixture.Create<string>();
			var competitionQuestionPart3 = Fixture.Create<string>();
			var competitionAnswerA = Fixture.Create<string>();
			var competitionAnswerB = Fixture.Create<string>();
			var competitionAnswerC = Fixture.Create<string>();
			var competitionTermAndConditionsUrl = Fixture.Create<string>();
			var competitionPageImage = new ImagesSetting
			{
				RegularImagePath = Fixture.Create<string>(),
				MobileImagePath = Fixture.Create<string>(),
				AltText = Fixture.Create<string>()
			};


			NewScreenTestConfigurator(cfg.DomainFacade)
				.WithMockConfiguration<IUiAppSettings>(c =>
				{
					c.Setup(s => s.CompetitionName).Returns(competitionName);
					c.Setup(s => s.CompetitionHeading).Returns(competitionHeading);
					c.Setup(s => s.CompetitionDescription).Returns(competitionDescription);
					c.Setup(s => s.CompetitionDescription1).Returns(competitionDescription1);
					c.Setup(s => s.CompetitionDescription2).Returns(competitionDescription2);
					c.Setup(s => s.CompetitionDescription3).Returns(competitionDescription3);
					c.Setup(s => s.CompetitionQuestionPart1).Returns(competitionQuestionPart1);
					c.Setup(s => s.CompetitionQuestionPart2).Returns(competitionQuestionPart2);
					c.Setup(s => s.CompetitionQuestionPart3).Returns(competitionQuestionPart3);
					c.Setup(s => s.CompetitionAnswerA).Returns(competitionAnswerA);
					c.Setup(s => s.CompetitionAnswerB).Returns(competitionAnswerB);
					c.Setup(s => s.CompetitionAnswerC).Returns(competitionAnswerC);
					c.Setup(s => s.CompetitionTermAndConditionsUrl).Returns(competitionTermAndConditionsUrl);
					c.Setup(s => s.CompetitionPageImages).Returns(competitionPageImage);
				})
				.NewTestCreateStepDataRunner()
				.WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
				.WhenCreated()
				.ThenTheStepDataIs<WebApp.Flows.AppFlows.CompetitionEntry.Steps.CompetitionEntry.ScreenModel>(actual =>
				{
					Assert.AreEqual(competitionName, actual.Name);
					Assert.AreEqual(competitionHeading, actual.Heading);
					Assert.AreEqual(competitionDescription, actual.Description);
					Assert.AreEqual(competitionDescription1, actual.Description1);
					Assert.AreEqual(competitionDescription2, actual.Description2);
					Assert.AreEqual(competitionDescription3, actual.Description3);
					Assert.AreEqual(competitionQuestionPart1, actual.QuestionPart1);
					Assert.AreEqual(competitionQuestionPart2, actual.QuestionPart2);
					Assert.AreEqual(competitionQuestionPart3, actual.QuestionPart3);
					Assert.AreEqual(competitionAnswerA, actual.AnswerA);
					Assert.AreEqual(competitionAnswerB, actual.AnswerB);
					Assert.AreEqual(competitionAnswerC, actual.AnswerC);
					Assert.AreEqual(competitionTermAndConditionsUrl, actual.TermAndConditionsUrl);
					Assert.AreEqual(competitionPageImage.RegularImagePath, actual.ImageDesktop);
					Assert.AreEqual(competitionPageImage.MobileImagePath, actual.ImageMobile);
					Assert.AreEqual(competitionPageImage.AltText, actual.ImageAltText);
				});
		}

		[Test]
		public void ItCanSubmitAnswer()
		{
			var cfg = ConfigureDomain();
			var stepDataBuilder = new CompetitionEntryDataBuilder(cfg);
			var rootDataBuilder = new RootDataBuilder(cfg);

			var stepData = stepDataBuilder.Create();
			var ip = Fixture.Create<string>();

			NewScreenTestConfigurator(cfg.DomainFacade)
				.WithMockConfiguration<IClientInfoResolver>(c => c.Setup(_ => _.ResolveIp()).Returns(ip))
				.NewEventTestRunner(stepData)
				.WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
				//act
				.WhenEvent(WebApp.Flows.AppFlows.CompetitionEntry.Steps.CompetitionEntry.StepEvent.SubmitCompetitionEntry)
				//flow step assertions 
				.ThenTheResultStepIs(CompetitionEntryStep.CompetitionEntrySuccessful);

			var account = cfg.ElectricityAndGasAccountConfigurators.Single();
			var command = new CompetitionEntryCommand(cfg.UserName,
					account.Model.AccountNumber,
					account.Model.FirstName,
					account.Model.LastName,
					account.UserContactDetails.ContactEMail,
					account.UserContactDetails.PrimaryPhoneNumber,
					stepData.Name,
					DateTime.Now,
					stepData.Answer,
					account.Model.Partner,
					false,
					ip);
			cfg.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(command);
		}

		[Test]
		public void DontExecuteCommandWhenEntryExist()
		{
			var cfg = ConfigureDomain();

			var stepDataBuilder = new CompetitionEntryDataBuilder(cfg);
			var rootDataBuilder = new RootDataBuilder(cfg);

			var stepData = stepDataBuilder.Create();
			stepData.HasExistingEntry = true;


			var ip = Fixture.Create<string>();


			NewScreenTestConfigurator(cfg.DomainFacade)
				.WithMockConfiguration<IClientInfoResolver>(c => c.Setup(_ => _.ResolveIp()).Returns(ip))
				.NewEventTestRunner(stepData)
				.WithExistingStepData(ScreenName.PreStart, rootDataBuilder.Create())
				//act
				.WhenEvent(WebApp.Flows.AppFlows.CompetitionEntry.Steps.CompetitionEntry.StepEvent.SubmitCompetitionEntry)
				//flow step assertions 
				.ThenTheResultStepIs(CompetitionEntryStep.CompetitionEntrySuccessful);

			var account = cfg.ElectricityAndGasAccountConfigurators.Single();
			var command = new CompetitionEntryCommand(cfg.UserName,
					account.Model.AccountNumber,
					account.Model.FirstName,
					account.Model.LastName,
					account.UserContactDetails.ContactEMail,
					account.UserContactDetails.PrimaryPhoneNumber,
					stepData.Name,
					DateTime.Now,
					stepData.Answer,
					account.Model.Partner,
					false,
					ip
					);
			cfg.DomainFacade.CommandDispatcher.AssertCommandWasNotExecuted(command);
		}

	}
}
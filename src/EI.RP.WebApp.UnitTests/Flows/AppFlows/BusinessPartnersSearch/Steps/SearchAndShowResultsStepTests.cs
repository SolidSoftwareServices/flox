using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.FlowDefinitions;
using NUnit.Framework;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.DomainServices.Commands.BusinessPartner.Deregister;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using EI.RP.TestServices.SpecimenGeneration;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.BusinessPartnersSearch.Infrastructure.StepsDataBuilder;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.BusinessPartnersSearch.Steps
{
	[TestFixture]
	internal class SearchAndShowResultsStepTests
	{
		IFixture Fixture = new Fixture().CustomizeFrameworkBuilders();

		private FlowScreenTestConfigurator<SearchAndShowResults, ResidentialPortalFlowType> NewScreenTestConfigurator(
			bool anonymousUser = false)
		{
			return NewScreenTestConfigurator(anonymousUser
				? new DomainFacade()
				: new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
		}

		private FlowScreenTestConfigurator<SearchAndShowResults, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<SearchAndShowResults, ResidentialPortalFlowType>(
					domainFacade.ModelsBuilder)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}
		private static AppUserConfigurator ConfigureDomain()
		{
			var cfg = new AppUserConfigurator(ResidentialPortalUserRole.AgentUser)
				.SetValidSession();

			cfg.Execute();
			return cfg;
		}

		private SearchAndShowResults.ScreenModel GetEmptyScreenModel()
		{
			return new SearchAndShowResults.ScreenModel
			{
				PartnerNum = string.Empty,
				City = string.Empty,
				HouseNum = string.Empty,
				LastName = string.Empty,
				Street = string.Empty,
				UserName = string.Empty
			};
		}

		private BusinessPartnerQuery GetEmptyBusinessPartnerQuery()
		{
			return new BusinessPartnerQuery
			{
				PartnerNum = string.Empty,
				UserName = string.Empty,
				HouseNum = string.Empty,
				Street = string.Empty,
				City = string.Empty,
				LastName = string.Empty
			};
		}

		[Test]
		public async Task GetStepData()
		{
			NewScreenTestConfigurator()
				.NewTestCreateStepDataRunner()
				.WhenCreated()
				.ThenTheStepDataIs<SearchAndShowResults.ScreenModel>(actual =>
				{
					Assert.AreEqual(30, actual.MaxRecords);
				});
		}

		[Test]
		public async Task SearchDataAndReturnNoResults()
		{ 
			NewScreenTestConfigurator()
				.NewEventTestRunner(new SearchAndShowResults.ScreenModel
				{
					PartnerNum = Fixture.Create<string>()
				})
				//act
				.WhenEvent(SearchAndShowResults.StepEvent.FetchBusinessPartnersRequested)

				//flow step assertions 
				.ThenTheValidationPassed()
				.ThenTheResultStepIs(BusinessPartnersSearchStep.SearchAndShowResultsStep)
				.ThenTheStepDataAfterIs<SearchAndShowResults.ScreenModel>(actual =>
				{
					Assert.AreEqual(30, actual.MaxRecords);
					Assert.AreEqual("No customers found for the search criteria entered. Use different search criteria, and try again", actual.ShowErrorMessage);
				});
		}

		[Test]
		public async Task SearchDataAndReturnResults()
		{
			var businessPartner = new BusinessPartnersDataBuilder().Create(1).First();
			var partnerNum = businessPartner.NumPartner;
			var query = GetEmptyBusinessPartnerQuery();
			query.PartnerNum = partnerNum;

			var screenModel = GetEmptyScreenModel();
			screenModel.PartnerNum = partnerNum;
			screenModel.ShowErrorMessage = Fixture.Create<string>();

			var cfg = ConfigureDomain();
			cfg.DomainFacade.QueryResolver.ExpectQuery(query, 
				new List<BusinessPartner> {businessPartner});

			NewScreenTestConfigurator(cfg.DomainFacade)
				.NewEventTestRunner(screenModel)
				//act
				.WhenEvent(SearchAndShowResults.StepEvent.FetchBusinessPartnersRequested)
				//flow step assertions 
				.ThenTheValidationPassed()
				.ThenTheResultStepIs(BusinessPartnersSearchStep.SearchAndShowResultsStep)
				.ThenTheStepDataAfterIs<SearchAndShowResults.ScreenModel>(actual =>
				{
					Assert.AreEqual(30, actual.MaxRecords);
					Assert.IsTrue(string.IsNullOrEmpty(actual.ShowErrorMessage),"There should not be an error message");
					Assert.AreEqual(1, actual.BusinessPartnerNumbers.Count());
					Assert.AreEqual(businessPartner.NumPartner, actual.BusinessPartnerNumbers.First());
				});
		}

		[Test]
		public async Task SearchDataAndReturnResultsWithMaxLimit()
		{
			var city = Fixture.Create<string>();
			int maxRecords = 15;
			
			var businessPartners = new BusinessPartnersDataBuilder().Create(20).ToList();
			var query = GetEmptyBusinessPartnerQuery();
			query.City = city;

			var screenModel = GetEmptyScreenModel();
			screenModel.City = city;
			screenModel.MaxRecords = maxRecords;

			var cfg = ConfigureDomain();
			cfg.DomainFacade.QueryResolver.ExpectQuery(query,
				businessPartners);
			
			NewScreenTestConfigurator(cfg.DomainFacade)
				.NewEventTestRunner(screenModel)
				//act
				.WhenEvent(SearchAndShowResults.StepEvent.FetchBusinessPartnersRequested)
				//flow step assertions 
				.ThenTheValidationPassed()
				.ThenTheResultStepIs(BusinessPartnersSearchStep.SearchAndShowResultsStep)
				.ThenTheStepDataAfterIs<SearchAndShowResults.ScreenModel>(actual =>
				{
					Assert.AreEqual(maxRecords, actual.MaxRecords);
					Assert.IsTrue(actual.ShowErrorMessage?.Equals($"The system found more than {maxRecords} customers for the search criteria entered. Use more restricted search criteria, and try again"));
					Assert.AreEqual(maxRecords, actual.BusinessPartnerNumbers.Count());
					CollectionAssert.AreEqual(businessPartners.Select(x=>x.NumPartner).Take(maxRecords), actual.BusinessPartnerNumbers);
				});
		}


		[Test]
		public async Task SearchDataAndDeregisterAccount()
		{
			var businessPartnerBuilder = new BusinessPartnersDataBuilder();
			var businessPartner = businessPartnerBuilder.Create(1).First();
			var partnerNum = businessPartner.NumPartner;
			var query = GetEmptyBusinessPartnerQuery();
			query.PartnerNum = partnerNum;

			var screenModel = GetEmptyScreenModel();
			screenModel.SelectedBusinessPartnerId = partnerNum;
			screenModel.PartnerNum = partnerNum;
			screenModel.BusinessPartnerNumbers = businessPartnerBuilder.Create(2).Select(x=>x.NumPartner).ToArray();

			var cfg = ConfigureDomain();
			cfg.DomainFacade.QueryResolver.ExpectQuery(query,
				new List<BusinessPartner> { businessPartner });

			var cmd = new DeRegisterBusinessPartnerCommand(partnerNum, true);
			cfg.DomainFacade.CommandDispatcher.ExpectCommandAndSuccess(cmd);

			NewScreenTestConfigurator(cfg.DomainFacade)
				.NewEventTestRunner(screenModel)
				//act
				.WhenEvent(SearchAndShowResults.StepEvent.DeRegistrationRequested)

				//flow step assertions 
				.ThenTheValidationPassed()
				.ThenTheResultStepIs(BusinessPartnersSearchStep.SearchAndShowResultsStep)
				.ThenTheStepDataAfterIs<SearchAndShowResults.ScreenModel>(actual =>
				{
					Assert.AreEqual(30, actual.MaxRecords);
					Assert.AreEqual("De-registration process has been completed successfully", actual.ShowSuccessfulMessage);
					Assert.IsNotNull(actual.BusinessPartnerNumbers);
					Assert.AreEqual(0, actual.BusinessPartnerNumbers.Count());
				});

			cfg.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);

		}
	}
}

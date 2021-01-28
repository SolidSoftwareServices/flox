using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AutoFixture;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Pages;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Tests
{
	[TestFixture]
	internal class WhenInAgentBusinessPartnerSearchTests : WebAppPageTests<AgentBusinessPartnerPage>
	{
		public WhenInAgentBusinessPartnerSearchTests() : base(ResidentialPortalDeploymentType.Internal)
		{ }

		private AgentBusinessPartnerPage _sut;
		private AppUserConfigurator _esbAgentUser;
		private AppUserConfigurator _esbBusinessPartner;
		private readonly IFixture _fixture = new Fixture().CustomizeDomainTypeBuilders();
		private string _cityNameWithInMaxLimit;
		private string _cityNameOutsideMaxLimit;
		private const int NumberOfResultsForCitySearch = 20;

		protected override async Task TestScenarioArrangement()
		{
			_esbAgentUser = App.ConfigureUser("testagent@esb.ie", "Password$1", ResidentialPortalUserRole.AgentUser);
			_esbBusinessPartner = App.ConfigureUser("testBusinessPartner@esb.ie", "Password$1",
				ResidentialPortalUserRole.OnlineUser);
			_esbBusinessPartner.AddElectricityAccount();
			_esbBusinessPartner.Execute();
			var esbBusinessPartnerAccount = _esbBusinessPartner.Accounts.First();

			ArrangeBusinessPartner();

			App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				AccountNumber = _esbBusinessPartner.Accounts.First().AccountNumber
			}, new List<AccountInfo> {esbBusinessPartnerAccount});

			App.DomainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
			{
				Opened = null
			}, new List<AccountInfo> {esbBusinessPartnerAccount});

			_cityNameWithInMaxLimit = _fixture.Create<string>();
			_cityNameOutsideMaxLimit = _fixture.Create<string>();
			ArrangeDataForPagination(_cityNameWithInMaxLimit, _cityNameOutsideMaxLimit);

			var app = (ResidentialPortalApp) await App.WithValidSessionFor(_esbAgentUser.UserName, _esbAgentUser.Role);

			await app.ToAgentBusinessPartnerSearch();
			_sut = App.CurrentPageAs<AgentBusinessPartnerPage>();

			void ArrangeBusinessPartner()
			{
				var businessPartner = new BusinessPartner
				{
					CommunicationsLevel = 0,
					Description = esbBusinessPartnerAccount.Description,
					MeterConfiguration = 0,
					NumPartner = esbBusinessPartnerAccount.AccountNumber
				};
				App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
					{
						UserName = _esbBusinessPartner.UserName,
						HouseNum = string.Empty,
						Street = string.Empty,
						City = string.Empty,
						PartnerNum = string.Empty,
						LastName = string.Empty
					},
					businessPartner.ToOneItemArray()
				);

				App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
					{
						UserName = null,
						HouseNum = null,
						Street = null,
						City = null,
						LastName = string.Empty,
						PartnerNum = businessPartner.NumPartner,
						NumberOfRows = 1
					},
					businessPartner.ToOneItemArray()
				);
			}
		}

		private void ArrangeDataForPagination(string cityNameWithInMaxLimit, string cityNameOutsideMaxLimit)
		{
			var businessPartners = Enumerable.Repeat(new BusinessPartner
			{
				CommunicationsLevel = 0,
				Description = _fixture.Create<string>(),
				MeterConfiguration = 0,
				NumPartner = App.DomainFacade.ModelsBuilder.Create<long>().ToString()
			}, NumberOfResultsForCitySearch).ToArray();
			App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
			{
				UserName = string.Empty,
				HouseNum = string.Empty,
				Street = string.Empty,
				City = cityNameWithInMaxLimit,
				PartnerNum = string.Empty,
				LastName = string.Empty
			}, businessPartners);

			foreach (var businessPartner in businessPartners)
			{
				App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
					{
						UserName = null,
						HouseNum = null,
						Street = null,
						City = null,
						LastName = string.Empty,
						PartnerNum = businessPartner.NumPartner,
						NumberOfRows = 1
					},
					businessPartner.ToOneItemArray()
				);	
			}

			businessPartners = Enumerable.Repeat(new BusinessPartner
			{
				CommunicationsLevel = 0,
				Description = _fixture.Create<string>(),
				MeterConfiguration = 0,
				NumPartner = App.DomainFacade.ModelsBuilder.Create<long>().ToString()
			}, NumberOfResultsForCitySearch * 2).ToArray();
			App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
			{
				UserName = string.Empty,
				HouseNum = string.Empty,
				Street = string.Empty,
				City = cityNameOutsideMaxLimit,
				PartnerNum = string.Empty,
				LastName = string.Empty
			}, businessPartners);

			foreach (var businessPartner in businessPartners)
			{
				App.DomainFacade.QueryResolver.ExpectQuery(new BusinessPartnerQuery
					{
						UserName = null,
						HouseNum = null,
						Street = null,
						City = null,
						LastName = string.Empty,
						PartnerNum = businessPartner.NumPartner,
						NumberOfRows = 1
					},
					businessPartner.ToOneItemArray()
				);	
			}
		}

		[Test]
		public async Task NoErrorShown()
		{
			Assert.IsNull(_sut.ErrorMessage, "Agent should not see error message");
			Assert.IsNull(_sut.Pagination, "Agent should not see pagination");
		}

		[Test]
		public async Task NoCustomerFoundErrorShown()
		{
			_sut.UserNameInput.Value = _fixture.Create<string>();
			var page = (await _sut.ClickOnElement(_sut.FindBusinessPartnersButton))
				.CurrentPageAs<AgentBusinessPartnerPage>();
			Assert.IsTrue(
				page.ErrorMessage?.Text()
					.Contains(
						"No customers found for the search criteria entered. Use different search criteria, and try again"),
				"Agent should see no customer found error message ");
			Assert.IsNull(_sut.Pagination, "Agent should not see pagination");
		}

		[Test]
		public async Task SearchSingleUser()
		{
			_sut.UserNameInput.Value = _esbBusinessPartner.UserName;
			var page = (await _sut.ClickOnElement(_sut.FindBusinessPartnersButton))
				.CurrentPageAs<AgentBusinessPartnerPage>();
			Assert.IsNull(page.ErrorMessage, "Agent should not see error message");
			Assert.IsNotNull(page.ViewBusinessPartnersButton, "Agent should see View business partner");
			Assert.IsNull(page.Pagination, "Agent should not see pagination");
		}

		[Test]
		public async Task ShowPaginationWithOutError()
		{
			_sut.CityInput.Value = _cityNameWithInMaxLimit;
			_sut.MaximumRecordsInput.Value = NumberOfResultsForCitySearch.ToString();
			var page = (await _sut.ClickOnElement(_sut.FindBusinessPartnersButton))
				.CurrentPageAs<AgentBusinessPartnerPage>();
			Assert.IsNull(page.ErrorMessage, "Agent should not see error message");
			Assert.IsNotNull(page.ViewBusinessPartnersButton, "Agent should see View business partner");
			Assert.IsNotNull(page.Pagination, "Agent should see pagination");
		}

		[Test]
		public async Task ShowPaginationWithError()
		{
			var maxRecords = _sut.MaximumRecordsInput.Value;
			_sut.CityInput.Value = _cityNameOutsideMaxLimit;
			var page = (await _sut.ClickOnElement(_sut.FindBusinessPartnersButton))
				.CurrentPageAs<AgentBusinessPartnerPage>();
			Assert.IsTrue(
				page.ErrorMessage?.Text()
					.Contains(
						$"The system found more than {maxRecords} customers for the search criteria entered. Use more restricted search criteria, and try again"),
				"Agent should see max record error message");
			Assert.IsNotNull(page.ViewBusinessPartnersButton, "Agent should see View business partner");
			Assert.IsNotNull(page.Pagination, "Agent should see pagination");
		}

		[Test]
		public async Task LogoClickIsRedirectedCorrectly()
		{
			Assert.IsTrue(_sut.LogoUrl?.Href.EndsWith("Agent/Init"),
				"Agent logo url is not set up correctly on account overview page");
		}
	}
}
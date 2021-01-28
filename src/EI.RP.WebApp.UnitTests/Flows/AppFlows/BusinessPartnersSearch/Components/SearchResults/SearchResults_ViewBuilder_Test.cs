using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.TestServices;
using EI.RP.TestServices.SpecimenGeneration;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Components.SearchResults;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.BusinessPartnersSearch.Infrastructure.StepsDataBuilder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Moq.AutoMock;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.BusinessPartnersSearch.Components.SearchResults
{
	[TestFixture]
	internal class SearchResults_ViewBuilder_Test : UnitTestFixture<SearchResults_ViewBuilder_Test.TestContext, ViewModelBuilder>
	{

		[Theory]
		public async Task Resolve_IsPaginationDone_Correctly(bool isPagingEnabled)
		{
			int totalBusinessPartners = 20;
			int pageSize = 10;
			string queryUserName = null;
			var businessPartners = new BusinessPartnersDataBuilder().Create(totalBusinessPartners).ToArray();
			
			var result = await Context
				.HasPagingEnabled(isPagingEnabled)
				.WithPageSize(pageSize)
				.WithQueryUserName(queryUserName)
				.WithBusinessPartners(businessPartners)
				.Sut
				.Resolve(Context.BuildInput());

			AssertResultData(result, isPagingEnabled, pageSize, queryUserName, totalBusinessPartners);
		}
		
		[Theory]
		public async Task Resolve_Single_Result(bool isPagingEnabled)
		{
			int totalBusinessPartners = 1;
			int pageSize = 10;
			string queryUserName = Context.Fixture.Create<string>();
			var businessPartners = new BusinessPartnersDataBuilder().Create(totalBusinessPartners).ToArray();
			
			var result = await Context
				.HasPagingEnabled(isPagingEnabled)
				.WithPageSize(pageSize)
				.WithQueryUserName(queryUserName)
				.WithBusinessPartners(businessPartners)
				.Sut
				.Resolve(Context.BuildInput());

			AssertResultData(result, isPagingEnabled, pageSize, queryUserName, totalBusinessPartners);
		}

		private static void AssertResultData(ViewModel viewModel, bool isPagingEnabled, int pageSize, string queryUserName, int totalBusinessPartners)
		{
			Assert.AreEqual(isPagingEnabled, viewModel?.IsPagingEnabled, $"Pagination should be {(isPagingEnabled?"enabled":"disabled")}");
			Assert.AreEqual(pageSize, viewModel?.Paging.PageSize, "Paging size should be same as we set");
			Assert.AreEqual(isPagingEnabled ? (int)Math.Ceiling(totalBusinessPartners / (double)pageSize) : 1, viewModel?.Paging.TotalPages, "Total pages according to paging enabled, page size and total business partners");
			Assert.AreEqual(!string.IsNullOrWhiteSpace(queryUserName), viewModel?.ShowDeRegistrationColumn,
				"Should be true if query user name is null or whitespace");
			Assert.AreEqual(totalBusinessPartners == 1, viewModel?.IsSingleUserBusinessPartner,
				"True if there is only 1 business partner ");
		}

		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			private IEnumerable<BusinessPartner> _businessPartners;
			private int _pageSize = 12;
			private int _pageIndex = 0;
			private int _numberPagingLinks = 5;
			private bool _isPagingEnabled;
			private string _queryUserName;

			public TestContext():base(new Fixture().CustomizeFrameworkBuilders())
			{
			}

			public TestContext HasPagingEnabled(bool isPagingEnabled)
			{
				_isPagingEnabled = isPagingEnabled;
				return this;
			}

			public TestContext WithPageSize(int pageSize)
			{
				_pageSize = pageSize;
				return this;
			}

			public TestContext WithPageIndex(int pageIndex)
			{
				_pageIndex = pageIndex;
				return this;
			}

			public TestContext WithBusinessPartners(IEnumerable<BusinessPartner> businessPartners)
			{
				_businessPartners = businessPartners;
				return this;
			}

			public TestContext WithNumberPagingLinks(int numberPagingLinks)
			{
				_numberPagingLinks = numberPagingLinks;
				return this;
			}

			public TestContext WithQueryUserName(string queryUserName)
			{
				_queryUserName = queryUserName;
				return this;
			}

			public InputModel BuildInput()
			{
				return new InputModel
				{
					IsPagingEnabled = _isPagingEnabled,
					NumberPagingLinks = _numberPagingLinks,
					PageIndex = _pageIndex,
					PageSize = _pageSize,
					BusinessPartnersIdToShow = _businessPartners.Select(x=>x.NumPartner).ToArray(),
					QueryUserName = _queryUserName,
				};
			}

			private readonly DomainFacade _domainFacade = new DomainFacade();
			protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
			{
				foreach (var businessPartner in _businessPartners)
				{
					var query = new BusinessPartnerQuery
					{
						City = null,
						HouseNum=null,
						
						NumberOfRows=1,
						PartnerNum = businessPartner.NumPartner,
						Street = null,
						UserName = null
						
					};
					_domainFacade.QueryResolver.ExpectQuery(query, businessPartner.ToOneItemArray());
				}


				_domainFacade.SetUpMocker(autoMocker);

				return base.BuildSut(autoMocker);
			}
		}
	}
}

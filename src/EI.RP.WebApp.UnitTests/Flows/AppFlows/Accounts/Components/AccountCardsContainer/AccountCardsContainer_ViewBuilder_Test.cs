using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System.FastReflection;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardsContainer;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Accounts.Components.AccountCardsContainer
{
	[TestFixture]
	internal class
		AccountCardsContainer_ViewBuilder_Test : UnitTestFixture<AccountCardsContainer_ViewBuilder_Test.TestContext,
			ViewModelBuilder>
	{
		[Theory]
		public void Resolve_HasStartedFromMeterReading_Correctly(bool hasStartedFromMeterReading)
		{
			var expected = hasStartedFromMeterReading;

			var actual = Context
				.WithHasStartedFromMeterReading(hasStartedFromMeterReading)
				.Sut
				.Resolve(Context.BuildInput())
				.GetAwaiter().GetResult()
				.CanSubmitMeterReading;

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_IsPagingEnabled_Correctly(bool isPagingEnabled)
		{
			var expected = isPagingEnabled;

			var resolve = await Context
				.WithIsPagingEnabled(isPagingEnabled)
				.Sut
				.Resolve(Context.BuildInput());
			var actual = resolve
				.IsPagingEnabled;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public async Task Resolve_NumberOfPageLinks_Correctly([Values(1, 10, 3)] int numberPagingLinks)
		{
			var expected = numberPagingLinks;

			var actual = (await Context
					.WithNumberPagingLinks(numberPagingLinks)
					.Sut
					.Resolve(Context.BuildInput()))

				.NumberOfPageLinks;

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_RouteValues_Correctly(bool isGas, bool isOpen)
		{
			var accountType = isGas ? ClientAccountType.Gas : ClientAccountType.EnergyService;

			var expected = new {AccountType = accountType.ToString(), IsOpen = isOpen};

			var actual = (await Context
					.WithAccountType(accountType)
					.WithIsOpen(isOpen)
					.Sut
					.Resolve(Context.BuildInput())
				)
				.RouteValues;

			Assert.AreEqual(expected.AccountType, actual.GetPropertyValueFast("AccountType"));
			Assert.AreEqual(expected.IsOpen, actual.GetPropertyValueFast("IsOpen"));
		}

		[Test]
		public async Task Resolve_ContainerId_Correctly(
			[Values("ThisIsAContainerId", "ThisIsAnotherContainerId")]
			string containerId)
		{
			var expected = containerId;

			var actual = (await Context
					.WithContainerId(containerId)
					.Sut
					.Resolve(Context.BuildInput()))
				.ContainerId;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public async Task Resolve_PaginationId_Correctly(
			[Values("ThisIsAPaginationId", "ThisIsAnotherPaginationId")]
			string paginationId)
		{
			var expected = paginationId;

			var actual = (await Context
					.WithPaginationId(paginationId)
					.Sut
					.Resolve(Context.BuildInput())
				)
				.PaginationId;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public async Task Resolve_WhenChangingPageFocusOn_Correctly(
			[Values("ThisIsAFocusId", "ThisIsAnotherFocusId")]
			string whenChangingPageFocusOn)
		{
			var expected = whenChangingPageFocusOn;

			var actual = (await Context
					.WithWhenChangingPageFocusOn(whenChangingPageFocusOn)
					.Sut
					.Resolve(Context.BuildInput())
				)
				.WhenChangingPageFocusOn;

			Assert.AreEqual(expected, actual);
		}



		[Test, Combinatorial]
		public async Task Resolve_Paging_Correctly([Values(1, 5, 3)] int pageSize, [Values(1, 2, 3)] int pageIndex,
			[Values(0, 1, 2, 6)] int numItems)
		{
			var cfg = new AppUserConfigurator(Context.DomainFacade);
			for (var i = 0; i < numItems; i++)
			{
				cfg.AddElectricityAccount();
			}

			cfg.Execute();

			var actual = (await Context
				.WithPageSize(pageSize)
				.WithPageIndex(pageIndex)
				.Sut
				.Resolve(Context.BuildInput())).Paging;


			var expectedPages = (int) Math.Ceiling(numItems / (double) pageSize);
			Assert.AreEqual(Math.Min(expectedPages, pageIndex), actual.PageIndex);
			Assert.AreEqual(expectedPages, actual.TotalPages);
			Assert.IsTrue(pageSize >= actual.CurrentPageItems.Length);

		}

		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			private ClientAccountType _accountType = ClientAccountType.Electricity;
			private bool _isOpen = true;
			private bool _hasStartedFromMeterReading;
			
			private int _pageIndex = 0;
			private int _pageSize = int.MaxValue;
			private int _numberPagingLinks = 4;
			private bool _isPagingEnabled = false;
			private string _containerId = "accounts-container";
			private string _paginationId = "accounts-pagination";
			private string _whenChangingPageFocusOn;

			public DomainFacade DomainFacade { get; } = new DomainFacade();

			public TestContext WithAccountType(ClientAccountType accountType)
			{
				_accountType = accountType;
				return this;
			}

			public TestContext WithIsOpen(bool isOpen)
			{
				_isOpen = isOpen;
				return this;
			}

			public TestContext WithHasStartedFromMeterReading(bool hasStartedFromMeterReading)
			{
				_hasStartedFromMeterReading = hasStartedFromMeterReading;
				return this;
			}

		

			public TestContext WithPageIndex(int pageIndex)
			{
				_pageIndex = pageIndex;
				return this;
			}

			public TestContext WithPageSize(int pageSize)
			{
				_pageSize = pageSize;
				return this;
			}

			public TestContext WithNumberPagingLinks(int numberPagingLinks)
			{
				_numberPagingLinks = numberPagingLinks;
				return this;
			}

			public TestContext WithIsPagingEnabled(bool isPagingEnabled)
			{
				_isPagingEnabled = isPagingEnabled;
				return this;
			}

			public TestContext WithContainerId(string containerId)
			{
				_containerId = containerId;
				return this;
			}

			public TestContext WithPaginationId(string paginationId)
			{
				_paginationId = paginationId;
				return this;
			}

			public TestContext WithWhenChangingPageFocusOn(string whenChangingPageFocusOn)
			{
				_whenChangingPageFocusOn = whenChangingPageFocusOn;
				return this;
			}

			protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
			{
				autoMocker.Use(DomainFacade.QueryResolver.Current.Object);
				return base.BuildSut(autoMocker);
			}

			public InputModel BuildInput()
			{
				return new InputModel
				{
					AccountType = _accountType,
					IsOpen = _isOpen,
					HasStartedFromMeterReading = _hasStartedFromMeterReading,
					PageIndex = _pageIndex,
					PageSize = _pageSize,
					NumberPagingLinks = _numberPagingLinks,
					IsPagingEnabled = _isPagingEnabled,
					ContainerId = _containerId,
					PaginationId = _paginationId,
					WhenChangingPageFocusOn = _whenChangingPageFocusOn
				};
			}
		}
	}
}
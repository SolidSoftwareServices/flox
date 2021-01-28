using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountsSubNavigation;
using EI.RP.WebApp.Flows.AppFlows.Accounts.FlowDefinitions;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Accounts.Components.AccountsSubNavigation
{
	[TestFixture]
	internal class
		AccountsSubNavigation_ViewBuilder_Test : UnitTestFixture<AccountsSubNavigation_ViewBuilder_Test.TestContext,
			ViewModelBuilder>
	{
		[Theory]
		public async Task Resolve_IsMultiPageView_Correctly(bool isMultiPageView)
		{
			var expected = isMultiPageView;

			var actual = (await Context
					.WithIsMultiPageView(isMultiPageView)
					.WithHasElectricityAccounts(true)
					.Sut
					.Resolve(Context.BuildInput()))
				.IsMultiPageView;

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_ElectricityNavigationItem_Correctly(bool hasElectricityAccounts)
		{
			var navigationItems = (await Context
					.WithIsMultiPageView(true)
					.WithHasElectricityAccounts(hasElectricityAccounts)
					.WithAccountType(ClientAccountType.Electricity)
					.WithIsOpen(true)
					.Sut
					.Resolve(Context.BuildInput()))
				.NavigationItems;

			var expected = hasElectricityAccounts;
			var actual = navigationItems.Any(x => x.AnchorLink?.Label == "Electricity");

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_GasNavigationItem_Correctly(bool hasGasAccounts)
		{
			var navigationItems = (await Context
					.WithIsMultiPageView(true)
					.WithHasGasAccounts(hasGasAccounts)
					.WithAccountType(ClientAccountType.Gas)
					.WithIsOpen(true)
					.Sut
					.Resolve(Context.BuildInput()))
				.NavigationItems;

			var expected = hasGasAccounts;
			var actual = navigationItems.Any(x => x.AnchorLink?.Label == "Gas");

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_EnergyServiceNavigationItem_Correctly(bool hasEnergyServiceAccounts)
		{
			var navigationItems = (await Context
					.WithIsMultiPageView(true)
					.WithHasEnergyServiceAccounts(hasEnergyServiceAccounts)
					.WithAccountType(ClientAccountType.EnergyService)
					.WithIsOpen(true)
					.Sut
					.Resolve(Context.BuildInput()))
				.NavigationItems;

			var expected = hasEnergyServiceAccounts;
			var actual = navigationItems.Any(x => x.AnchorLink?.Label == "Energy Services");

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_IsClosed_Correctly(bool isOpened)
		{
			var navigationItems = (await Context
					.WithIsMultiPageView(true)
					.WithHasElectricityAccounts(true)
					.WithAccountType(ClientAccountType.Electricity)
					.WithIsOpen(isOpened)
					.Sut
					.Resolve(Context.BuildInput()))
				.NavigationItems
				.ToArray();

			var expected = isOpened;
			var actual =
				((AccountsFlowInput) navigationItems?.First()?.AnchorLink?.FlowAction?.FlowParameters)?.IsOpen;

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_ActiveClass_Correctly(bool isActiveAccountType)
		{
			var navigationItems = (await Context
				.WithIsMultiPageView(true)
				.WithHasElectricityAccounts(true)
				.WithAccountType(isActiveAccountType ? ClientAccountType.Electricity : ClientAccountType.Gas)
				.WithIsOpen(true)
				.Sut
				.Resolve(Context.BuildInput()))
				.NavigationItems
				.ToArray();

			var expected = isActiveAccountType;
			var actual = navigationItems?.First()?.ClassList.Contains("active");

			Assert.AreEqual(expected, actual);
		}

		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			private bool _isMultiPageView;
			private bool _hasElectricityAccounts;
			private bool _hasGasAccounts;
			private bool _hasEnergyServiceAccounts;
			public DomainFacade DomainFacade { get; } = new DomainFacade();
			private ClientAccountType _accountType;
			private bool _isOpen=true;

			public TestContext WithIsMultiPageView(bool isMultiPageView)
			{
				_isMultiPageView = isMultiPageView;
				return this;
			}

			public TestContext WithHasElectricityAccounts(bool hasElectricityAccounts)
			{
				_hasElectricityAccounts = hasElectricityAccounts;
				return this;
			}

			public TestContext WithHasGasAccounts(bool hasGasAccounts)
			{
				_hasGasAccounts = hasGasAccounts;
				return this;
			}

			public TestContext WithHasEnergyServiceAccounts(bool hasEnergyServiceAccounts)
			{
				_hasEnergyServiceAccounts = hasEnergyServiceAccounts;
				return this;
			}

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

			protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
			{
				ConfigureDomainFacade(autoMocker);
				return base.BuildSut(autoMocker);
			}

			private void ConfigureDomainFacade(AutoMocker autoMocker)
			{
				var cfg = new AppUserConfigurator(DomainFacade);
				if (_hasElectricityAccounts)
				{
					cfg.AddElectricityAccount(_isOpen&&!_isMultiPageView);
					if (_isMultiPageView)
					{
						cfg.AddElectricityAccount();
						cfg.AddElectricityAccount(_isOpen);
						cfg.AddGasAccount();
						cfg.AddGasAccount(_isOpen);
					}
				}

				if (_hasGasAccounts)
				{
					cfg.AddGasAccount(_isOpen&&!_isMultiPageView);
					if (_isMultiPageView)
					{
						cfg.AddElectricityAccount();
						cfg.AddElectricityAccount(_isOpen);
						cfg.AddGasAccount();
						cfg.AddGasAccount(_isOpen);

					}
				}

				if (_hasEnergyServiceAccounts)
				{
					cfg.AddEnergyServicesAccount(_isOpen&&!_isMultiPageView);
					if (_isMultiPageView)
					{
						cfg.AddElectricityAccount();
						cfg.AddElectricityAccount(_isOpen);
						cfg.AddEnergyServicesAccount();
						cfg.AddEnergyServicesAccount(_isOpen);
					}
				}

				cfg.Execute();

				autoMocker.Use(DomainFacade.QueryResolver.Current.Object);
			}

			public InputModel BuildInput()
			{
				return new InputModel
				{

					AccountType = _accountType,
					IsOpen = _isOpen
				};
			}
		}
	}
}
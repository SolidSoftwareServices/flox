using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.CoreServices.System.FastReflection;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCard;
using EI.RP.WebApp.UnitTests.Infrastructure;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Accounts.Components.AccountCard
{
	[TestFixture]
	internal class AccountCard_ViewBuilder_Test : UnitTestFixture<AccountCard_ViewBuilder_Test.TestContext, ViewModelBuilder>
	{
		[Theory]
		public async Task Resolve_HasStartedFromMeterReading_Correctly(bool canSubmitMeterReading)
		{
			var expected = canSubmitMeterReading;

			var actual = (await Context
					.WithHasStartedFromMeterReading(canSubmitMeterReading)
					.Sut
					.Resolve(Context.BuildInput())
				)
				.CanSubmitMeterReading;

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_ShowBillingDetails_Correctly(bool showBillingDetails)
		{
			var expected = showBillingDetails;

			var actual = (await Context
					.WithShowBillingDetails(showBillingDetails)
					.Sut
					.Resolve(Context.BuildInput()))
				.ShowBillingDetails;

			Assert.AreEqual(expected, actual);
		}

		[Theory]
		public async Task Resolve_Account_Correctly()
		{
			var actual = (await Context
					.Sut
					.Resolve(Context.BuildInput()))
				.AccountNumber;
			var expected = Context.Account;
			Assert.AreEqual(expected.AccountNumber, actual);
		}

		public class TestContext : UnitTestContext<ViewModelBuilder>
		{
			private bool _hasStartedFromMeterReading;
			private bool _showBillingDetails;
			private readonly DomainFacade _domainFacade = new DomainFacade();

			public TestContext WithHasStartedFromMeterReading(bool hasStartedFromMeterReading)
			{
				_hasStartedFromMeterReading = hasStartedFromMeterReading;
				return this;
			}

			public TestContext WithShowBillingDetails(bool showBillingDetails)
			{
				_showBillingDetails = showBillingDetails;
				return this;
			}

			protected override ViewModelBuilder BuildSut(AutoMocker autoMocker)
			{
				var cfg = new AppUserConfigurator(_domainFacade);
				cfg.AddElectricityAccount();
				cfg.Execute();

				this.Account = cfg.Accounts.Single();

				_domainFacade.SetUpMocker(autoMocker);
				return base.BuildSut(autoMocker);
			}

			public AccountInfo Account { get; private set; }

			public InputModel BuildInput()
			{
				return new InputModel
				{
					CanSubmitMeterReading = _hasStartedFromMeterReading,
					ShowBillingDetails = _showBillingDetails,
					AccountNumber = Account.AccountNumber
				};
			}
		}
	}
}
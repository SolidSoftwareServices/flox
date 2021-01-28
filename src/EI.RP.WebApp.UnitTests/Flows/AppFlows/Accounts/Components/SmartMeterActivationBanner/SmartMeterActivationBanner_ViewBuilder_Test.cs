using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.DataModels.Events;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.TestServices;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Components.SmartMeterActivationBanner;
using EI.RP.WebApp.Flows.AppFlows.Accounts.Steps;
using EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Accounts.Components.SmartMeterActivationBanner
{
	[TestFixture]
	internal class
		SmartMeterActivationBanner_ViewBuilder_Test : UnitTestFixture<SmartMeterActivationBannerTestContext,
			ViewModelBuilder>
	{
		public static IEnumerable<TestCaseData> ItResolvesTheSmartActivationInfoCases()
		{
			var bools = new[] {true, false};
			foreach (var isSmartActivationEnabled in bools)
			foreach (var isOpen in bools)
			foreach (var isAlreadySmart in bools)
			foreach (var canOptToSmart in bools)
			foreach (var isNotificationDismissed in bools)
			foreach (var clientAccountType in new[] {ClientAccountType.Electricity, ClientAccountType.Gas})
			{
				yield return new TestCaseData(isSmartActivationEnabled,
						isAlreadySmart, canOptToSmart, isNotificationDismissed,
						clientAccountType, isOpen)
					.SetName(ResolveName());

				string ResolveName()
				{
					var at = clientAccountType.IsElectricity() ? "Electricity" : "Gas";
					var sae = isSmartActivationEnabled ? "Enabled" : "Disabled";
					var o = isOpen ? "Open" : "Closed";
					var sm = isAlreadySmart ? "already smart" : "not smart";
					var c = canOptToSmart ? "account can be made smart" : string.Empty;
					var d = isNotificationDismissed ? string.Empty : "NOT";
					return
						$"AccountType: {at}, Account is {o} and {sm}, SmartActivation: {sae}, {c} and notification is {d} dismissed for account";
				}
			}
		}


		[TestCaseSource(nameof(ItResolvesTheSmartActivationInfoCases))]
		public async Task ResolveCanOptToSmart(bool isSmartActivationEnabled, 
			bool isAlreadySmart, bool canOptToSmart, bool isNotificationDismissed, ClientAccountType accountType,
			bool isOpen)
		{
			var expected = isSmartActivationEnabled &&
			               isOpen &&
			               accountType.IsElectricity() &&
			               !isAlreadySmart &&
			               !isNotificationDismissed &&
			               canOptToSmart;

			var dismissEvent = AccountSelection.StepEvent.DismissSmartActivationNotification;
			var flowEvent = AccountSelection.StepEvent.ToSmartActivation;

			var actual = await Context
				.WithAccountType(accountType)
				.WithEvents(dismissEvent, flowEvent)
				.WithIsAccountOpen(isOpen)
				.WithIsAccountAlreadySmart(isAlreadySmart)
				.WithIsSmartActivationEnabled(isSmartActivationEnabled)
				.WithHasSmartEligibleAccounts(canOptToSmart)
				.WithIsNotificationDismissed(isNotificationDismissed)
				.Sut
				.Resolve(Context.BuildInput());

			Assert.AreEqual(expected, actual.CanOptToSmart);

			if (!actual.CanOptToSmart) return;

			var electricityAccount = Context.ElectricityAccounts.First();
			var eligibleAccount = actual.SmartActivationEligibleItems.FirstOrDefault();
			Assert.AreEqual(electricityAccount.AccountNumber, eligibleAccount?.AccountNumber);
			Assert.AreEqual(dismissEvent, eligibleAccount?.DismissNotificationAction?.TriggerEvent);
			Assert.AreEqual(flowEvent, eligibleAccount?.FlowAction?.TriggerEvent);
			var contractItem = electricityAccount.BusinessAgreement.Contracts.FirstOrDefault();
			var mprn = contractItem?.Premise?.PointOfDeliveries?.FirstOrDefault()?.Prn ?? string.Empty;
			var eventPublisher = Context.AutoMocker.GetMock<IUIEventPublisher>();
			eventPublisher.Verify(x => x.Publish(
				It.Is<UiEventInfo>(e =>
					e.Description == "View Smart Activation Notice" &&
					e.AccountNumber == electricityAccount.AccountNumber &&
					e.Partner == electricityAccount.Partner &&
					e.MPRN == electricityAccount.PointReferenceNumber.ToString() &&
					e.SubCategoryId == EventSubCategory.ShowSmartActivationNotificationToUser &&
					e.CategoryId == EventCategory.View)
			), Times.Once);
		}
	}
}
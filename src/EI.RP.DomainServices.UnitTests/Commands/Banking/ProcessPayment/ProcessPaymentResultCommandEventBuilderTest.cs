using System.Threading.Tasks;
using EI.RP.DomainServices.Commands.Banking.ProcessPayment;
using EI.RP.TestServices;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.DataModels.Events;
using NUnit.Framework;
using AutoFixture;

namespace EI.RP.DomainServices.UnitTests.Commands.Banking.ProcessPayment
{
	internal class ProcessPaymentResultCommandEventBuilderTest : UnitTestFixture<ProcessPaymentResultCommandEventBuilderTest.TestContext,
		ProcessPaymentResultCommandEventBuilder>
	{
		public class TestContext : UnitTestContext<ProcessPaymentResultCommandEventBuilder>
		{
		}

		[Test]
		public async Task TestPaymentSucessfullCommandResultBuildEventsOnSuccessEvents()
		{
			var cmd = ArrangeAndGetPaymentSuccessfulResultCommand();
			var actual =
				await Context
				.Sut
				.BuildEventsOnSuccess(cmd);

			AssertResult((EventApiEvent)actual[0], "Payment made Successfully", EventAction.LastOperationWasSuccessful, cmd.UserName, cmd.Partner, cmd.AccountNumber);
		}

		[Test]
		public async Task TestPaymentSucessfullCommandResultBuildEventsOnErrorEvents()
		{
			var cmd = ArrangeAndGetPaymentSuccessfulResultCommand();
			var actual =
				await Context
				.Sut
				.BuildEventsOnError(cmd, null);

			AssertResult((EventApiEvent)actual[0], "Payment Successful, payment result handling failed", EventAction.LastOperationFailed, cmd.UserName, cmd.Partner, cmd.AccountNumber);
		}

		[Test]
		public async Task TestPaymentFailedCommandResultBuildEventsOnSuccessEvents()
		{
			var cmd = ArrangeAndGetPaymentFailedCommand();
			var actual =
				await Context
				.Sut
				.BuildEventsOnSuccess(cmd);

			AssertResult((EventApiEvent)actual[0], "Payment failed", EventAction.LastOperationFailed, cmd.UserName, cmd.Partner, cmd.AccountNumber);
		}

		[Test]
		public async Task TestPaymentFailedCommandResultBuildOnErrorEvents()
		{
			var cmd = ArrangeAndGetPaymentFailedCommand();
			var actual =
				await Context
				.Sut
				.BuildEventsOnError(cmd, null);

			AssertResult((EventApiEvent)actual[0], "Payment failed", EventAction.LastOperationFailed, cmd.UserName, cmd.Partner, cmd.AccountNumber);
		}

		void AssertResult(EventApiEvent actual, string expectedDescription, long eventAction, string userName, string partner, string accountNumber)
		{
			const int maxDescriptionLength = 50;

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.Description.Length <= maxDescriptionLength);
			Assert.IsTrue(actual.Description.Equals(expectedDescription));
			Assert.IsTrue(actual.ActionId.Equals(eventAction));
			Assert.IsTrue(actual.CategoryId.Equals(EventCategory.PaymentResult));
			Assert.IsTrue(actual.Username.Equals(userName));
			Assert.IsTrue(actual.Partner.Equals(long.Parse(partner)));
			Assert.IsTrue(actual.ContractAccount.Equals(long.Parse(accountNumber)));
			Assert.IsTrue(actual.SubCategoryId.Equals(EventSubCategory.PaymentResult));
		}

		ProcessPaymentFailedResultCommand ArrangeAndGetPaymentFailedCommand()
		{
			var accountNumber = Context.Fixture.Create<long>();
			var partner = Context.Fixture.Create<long>();
			var paymentCardType = Context.Fixture.Create<string>();
			var payerReference = Context.Fixture.Create<string>();
			var userName = Context.Fixture.Create<string>();

			var cmd = new ProcessPaymentFailedResultCommand(partner.ToString(), paymentCardType, payerReference, userName, accountNumber.ToString());
			return cmd;
		}

		ProcessPaymentSuccessfulResultCommand ArrangeAndGetPaymentSuccessfulResultCommand()
		{
			var accountNumber = Context.Fixture.Create<long>();
			var partner = Context.Fixture.Create<long>();
			var paymentCardType = Context.Fixture.Create<string>();
			var payerReference = Context.Fixture.Create<string>();
			var userName = Context.Fixture.Create<string>();

			var cmd = new ProcessPaymentSuccessfulResultCommand(partner.ToString(), paymentCardType, payerReference, userName, accountNumber.ToString());
			return cmd;
		}

	}
}

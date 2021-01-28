using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Cqrs.Commands;

using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.DomainServices.Commands.Billing.RequestRefund;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Commands.Billing.RequestRefund
{
	internal class RequestRefundCommandHandlerTest : CommandHandlerTest<RequestRefundCommandHandler,
		RequestRefundCommand>
	{
		public static IEnumerable ItPublishesCorrectBillingActivityCases()
		{
			foreach (var method in PaymentMethodType.AllValues.Where(x=>x!=PaymentMethodType.Manual) )
			{
				yield return new TestCaseData(method).SetName($"{nameof(ItPublishesCorrectBillingActivity)}WhenPaymentMethodIs{method.DebuggerDisplayValue}");
			}
		}
		
		[Test,TestCaseSource(nameof(ItPublishesCorrectBillingActivityCases))]
		public async Task ItPublishesCorrectBillingActivity(PaymentMethodType methodType)
		{
			var cmd = ArrangeAndGetCommand();

			await Context.Sut.ExecuteAsync(new RequestRefundCommand(cmd.AccountNumber,cmd.BusinessPartner,methodType,cmd.Description));

			Assert();

			void Assert()
			{
				var publisher = Context.AutoMocker.GetMock<ICommandHandler<PublishBusinessActivityDomainCommand>>();
				
					publisher.Verify(x => x.ExecuteAsync(It.Is<PublishBusinessActivityDomainCommand>(y=>y==cmd)), Times.Once);
			}

			PublishBusinessActivityDomainCommand ArrangeAndGetCommand()
			{
				var t= methodType==PaymentMethodType.DirectDebit || methodType == PaymentMethodType.Equalizer
					? PublishBusinessActivityDomainCommand.BusinessActivityType.RefundRequestForDirectDebit
					: PublishBusinessActivityDomainCommand.BusinessActivityType.RefundRequestForNonDirectDebit;
				return new PublishBusinessActivityDomainCommand(t,
					Context.Fixture.Create<string>(), Context.Fixture.Create<string>(), documentStatus: "E0001",
					processType: "ZCUT", description: Context.Fixture.Create<string>(), subject: "Refund");
			}
		}
	}
}
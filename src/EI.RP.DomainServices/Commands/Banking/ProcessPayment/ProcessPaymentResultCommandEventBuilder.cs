using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Events;
using EI.RP.CoreServices.Http;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Events;

namespace EI.RP.DomainServices.Commands.Banking.ProcessPayment
{
	internal class ProcessPaymentResultCommandEventBuilder :
		ICommandEventBuilder<ProcessPaymentSuccessfulResultCommand>,
		ICommandEventBuilder<ProcessPaymentFailedResultCommand>
	{
		private readonly IClientInfoResolver _clientInfoResolver;

		public ProcessPaymentResultCommandEventBuilder(IClientInfoResolver clientInfoResolver)
		{
			_clientInfoResolver = clientInfoResolver;
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(ProcessPaymentFailedResultCommand command)
		{
			//the following is correct
			return await BuildFor(command, EventAction.LastOperationFailed, "Payment failed");
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(ProcessPaymentFailedResultCommand command,
            AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed, "Payment failed");
		}

		public async Task<IEventApiMessage[]> BuildEventsOnSuccess(ProcessPaymentSuccessfulResultCommand command)
		{
			return await BuildFor(command, EventAction.LastOperationWasSuccessful, "Payment made Successfully");
		}

		public async Task<IEventApiMessage[]> BuildEventsOnError(ProcessPaymentSuccessfulResultCommand command,
            AggregateException exceptions)
		{
			return await BuildFor(command, EventAction.LastOperationFailed,
				"Payment Successful, payment result handling failed");
		}


		private async Task<IEventApiMessage[]> BuildFor(PaymentResultCommand command, long eventAction,
			string description)
		{
			return((IEventApiMessage)new EventApiEvent(_clientInfoResolver)
			{
				CategoryId = EventCategory.PaymentResult,
				ActionId = eventAction,
				Username = command.UserName,
				Partner = long.Parse(command.Partner),
				ContractAccount = long.Parse(command.AccountNumber),


				Description = description,
				SubCategoryId = EventSubCategory.PaymentResult
			}).ToOneItemArray();
		}
	}
}
using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using NLog;

namespace EI.RP.DomainServices.Commands.Banking.ProcessPayment
{
	internal class ProcessPaymentResultCommandHandler : ICommandHandler<ProcessPaymentSuccessfulResultCommand>,
		ICommandHandler<ProcessPaymentFailedResultCommand>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly ICommandHandler<PublishBusinessActivityDomainCommand> _activityPublisher;
		private readonly ISapRepositoryOfCrmUmc _crmUmcRepository;

		public ProcessPaymentResultCommandHandler(ISapRepositoryOfCrmUmc crmUmcRepository,
			ICommandHandler<PublishBusinessActivityDomainCommand> activityPublisher)
		{
			_crmUmcRepository = crmUmcRepository;
			_activityPublisher = activityPublisher;
		}

		public async Task ExecuteAsync(ProcessPaymentFailedResultCommand command)
		{
			await PublishErrorResultActivity(command);
		}

		public async Task ExecuteAsync(ProcessPaymentSuccessfulResultCommand command)
		{
			await Task.WhenAll(AddPaymentCard(command), PublishSuccessfulResultActivity(command));
		}

		private async Task PublishErrorResultActivity(ProcessPaymentFailedResultCommand commandData)
		{
			var businessActivityType = PublishBusinessActivityDomainCommand.BusinessActivityType.PaymentResultFailed;

			await _activityPublisher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
				commandData.Partner, commandData.AccountNumber));
		}


		private async Task PublishSuccessfulResultActivity(ProcessPaymentSuccessfulResultCommand commandData)
		{
			var businessActivityType =
				PublishBusinessActivityDomainCommand.BusinessActivityType.PaymentResultSuccessful;

			try
			{
				await _activityPublisher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
					commandData.Partner, commandData.AccountNumber));
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		private async Task AddPaymentCard(ProcessPaymentSuccessfulResultCommand commandData)
		{
			var cardsRequest = _crmUmcRepository
				.NewQuery<AccountDto>()
				.Key(commandData.Partner)
				.NavigateTo<PaymentCardDto>()
				.GetMany();

			var paymentCard = new PaymentCardDto();
			paymentCard.AccountID = commandData.Partner;
			paymentCard.PaymentCardID = string.Empty;
			paymentCard.PaymentCardTypeID = commandData.PaymentCardType;
			paymentCard.StandardFlag = true;
			paymentCard.Issuer = string.Empty;
			if (commandData.PayerReference != default(string) && commandData.PayerReference.Length > 40)
			{
				paymentCard.Description = commandData.PayerReference.Substring(0, 40);
				paymentCard.Issuer = commandData.PayerReference.Substring(40, commandData.PayerReference.Length - 40);
			}
			else
			{
				paymentCard.Description = commandData.PayerReference;
			}

			paymentCard.Cardholder = commandData.UserName;

			paymentCard.CardNumber = DateTime.UtcNow.ToFileTime().ToString(); // funny stuff cardNumber;

			try
			{
				var cardExists = (await cardsRequest).Any(x => x.StandardFlag && x.Cardholder == commandData.UserName);
				if (commandData.PayerReference != default(string) && !cardExists)
					await _crmUmcRepository.Add(paymentCard);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}
	}
}
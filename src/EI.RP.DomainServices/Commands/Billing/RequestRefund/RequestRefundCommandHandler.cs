using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using NLog;

namespace EI.RP.DomainServices.Commands.Billing.RequestRefund
{
    internal class RequestRefundCommandHandler : ICommandHandler<RequestRefundCommand>
    {
        private readonly ICommandHandler<PublishBusinessActivityDomainCommand> _businessActivityPublisher;

        public RequestRefundCommandHandler(ICommandHandler<PublishBusinessActivityDomainCommand> businessActivityPublisher)
        {
            _businessActivityPublisher = businessActivityPublisher;
        }

        public async Task ExecuteAsync(RequestRefundCommand command)
        {
            var businessActivityType =
                command.PaymentMethod == PaymentMethodType.DirectDebit ||command.PaymentMethod == PaymentMethodType.Equalizer
                    ? PublishBusinessActivityDomainCommand.BusinessActivityType.RefundRequestForDirectDebit
                    : PublishBusinessActivityDomainCommand.BusinessActivityType.RefundRequestForNonDirectDebit;

            await _businessActivityPublisher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
                command.Partner, command.AccountNumber,documentStatus: "E0001",processType: "ZCUT",description:command.Description,subject:"Refund"));
        }

    }
}
using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.Cqrs.Commands;
using FluentValidation;

namespace EI.RP.DomainServices.Commands.Billing.RequestRefund
{
    internal class RequestRefundCommandValidator : CommandValidator<RequestRefundCommand>
    {
        protected override void RegisterValidations(AbstractValidator<RequestRefundCommand> validation)
        {
            validation.RuleFor(x => x.Description.ToString()).MaximumLength(1900).NotEmpty()
                .Matches(@"^[^<><|>]+$");
        }
    }
}

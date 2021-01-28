using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.DomainServices.Validation;
using FluentValidation;

namespace EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit
{
	internal class SetUpDirectDebitCommandValidator : CommandValidator<SetUpDirectDebitDomainCommand>
	{
		private readonly IReservedIbanService _reservedIbanService;
		public SetUpDirectDebitCommandValidator(IReservedIbanService reservedIbanService)
		{
			_reservedIbanService = reservedIbanService;
		}
		protected override void RegisterValidations(AbstractValidator<SetUpDirectDebitDomainCommand> validation)
		{
            //TODO: VALIDATE IBAN HERE TOO
            validation.RuleFor(x => x.NewIBAN.ToString()).NotEmpty()
                .Matches(@"^([Ii][Ee][A-Za-z0-9\*]{20})$");

            validation.RuleFor(x => x.NameOnBankAccount.ToString()).NotEmpty();

			validation.RuleFor(x => !_reservedIbanService.IsReservedIban(x.NewIBAN));
		}
	}
}
using EI.RP.CoreServices.Cqrs.Commands;
using FluentValidation;

namespace EI.RP.DomainServices.Commands.Users.Membership.CreateAccount
{
	internal class CreateAccountCommandValidator : CommandValidator<CreateAccountCommand>
	{
		protected override void RegisterValidations(AbstractValidator<CreateAccountCommand> validation)
		{
			validation.RuleFor(x => x.MPRNGPRNLast6DigitsOf.ToString()).Length(6);
		}
	}
}
using EI.RP.CoreServices.Cqrs.Commands;
using FluentValidation;

namespace EI.RP.DomainServices.Commands.Users.Session.CreateUserSession
{
	internal class CreateUserSessionValidator : CommandValidator<CreateUserSessionCommand>
	{
		protected override void RegisterValidations(AbstractValidator<CreateUserSessionCommand> validation)
		{
			//TODO:
			//validation.RuleFor(x=>x.MPRNGPRNLast6DigitsOf.ToString()).Length(exactLength:6);
		}
	}
}
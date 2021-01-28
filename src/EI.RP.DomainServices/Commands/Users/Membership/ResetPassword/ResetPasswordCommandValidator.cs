using EI.RP.CoreServices.Cqrs.Commands;
using FluentValidation;

namespace EI.RP.DomainServices.Commands.Users.Membership.ResetPassword
{
	internal class ResetPasswordCommandValidator : CommandValidator<ResetPasswordDomainCommand>
	{
		protected override void RegisterValidations(AbstractValidator<ResetPasswordDomainCommand> validation)
		{
		}
	}
}
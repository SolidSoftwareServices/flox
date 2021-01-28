using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Billing.Info;
using FluentValidation;

namespace EI.RP.DomainServices.Commands.Billing.ChangeBillingPeriod
{
	public sealed class SetMonthlyBillingPeriodCommandValidator : CommandValidator<SetMonthlyBillingPeriodCommand>
	{

		private readonly IDomainQueryResolver _queryResolver;

		public SetMonthlyBillingPeriodCommandValidator(IDomainQueryResolver queryResolver)
		{
			_queryResolver = queryResolver;
		}

		protected override void RegisterValidations(AbstractValidator<SetMonthlyBillingPeriodCommand> validation)
		{
			validation.RuleFor(x => x.DayOfTheMonth).InclusiveBetween(1, 28);
		}

		protected override async Task OnValidateComplexAsync(SetMonthlyBillingPeriodCommand command)
		{
			var billingInfo= await _queryResolver.GetAccountBillingInfoByAccountNumber(command.AccountNumber, byPassPipeline: true);
			if (!billingInfo.MonthlyBilling.CanSwitchToMonthlyBilling)
			{
				throw new ValidationException("Cannot set monthly billing period");
			}
		}
	}
}
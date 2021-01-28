using System;
using EI.RP.CoreServices.Http;
using FluentValidation;

namespace EI.RP.CoreServices.Cqrs.Events
{
	public partial class EventApiEvent
	{
		private static readonly Validator EventValidator = new Validator();

		public IEventApiMessage Validate()
		{
			EventValidator.ValidateAndThrow(this);
			return this;
		}

		private class Validator : AbstractValidator<EventApiEvent>
		{
			public Validator()
			{
				RuleFor(x => x.ApplicationName).MaximumLength(50).NotNull();
				RuleFor(x => x.BrowserVersion).MaximumLength(50);
				RuleFor(x => x.Description).MaximumLength(50);
				RuleFor(x => x.CategoryId).NotNull();
				RuleFor(x => x.DeviceInfo).MaximumLength(500);
				RuleFor(x => x.OperatingSystem).MaximumLength(50);
				RuleFor(x => x.IpAddress).MaximumLength(50);
				RuleFor(x => x.Hardware).MaximumLength(50);
				RuleFor(x => x.MPRN).MaximumLength(6).WithMessage("Last six digits of MPRN");
				RuleFor(x => x.Timestamp).NotNull();
				RuleFor(x => x.Partner).NotNull();

			}
		}
	}
}
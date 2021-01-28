using System;
using AutoFixture.Kernel;
using EI.RP.CoreServices.System;

namespace EI.RP.TestServices.SpecimenGeneration.DefaultSpecimens
{
	public class EmailAddressSpecimenBuilder : ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			var type = request as Type;
			if (type != typeof(EmailAddress))
				return new NoSpecimen();

			return new EmailAddress($"Email{DateTime.UtcNow.Ticks}@Domain{DateTime.UtcNow.Ticks}.com");
		}
	}

}
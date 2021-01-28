using System;
using AutoFixture.Kernel;
using EI.RP.CoreServices.System;

namespace EI.RP.TestServices.SpecimenGeneration.DefaultSpecimens
{
	public class DateTimeRangeSpecimenBuilder : ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			var type = request as Type;
			if (type != typeof(DateTimeRange))
				return new NoSpecimen();

			return new DateTimeRange((DateTime)context.Resolve(typeof(DateTime)),(TimeSpan)context.Resolve(typeof(TimeSpan)));
		}
	}
}
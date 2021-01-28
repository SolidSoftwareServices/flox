using System;
using AutoFixture;
using AutoFixture.Kernel;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public class DefaultPointReferenceNumberSpecimenBuilder : ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			var requestedType = request as Type;

			if (typeof(PointReferenceNumber) != requestedType)
			{
				return new NoSpecimen();
			}

			//lets return this by default
			return context.Create<ElectricityPointReferenceNumber>();
		}

	}
}
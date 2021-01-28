using System;
using System.Threading;
using AutoFixture.Kernel;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public class GasPointReferenceNumberSpecimenBuilder : ISpecimenBuilder
	{
		private int _counter = new Random((int)DateTime.UtcNow.Ticks).Next(100,20000);
		public object Create(object request, ISpecimenContext context)
		{
			var requestedType = request as Type;

			if (typeof(GasPointReferenceNumber) != requestedType)
			{
				return new NoSpecimen();
			}
			return new GasPointReferenceNumber(Interlocked.Increment(ref _counter));
		}

	}
}
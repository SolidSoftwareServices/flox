using System;
using System.Threading;
using AutoFixture.Kernel;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public class ElectricityPointReferenceNumberSpecimenBuilder : ISpecimenBuilder
	{
		private int _counter = new Random((int)DateTime.UtcNow.Ticks).Next(100,99900);

		public object Create(object request, ISpecimenContext context)
		{
			var requestedType = request as Type;

			if (typeof(ElectricityPointReferenceNumber) != requestedType)
			{
				return new NoSpecimen();
			}

            
            var electricityPointReferenceNumber = new ElectricityPointReferenceNumber($"1{Interlocked.Increment(ref _counter).ToString().PadLeft(10,'0')}");
			return electricityPointReferenceNumber;
		}
		
	}
}
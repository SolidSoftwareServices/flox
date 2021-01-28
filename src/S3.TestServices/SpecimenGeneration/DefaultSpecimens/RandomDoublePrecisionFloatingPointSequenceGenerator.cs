using System;
using AutoFixture.Kernel;

namespace S3.TestServices.SpecimenGeneration.DefaultSpecimens
{
	/// <summary>
	/// It generates data from objectmothers with floating part that will work with dn decimal types
	/// It Generates a number from 0.00 to 9.99. Note the 2 decimal numbers
	/// </summary>
	public class RandomDoublePrecisionFloatingPointSequenceGenerator
	: ISpecimenBuilder
	{
		private readonly object _syncRoot;
		private readonly Random _random;

		public RandomDoublePrecisionFloatingPointSequenceGenerator()
		{
			this._syncRoot = new object();
			this._random = new Random();
		}

		public object Create(object request, ISpecimenContext context)
		{
			var type = request as Type;
			if (type == null)
				return new NoSpecimen();

			return this.CreateRandom(type);
		}

		private double GetNextRandom()
		{
			lock (this._syncRoot)
			{
				return Math.Round(this._random.Next(0,9)+ this._random.NextDouble(),2);
			}
		}

		private object CreateRandom(Type request)
		{
			switch (Type.GetTypeCode(request))
			{
				case TypeCode.Decimal:
					return (decimal)
						this.GetNextRandom();

				case TypeCode.Double:
					return (double)
						this.GetNextRandom();

				case TypeCode.Single:
					return (float)
						this.GetNextRandom();

				default:
					return new NoSpecimen();
			}
		}
	}
}

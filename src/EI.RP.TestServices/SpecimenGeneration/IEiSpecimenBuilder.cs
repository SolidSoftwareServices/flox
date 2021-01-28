using System;
using AutoFixture.Kernel;

namespace EI.RP.TestServices.SpecimenGeneration
{
	public interface IEiSpecimenBuilder : ISpecimenBuilder
	{
		Type SpecimenToBuildType { get; }
	}
}
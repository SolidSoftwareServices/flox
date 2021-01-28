using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using EI.RP.TestServices.SpecimenGeneration;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public static class FixtureDomainExtensions
	{
		public static IFixture CustomizeDomainTypeBuilders(this IFixture fixture)
		{
			fixture.CustomizeFrameworkBuilders();

			fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
				.ForEach(b => fixture.Behaviors.Remove(b));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());



			var enumerable = typeof(FixtureDomainExtensions).Assembly.GetTypes()
				.Where(x => !x.IsAbstract && typeof(ISpecimenBuilder).IsAssignableFrom(x)).ToArray();
			foreach (var type in enumerable)
			{
				fixture.Customizations.Add((ISpecimenBuilder) Activator.CreateInstance(type));
			}


			return fixture;
		}
	}
}
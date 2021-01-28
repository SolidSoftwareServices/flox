using System;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;

namespace S3.TestServices.SpecimenGeneration
{
	public static class FixtureExtensions
	{
		public static IFixture CustomizeFrameworkBuilders(this IFixture source)
		{
			source.Customize(new AutoMoqCustomization());

			foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
				.Where(x => !x.IsAbstract && typeof(ISpecimenBuilder).IsAssignableFrom(x)))
				source.Customizations.Add((ISpecimenBuilder) Activator.CreateInstance(type));


			return source;
		}
	}
}
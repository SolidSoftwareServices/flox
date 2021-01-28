using System.Collections.Generic;
using System.Reflection;
using AutoFixture.Kernel;
using EI.RP.CoreServices.Ports.OData;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public class DtoSpecialPropertiesBuilder : ISpecimenBuilder
	{
		private static readonly Dictionary<string, object> KnownFixedPropertyValues =
			new Dictionary<string, object>
			{
				{"AlternativePayerID", string.Empty}
			};


		public object Create(object request, ISpecimenContext context)
		{
			var result = ResolveCoherentDtoPropertyValue(request);


			return result;
		}

		private object ResolveCoherentDtoPropertyValue(object request)
		{
			var t = request as PropertyInfo;
			if (t == null || !typeof(ODataDtoModel).IsAssignableFrom(t.DeclaringType) ||
			    !KnownFixedPropertyValues.ContainsKey(t.Name))
				return new NoSpecimen();


			return KnownFixedPropertyValues[t.Name];
		}
	}
}
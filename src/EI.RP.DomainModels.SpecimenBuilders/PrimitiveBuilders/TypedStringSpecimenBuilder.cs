using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using AutoFixture.Kernel;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.MappingValues;

namespace EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders
{
	public class TypedStringSpecimenBuilder : ISpecimenBuilder
	{
		private static readonly ConcurrentDictionary<Type, object[]> TypedStringValues =
			new ConcurrentDictionary<Type, object[]>();

		private static int _counter;

		public object Create(object request, ISpecimenContext context)
		{
			var result= ResolveDirectTypedStringValue(request);
			if (result is NoSpecimen)
			{
				result = ResolveCoherentDtoPropertyValue(request);
			}

			return result;
		}

		

		private static object ResolveDirectTypedStringValue(object request)
		{
			var t = request as Type;
			if (t == null || t.IsAbstract || !typeof(TypedStringValue).IsAssignableFrom(t))
				return new NoSpecimen();

			return GetNextValue(t);
		}

		private static object GetNextValue(Type type)
		{
			var values = TypedStringValues.GetOrAdd(type, t =>
			{
				return t.GetMembers(BindingFlags.Public | BindingFlags.Static)
					.Where(x => (x.MemberType & MemberTypes.Field) != 0).Cast<FieldInfo>()
					.Where(x => x.FieldType == type).Select(x => x.GetValue(null)).ToArray();
			});

			var index = Interlocked.Increment(ref _counter);

			return values[index < values.Length ? index : index % values.Length];
		}

		//TODO: REMOVE this dependency
		private static readonly Dictionary<string, Type> KnownTypedStringValues =
			new Dictionary<string, Type>
			{
				{"DivisionID",typeof(DivisionType) }
				,{"AccountCategory",typeof(DivisionType)}
				,{"IncomingPaymentMethodID",typeof(PaymentMethodType)}

				,{nameof(LatestBillInfo.PaymentMethod),typeof(PaymentMethodType)}
				,{nameof(LatestBillInfo.MeterReadingType),typeof(MeterReadingCategoryType)}
			};
		private object ResolveCoherentDtoPropertyValue(object request)
		{
			var t = request as PropertyInfo;
			if (t == null || !typeof(ODataDtoModel).IsAssignableFrom(t.DeclaringType) || !KnownTypedStringValues.ContainsKey(t.Name))
				return new NoSpecimen();

			

			return GetNextValue(KnownTypedStringValues[t.Name]).ToString();
		}
	}
}
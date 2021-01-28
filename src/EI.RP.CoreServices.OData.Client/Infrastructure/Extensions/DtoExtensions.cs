using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.FastReflection;
using ObjectsComparer;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Extensions
{
	internal static class DtoExtensions
	{
		private static readonly MethodInfo AddToDictionaryMethod = typeof(IDictionary<string, object>).GetMethod(nameof(Dictionary<string,string>.Add));
		private static readonly ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>> Converters = new ConcurrentDictionary<Type, Func<object, IDictionary<string, object>>>();
		private static readonly ConstructorInfo DictionaryConstructor = typeof(Dictionary<string, object>).GetConstructors().FirstOrDefault(c => c.IsPublic && !c.GetParameters().Any());
		private static readonly Type[] AcceptedNonPrimitiveTypes = { typeof(string),typeof(DateTime)};
		public static IDictionary<string, object> ToDictionary(this object obj)
		{
			bool IsIndexableProperty(PropertyInfo propertyInfo)
			{
				var propertyType
					= propertyInfo.PropertyType;
				var nullableUnderlyingType = Nullable.GetUnderlyingType(propertyType);
				return propertyInfo.CanRead && (propertyType.IsPrimitive ||
				                                AcceptedNonPrimitiveTypes.Contains(propertyType) 
												//repeat for nullable
				                                ||nullableUnderlyingType != null && (nullableUnderlyingType.IsPrimitive ||
				                                 AcceptedNonPrimitiveTypes.Contains(nullableUnderlyingType)));
			}

			return obj == null ? null : Converters.GetOrAdd(obj.GetType(), o =>
			{
				var outputType = typeof(IDictionary<string, object>);
				var inputType = obj.GetType();
				var inputExpression = Expression.Parameter(typeof(object), "input");
				var typedInputExpression = Expression.Convert(inputExpression, inputType);
				var outputVariable = Expression.Variable(outputType, "output");
				var returnTarget = Expression.Label(outputType);
				var body = new List<Expression>
				{
					Expression.Assign(outputVariable, Expression.New(DictionaryConstructor))
				};
				body.AddRange(
					from prop in inputType.GetProperties(BindingFlags.Instance | BindingFlags.Public)// | BindingFlags.FlattenHierarchy)
					where IsIndexableProperty(prop)
					let getExpression =Expression.Convert(Expression.Property(typedInputExpression, prop.GetMethod),typeof(object))
					select Expression.Call(outputVariable, AddToDictionaryMethod, Expression.Constant(prop.Name), getExpression));
				body.Add(Expression.Return(returnTarget, outputVariable));
				body.Add(Expression.Label(returnTarget, Expression.Constant(null, outputType)));

				var lambdaExpression = Expression.Lambda<Func<object, IDictionary<string, object>>>(
					Expression.Block(new[] { outputVariable }, body),
					inputExpression);

				return lambdaExpression.Compile();
			})(obj);
		}

		private static readonly ConcurrentDictionary<Type, IBaseComparer> _comparers=new ConcurrentDictionary<Type, IBaseComparer>();
		public static IDictionary<string, object> ToDictionaryOfChangedValues<TDto>(this TDto changedObject, TDto originalObject) where TDto:ODataDtoModel
		{
			if (originalObject == null)
			{
				return changedObject.ToDictionary();
			}

			var result = new Dictionary<string, object>();

			var comparer=(ObjectsComparer.Comparer<TDto>)_comparers.GetOrAdd(typeof(TDto), (type) => new ObjectsComparer.Comparer<TDto>(new ComparisonSettings
			{
				RecursiveComparison = true,
				EmptyAndNullEnumerablesEqual = true,
				UseDefaultIfMemberNotExist = true
			}));
			if (!comparer.Compare(originalObject, changedObject, out var differences))
			{
				foreach (var diff in differences)
				{
					var propertyName = diff.MemberPath.Split('.')[0];
				
					result.Add(propertyName, changedObject.GetPropertyValueFast(propertyName));
				}
			}

			return result;
		}

		
	}
}
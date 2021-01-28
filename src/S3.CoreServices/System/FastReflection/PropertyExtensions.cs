using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace S3.CoreServices.System.FastReflection
{



#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class PropertyExtensions
	{
		/// <summary>
		/// Gets the property path of a given expression separated by '.'''
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string GetPropertyPath<T, TProperty>(this Expression<Func<T, TProperty>> expression)
		{
			MemberExpression memberExpression;
			switch (expression?.Body)
			{
				case null:
					throw new ArgumentNullException(nameof(expression));
				case UnaryExpression unaryExp when unaryExp.Operand is MemberExpression memberExp:
					memberExpression = memberExp;
					break;
				case MemberExpression memberExp:
					memberExpression = memberExp;
					break;
				default:
					throw new ArgumentException($"The expression doesn't indicate a valid property. [ {expression} ]");
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
			var sb = new StringBuilder(propertyInfo.Name);
			var nested = memberExpression.Expression;

			while (nested is MemberExpression)
			{
				memberExpression = (MemberExpression)nested;
				propertyInfo = (PropertyInfo)memberExpression.Member;
				sb.Insert(0, $"{propertyInfo.Name}.");
				nested = memberExpression.Expression;
			}

			return sb.ToString();
		}





		private const BindingFlags DefaultPropertyBindingFlags =BindingFlags.IgnoreCase| BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;

		private static readonly ConcurrentDictionary<string, PropertyInfo[]> _CachedProperties = new ConcurrentDictionary<string, PropertyInfo[]>();

		public static PropertyInfo[] GetPropertiesFast<TTarget>(this TTarget src,
			Func<PropertyInfo, bool> selectorFunc = null, string cacheKey = null)
		{
			return src.GetPropertiesFast(
					DefaultPropertyBindingFlags,
					selectorFunc);
		}

		public static PropertyInfo[] GetPropertiesFast(this Type forType, Func<PropertyInfo, bool> selectorFunc = null, string cacheKey = null)
		{
			return forType.GetPropertiesFast(DefaultPropertyBindingFlags, selectorFunc);
		}

		public static PropertyInfo[] GetPropertiesFast<TTarget>(this TTarget src, BindingFlags bindingFlags,
			Func<PropertyInfo, bool> selectorFunc = null, string cacheKey = null)
		{

			if (cacheKey == null)
			{
				return Resolve();
			}

			return _CachedProperties.GetOrAdd(cacheKey, x => Resolve());

			PropertyInfo[] Resolve()
			{
				var forType = src.GetType();
				return forType.GetPropertiesFast(bindingFlags, selectorFunc);
			}
		}

		public static PropertyInfo[] GetPropertiesFast(this Type forType, BindingFlags bindingFlags, Func<PropertyInfo, bool> selectorFunc = null, string cacheKey = null)
		{
			if (cacheKey == null)
			{
				return Resolve();
			}
			return _CachedProperties.GetOrAdd(cacheKey, x => Resolve());

			PropertyInfo[] Resolve()
			{
				var props = PropertyCallAdapterProvider.GetProperties(forType, bindingFlags);
				if (selectorFunc != null)
				{
					props = props.Where(selectorFunc).ToArray();
				}

				return props;
			}
		}
		public static IEnumerable<object> GetPropertyValuesFast<TTarget>(this TTarget src, params string[] propertyNames)
		{
			var result = propertyNames.Select(property => src.GetPropertyValueFast(property)).ToArray();
			return result;

		}
		
        public static object GetPropertyValueFast(this object src, PropertyInfo propertyInfo)
        {
            return src.GetPropertyValueFast(propertyInfo.Name);
        }


		public static object GetPropertyValueFast(this object src, string propertyName)
		{
			return PropertyCallAdapterProvider.GetInstance(src.GetType(),propertyName).InvokeGet(src);
		}
        public static void SetPropertyValueFast(this object src, PropertyInfo propertyInfo, object value)
        {
	        src.SetPropertyValueFast(propertyInfo.Name, value);
        }
		public static void SetPropertyValueFast<T, TProperty>(this T src, Expression<Func<T, TProperty>> expression, TProperty newValue)
		{
	        src.SetPropertyValueFastFromPropertyPath(expression.GetPropertyPath(),newValue);
        }
        public static void SetPropertyValueFast(this object src, string propertyName, object value,bool failIfPropertyNameNotFound=true)
        {
	        var adapter = PropertyCallAdapterProvider.GetInstance(src.GetType(), propertyName,failIfPropertyNameNotFound);
	        if (adapter!=null && adapter.HasSetter)
	        {
		        adapter.InvokeSet(src,value);
	        }
        }
        public static void SetPropertyValueFastFromPropertyPath<TPropertyType>(this object src, string propertyPath, TPropertyType value)
        {
	        object o = null;
	        var parts = propertyPath.Split('.');
	        for (var index = 0; index < parts.Length-1; index++)
	        {
		        var pathPart = parts[index];
		        o = src.GetPropertyValueFast(pathPart);
		        if (o == null && index != parts.Length - 1) throw new NullReferenceException();
		        src = o;
	        }

	        var propertyName = parts.Last();
			src.SetPropertyValueFast(propertyName,value);

        }
		public static TResult GetPropertyValueFast<TType,TResult>(this TType src,Expression<Func<TType,TResult>> expression)
        {
	        return (TResult) src.GetPropertyValueFastFromPropertyPath(expression.GetPropertyPath());
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="propertyPath">Path to property separated by '.'''</param>
		/// <returns></returns>
		public static object GetPropertyValueFastFromPropertyPath(this object src, string propertyPath)
		{
			object o = null;
			var parts = propertyPath.Split('.');
			for (var index = 0; index < parts.Length; index++)
			{
				var pathPart = parts[index];
				o = src.GetPropertyValueFast(pathPart);
				if (o == null && index!=parts.Length-1) throw new NullReferenceException();
				src = o;
			}

			return o;
		}

		private static readonly ConcurrentDictionary<PropertyInfo,object>PropertyExpressions=new ConcurrentDictionary<PropertyInfo, object>();
		public static object GetPropertyExpression(this PropertyInfo propertyInfo)
		{
			return PropertyExpressions.GetOrAdd(propertyInfo,(p) =>
			{
				var parameter = Expression.Parameter(propertyInfo.DeclaringType);
				var memberExpression = Expression.Property(parameter, propertyInfo.Name);
				var lambdaExpression = Expression.Lambda(memberExpression, parameter).Compile();
				return lambdaExpression;
			});
		}

		private interface IPropertyCallAdapter
		{
			object InvokeGet(object @this);
			void InvokeSet(object @this, object value);
			bool HasGetter { get; }
			bool HasSetter {get;}
		}
		private class PropertyCallAdapter<TThis,TResult> : IPropertyCallAdapter
		{
			private readonly Func<TThis, TResult> _getterInvocation;
			private readonly Action<TThis, TResult> _setterInvocation;

			public bool HasGetter => _getterInvocation != null;
			public bool HasSetter => _setterInvocation != null;

			public PropertyCallAdapter(Func<TThis, TResult> getterInvocation, Action<TThis, TResult> setterInvocation)
			{
				_getterInvocation = getterInvocation;
				_setterInvocation = setterInvocation;
			}

			public object InvokeGet(object @this)
			{
				return _getterInvocation.Invoke((TThis)@this);
			}

			private static readonly ConcurrentDictionary<Type, MethodInfo> CachedTypedStringValueFrom =
				new ConcurrentDictionary<Type, MethodInfo>();

			public void InvokeSet(object @this, object value)
			{
				if (!(value is TResult))
				{
					if (typeof(TResult).IsEnum)
					{
						var s = string.IsNullOrWhiteSpace((string)value) ? default(TResult).ToString() : (string) value;
						value = Enum.Parse(typeof(TResult), s, true);
					}
					else if (typeof(TResult).IsNullable() && typeof(TResult).GenericTypeArguments.Single().IsEnum)
					{
						var s = string.IsNullOrWhiteSpace((string)value) ? typeof(TResult).GetDefault()?.ToString() : (string) value;
						var innerType = typeof(TResult).GenericTypeArguments.Single();
						value = s!=null? Enum.Parse(innerType, s, true):null;
					}
					else if (typeof(TypedStringValue).IsAssignableFrom(typeof(TResult)))
					{

						var fromMethod = CachedTypedStringValueFrom.GetOrAdd(typeof(TResult), t =>
						{
							return typeof(TypedStringValue<>).MakeGenericType(typeof(TResult))
								.GetMethod("From", new[] {typeof(string)});
						});
						value=(TResult) fromMethod.Invoke(null,new[]{value});
					}
					else
					{
						if (value != null)
						{
							value = (TResult) Convert.ChangeType(value, typeof(TResult),CultureInfo.InvariantCulture);
						}
					}
				}
				_setterInvocation.Invoke((TThis)@this,(TResult)value);
			}
		}

		private static class PropertyCallAdapterProvider
		{
			private static readonly ConcurrentDictionary<int, IPropertyCallAdapter> Instances =
				new ConcurrentDictionary<int, IPropertyCallAdapter>();

			public static IPropertyCallAdapter GetInstance(Type forType,string forPropertyName,bool failIfNotFound=true)
			{
				var key = $"{forPropertyName}-{forType.FullName}".GetHashCode() *397;
				return Instances.GetOrAdd(key, (n) =>
				{

						var property = forType.GetProperty(
							forPropertyName,
							BindingFlags.IgnoreCase|BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

						MethodInfo getMethod;
						Delegate getterInvocation = null;
						MethodInfo setMethod;
						Delegate setterInvocation = null;
						if (property != null)
						{
							if ((getMethod = property.GetGetMethod(true)) != null)
							{
								var openGetterType = typeof(Func<,>);
								var concreteGetterType = openGetterType
									.MakeGenericType(forType, property.PropertyType);

								getterInvocation =
									Delegate.CreateDelegate(concreteGetterType, null, getMethod);
							}
							if ((setMethod = property.GetSetMethod(true)) != null)
							{
								var openSetterType = typeof(Action<,>);
								var concreteSetterType = openSetterType
									.MakeGenericType(forType, property.PropertyType);

								setterInvocation =
									Delegate.CreateDelegate(concreteSetterType, null, setMethod);
							}
						}
						else
						{
							if (failIfNotFound)
							{
								throw new InvalidOperationException($"{forType} does not contain any property by the name :{forPropertyName}");
							}

							return null;
						}

						var openAdapterType = typeof(PropertyCallAdapter<,>);
						var concreteAdapterType = openAdapterType
							.MakeGenericType(forType, property.PropertyType);
						return Activator.CreateInstance(concreteAdapterType, getterInvocation,setterInvocation)
							as IPropertyCallAdapter;

				});
			}

			private static readonly ConcurrentDictionary<int, PropertyInfo[]> Properties=new ConcurrentDictionary<int, PropertyInfo[]>();
			public static PropertyInfo[] GetProperties(Type forType, BindingFlags bindingFlags)
			{
				var key = $"{forType.FullName}+{(int)bindingFlags}".GetHashCode() * 397;
				return Properties.GetOrAdd(key, forType.GetProperties(bindingFlags));
			}



			

		}


	}
}
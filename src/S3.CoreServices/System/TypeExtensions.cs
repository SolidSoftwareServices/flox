using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace S3.CoreServices.System
{
	public static class TypeExtensions
	{
		public static bool IsStatic(this Type target)
		{
			return target.GetConstructor(Type.EmptyTypes) == null && target.IsAbstract && target.IsSealed;
		}

		public static bool IsCompilerGenerated(this Type target)
		{
			return null != Attribute.GetCustomAttribute(target, typeof(CompilerGeneratedAttribute));
		}
		
		

		public static bool ImplementsOpenGeneric(this Type target, Type openGenericType)
		{
			return
				target == openGenericType ||
				target.IsGenericType && target.GetGenericTypeDefinition() == openGenericType ||
				target.GetInterfaces().Any(i => i.IsGenericType && i.ImplementsOpenGeneric(openGenericType));
		}

		public static bool Implements(this Type target, Type interfaceType)
		{
			return interfaceType.IsAssignableFrom(target);
		}
		public static bool Implements<TInterface>(this Type target)
		{
			return target.Implements(typeof(TInterface));
		}
		public static bool ExtendsOpenGeneric(this Type target, Type openGenericType)
		{
			return
				target == openGenericType ||
				target.IsGenericType && target.GetGenericTypeDefinition() == openGenericType ||
				target.BaseTypes().Any(i => i.IsGenericType && i.GetGenericTypeDefinition()==openGenericType);
		}

		public static IEnumerable<Type> BaseTypes(this Type type)
		{
			var result=new List<Type>();

			var actual = type;
			while (actual.BaseType != null)
			{
				result.Add(actual.BaseType);
				actual = actual.BaseType;
			}
			return result;
		}

		
		public static bool IsAnonymous(this Type type)
		{
			return type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any() &&
			       type.FullName.Contains("AnonymousType");

		}

		public static bool IsNullable(this Type type)
		{
			return Nullable.GetUnderlyingType(type)!=null;
		}

		private static readonly Type[] _NumericTypes =
		{
			typeof(byte),
			typeof(sbyte),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(decimal),
			typeof(double),
			typeof(float),
		};
		private static readonly Type[] _NumericTypesWithNullable =
		{
			typeof(byte),
			typeof(sbyte),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(byte?),
			typeof(sbyte?),
			typeof(ushort?),
			typeof(uint?),
			typeof(ulong?),
			typeof(short?),
			typeof(int?),
			typeof(long?),
			typeof(decimal?),
			typeof(double?),
			typeof(float?),
		};

		public static bool IsNumeric(this Type src,bool includeNullable=false)
		{
			return !includeNullable && _NumericTypes.Contains(src)
			       || includeNullable && _NumericTypesWithNullable.Contains(src);

		}

		private static readonly ConcurrentDictionary<Type, object> typeDefaults =
			new ConcurrentDictionary<Type, object>();
		
		public static object GetDefault(this Type type)
		{
			return type.IsValueType
				? typeDefaults.GetOrAdd(type, Activator.CreateInstance)
				: null;
		}

	}
}
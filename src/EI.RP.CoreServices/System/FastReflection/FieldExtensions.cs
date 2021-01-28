using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace EI.RP.CoreServices.System.FastReflection
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class FieldExtensions
	{

		
		public static void SetFieldValueFast<TTarget, TValue>(this TTarget target, string fieldName,TValue newValue,BindingFlags bindingFlags= BindingFlags.Instance | BindingFlags.NonPublic )
		{
			TypeSetter<TTarget, TValue>.Instance.Set(target, fieldName, newValue,bindingFlags);
			
		}

		private class TypeSetter<TTarget, TValue>
		{
			public static readonly TypeSetter<TTarget,TValue> Instance=new TypeSetter<TTarget, TValue>();

			private TypeSetter()
			{
			}

			private  readonly ConcurrentDictionary<string, Action<TTarget, TValue>> _cacheSet= new ConcurrentDictionary<string, Action<TTarget, TValue>>();

			public void Set(TTarget target, string fieldName, TValue newValue, BindingFlags bindingFlags)
			{
				if (target == null) throw new ArgumentNullException(nameof(target));
				var type = target.GetType();
				_cacheSet.GetOrAdd($"{type.FullName}.{fieldName}.{bindingFlags}", k =>
				{
					var field = type.GetField(fieldName,bindingFlags );
					if (field == null) throw new ArgumentException(nameof(fieldName));
					var targetExp = Expression.Parameter(type, "target");
					var valueExp = Expression.Parameter(typeof(TValue), "value");

					var fieldExp = Expression.Field(targetExp, field);
					var assignExp = Expression.Assign(fieldExp, valueExp);

					return Expression.Lambda<Action<TTarget, TValue>>
						(assignExp, targetExp, valueExp).Compile();
				})(target, newValue);
			}
		}
		
	}
}
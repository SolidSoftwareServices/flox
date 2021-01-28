using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EI.RP.CoreServices.Linq;
using EI.RP.CoreServices.OData.Client.Infrastructure.Extensions;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.CoreServices.System.FastReflection;
using EI.RP.CoreServices.System.Structs;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Tracker
{
	internal class ChangesTracker : IChangesTracker
	{
		private static readonly ConcurrentDictionary<Type, string[]> EnumerableCachedPropertyNames =
			new ConcurrentDictionary<Type, string[]>();

		private static readonly ConcurrentDictionary<Type, string[]> NotEnumerableCachedPropertyNames =
			new ConcurrentDictionary<Type, string[]>();

		private readonly IODataClientSettings _settings;

		private readonly ConcurrentDictionary<object, object> _trackedEntities =
			new ConcurrentDictionary<object, object>();

		public ChangesTracker(IODataClientSettings settings)
		{
			_settings = settings;
		}

		public void Detach<TDto>(TDto changedEntity) where TDto : class
		{
			_trackedEntities.TryRemove(changedEntity, out var val);
		}

		public IDictionary<string, object> GetChanges<TDto>(TDto changedEntity) where TDto : ODataDtoModel
		{
			var dictionaryOfChangedValues = changedEntity.ToDictionaryOfChangedValues(OriginalOf(changedEntity));
			return dictionaryOfChangedValues;
		}

		public void TrackInstance<TDto>(TDto updateable) where TDto : class
		{
			if (updateable != null)
			{
				var trackableUpdatables = updateable.Traverse(GetTrackableChildren)
					.ToArray();
				foreach (var trackableUpdatable in trackableUpdatables)
				{
					var original = trackableUpdatable.CloneDeep();
					Attach(original, trackableUpdatable);
				}
			}
		}

		private TDto OriginalOf<TDto>(TDto changedEntity) where TDto : class
		{
			_trackedEntities.TryGetValue(changedEntity, out var originalDto);
			return (TDto) originalDto;
		}

		private IEnumerable<object> GetTrackableChildren<TDto>(TDto target) where TDto : class
		{
			var trackableChildren = Get(EnumerableCachedPropertyNames, p =>
					p.PropertyType.ImplementsOpenGeneric(typeof(IEnumerable<>)), x => (x as IEnumerable).Cast<object>())
				.Union(Get(NotEnumerableCachedPropertyNames,
					p => !p.PropertyType.ImplementsOpenGeneric(typeof(IEnumerable<>)), x => x.ToOneItemArray()));
			return trackableChildren;

			object[] Get(ConcurrentDictionary<Type, string[]> cache, Expression<Func<PropertyInfo, bool>> resolver,
				Func<object, IEnumerable<object>> valueSelector)
			{
				var selectorExpression = (Func<PropertyInfo, bool>)
					(propertyInfo => propertyInfo.CanRead
					                 && propertyInfo.CanWrite
					                 && propertyInfo.PropertyType.Namespace != null
					                 && (
						                 propertyInfo.PropertyType.Namespace.StartsWith(_settings.ODataDtosNamespaceRoot)
										|| propertyInfo.PropertyType.ImplementsOpenGeneric(typeof(IEnumerable<>)) 
						                 && propertyInfo.PropertyType.GenericTypeArguments.Length == 1 
										&& propertyInfo.PropertyType.GenericTypeArguments[0].Namespace.StartsWith(_settings.ODataDtosNamespaceRoot)
					                  ));
				var enumerablePropertyNames = cache.GetOrAdd(target.GetType(), t =>
				{
					return target.GetPropertiesFast(BindingFlags.Instance | BindingFlags.Public,
							PredicateBuilder
								.ConfigurePredicateFor<PropertyInfo>()
								.And(selectorExpression.ToExpression())
								.And(resolver)
								.Compile())
						.Select(x => x.Name).ToArray();
				});

				var result = target.GetPropertyValuesFast(enumerablePropertyNames)
					.Where(x => x != null)
					.SelectMany(valueSelector)
					.ToArray();
				return result;
			}
		}


		private void Attach<TDto>(TDto original, TDto updateable) where TDto : class
		{
			if (original == null) throw new ArgumentNullException(nameof(original));
			if (updateable == null) throw new ArgumentNullException(nameof(updateable));

			if (!_trackedEntities.TryAdd(updateable, original) && ReferenceEquals(OriginalOf(updateable), original))
			{
				throw new InvalidOperationException("There is another original attached to the same key");
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ei.Rp.Mvc.Core.System.Request
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class QueryCollectionExtensions
	{
		private static readonly string[] Exclusions = {"action", "controller", "init"};
		public static ExpandoObject ToExpandoObject(this RouteValueDictionary routeValues, IQueryCollection requestQuery)
		{

			var src= routeValues.Where(x => x.Key != null && !x.Key.StartsWith(Prefixes.ContainerProperty) && !Exclusions.Contains(x.Key.ToLower()))
				.Select(x=>new KeyValuePair<string,object>(x.Key?.ToLowerInvariant(),x.Value?.ToString().ToLowerInvariant()));
			if (requestQuery != null)
			{
				src = src.Union(requestQuery.ToDynamicObject()).SelectSingleValueByKey();
			}

			var dictionary = src.ToDictionary(x => x.Key?.ToLowerInvariant(), x => (object) x.Value?.ToString().ToLowerInvariant());
			return BuildExpandoObject(dictionary);
		}

		private static IEnumerable<KeyValuePair<string, object>> ToDynamicObject(this IQueryCollection requestQuery)
		{
			return requestQuery
				.Where(x => x.Key != null && !x.Key.StartsWith(Prefixes.ContainerProperty) &&
				            !Exclusions.Contains(x.Key.ToLower()))
				.Select(x =>
					new KeyValuePair<string, object>(x.Key?.ToLower(),
						(object) x.Value.First()?.ToString().ToLower()));
			
				
			
		}
		public static ExpandoObject ToExpandoObject(this IQueryCollection requestQuery)
		{
			return (ExpandoObject)BuildExpandoObject(requestQuery.ToDynamicObject().ToDictionary(x => x.Key, x => x.Value));



		}
		private static IEnumerable<KeyValuePair<string, object>> SelectSingleValueByKey(
			this IEnumerable<KeyValuePair<string, object>> src)
		{
			return src.GroupBy(x => x.Key)
				.Select(g =>
				{
					var values = g.Select(x => x.Value).ToArray();
					var first = values.First();
#if DEBUG
						if (!values.Where(x=>!string.IsNullOrEmpty(x as string)).All(x => x == first))
						{
							throw new InvalidOperationException(
								$"{g.Key} has different values. This indicates an error");
						}
#endif
					return new KeyValuePair<string, object>(g.Key, first);
				})
				.ToArray();
		}


		public static ExpandoObject ToDynamicObjectOfContainerProperties(this RouteValueDictionary routeValues)
		{
			var dictionary = routeValues.Where(x => x.Key != null && x.Key.StartsWith(Prefixes.ContainerProperty))
				.ToDictionary(x => x.Key, x => (object)x.Value?.ToString());
			return BuildExpandoObject(dictionary);
		}
		private static ExpandoObject BuildExpandoObject(IDictionary<string, object> dictionary)
		{
			var expandoObject = new ExpandoObject();
			var collection = (ICollection<KeyValuePair<string, object>>)expandoObject;

			foreach (var kvp in dictionary)
			{
				collection.Add(kvp);
			}

			return expandoObject;
		}

	}
}

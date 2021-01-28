using System;
using System.Collections.Generic;

namespace S3.CoreServices.System.Structs
{
	public static class GraphExtensions
	{
		/// <summary>
		/// It traverses and object graph
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="parent"></param>
		/// <param name="children"></param>
		/// <returns></returns>
		public static IEnumerable<object> Traverse<TObject>(this TObject parent, Func<object, IEnumerable<object>> children)
		{
			var seen = new HashSet<object>();
			var stack = new Stack<object>();
			seen.Add(parent);
			stack.Push(parent);
			yield return parent;
			while (stack.Count > 0)
			{
				var current = stack.Pop();
				var childItems = children(current);
				foreach (var newItem in childItems)
				{
					if (!seen.Contains(newItem))
					{
						seen.Add(newItem);
						stack.Push(newItem);
						yield return newItem;
					}
				}
			}
		}
	}
}
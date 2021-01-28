using System;
using System.Collections.Generic;
using System.Linq;

namespace S3.CoreServices.Linq
{
	public static class TreeExtensions
	{
		public static IEnumerable<TNode> Descendants<TNode>(this TNode root,Func<TNode,IEnumerable<TNode>>nodeChildrenResolver)
		{
			var nodes=new Stack<TNode>(new []{root});
			while (nodes.Any())
			{
				var node = nodes.Pop();
				yield return node;
				foreach (var childNode in nodeChildrenResolver(node))
				{
					nodes.Push(childNode);
				}
			}
		}
	}
}
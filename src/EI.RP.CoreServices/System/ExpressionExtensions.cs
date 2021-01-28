using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EI.RP.CoreServices.System
{
	public static class ExpressionExtensions{
		public static Expression<Func<T, TResult>> ToExpression<T, TResult>(this Func<T, TResult> func)
		{
			return x => func(x);
		}

		public static string GetPropertyName<TSource, TField>(this Expression<Func<TSource, TField>> propertyExpression)
		{
			if (object.Equals(propertyExpression, null))
			{
				throw new NullReferenceException("Field is required");
			}

			MemberExpression expr = null;

			if (propertyExpression.Body is MemberExpression)
			{
				expr = (MemberExpression)propertyExpression.Body;
			}
			else if (propertyExpression.Body is UnaryExpression)
			{
				expr = (MemberExpression)((UnaryExpression)propertyExpression.Body).Operand;
			}
			else
			{
				throw new ArgumentException($"Expression '{propertyExpression}' not supported.");
			}

			return expr.Member.Name;
		}



		public static string GetPropertyNameEx<T>(this Expression<Func<T,object>> expression)
		{
			var stack = new Stack<string>();
			Expression expression1 = expression.Body;
			while (expression1 != null)
			{
				if (expression1.NodeType == ExpressionType.Call)
				{
					var methodCallExpression = (MethodCallExpression)expression1;
					if (IsSingleArgumentIndexer(methodCallExpression))
					{
						stack.Push(string.Empty);
						expression1 = methodCallExpression.Object;
					}
					else
						break;
				}
				else if (expression1.NodeType == ExpressionType.ArrayIndex)
				{
					var binaryExpression = (BinaryExpression)expression1;
					stack.Push(string.Empty);
					expression1 = binaryExpression.Left;
				}
				else if (expression1.NodeType == ExpressionType.MemberAccess)
				{
					var memberExpression = (MemberExpression)expression1;
					stack.Push("." + memberExpression.Member.Name);
					expression1 = memberExpression.Expression;
				}
				else if (expression1.NodeType == ExpressionType.Parameter)
				{
					stack.Push(string.Empty);
					expression1 = null;
				}
				else if (expression1.NodeType == ExpressionType.Convert)
				{
					var memberExp = ((UnaryExpression)expression1).Operand as MemberExpression;
					stack.Push("." + memberExp.Member.Name);
					expression1 = memberExp.Expression;
				}
				else
					break;
			}
			if (stack.Count > 0 && string.Equals(stack.Peek(), ".model", StringComparison.OrdinalIgnoreCase))
				stack.Pop();
			if (stack.Count <= 0)
				return string.Empty;
			return (stack).Aggregate(((left, right) => left + right)).TrimStart(new[] { '.' });
		}

		private static bool IsSingleArgumentIndexer(Expression expression)
		{
			var methodExpression = expression as MethodCallExpression;
			if (methodExpression == null || methodExpression.Arguments.Count != 1)
				return false;
			return (methodExpression.Method.DeclaringType.GetDefaultMembers()).OfType<PropertyInfo>().Any((p => p.GetGetMethod() == methodExpression.Method));
		}

		public static IReadOnlyList<PropertyInfo> GetExpressionProperties<TSource,TResult>(this Expression<Func<TSource, TResult>> expression)
		{
			var visitor = new PropertyVisitor();
			visitor.Visit(expression.Body);
			visitor.Path.Reverse();
			return visitor.Path.Distinct().ToArray();
		}

		private class PropertyVisitor : ExpressionVisitor
		{
			internal readonly List<PropertyInfo> Path = new List<PropertyInfo>();

			protected override Expression VisitMember(MemberExpression node)
			{
				if (node.Member is PropertyInfo)
				{
					this.Path.Add((PropertyInfo)node.Member);
				}

				return base.VisitMember(node);
			}
		}

	}


}
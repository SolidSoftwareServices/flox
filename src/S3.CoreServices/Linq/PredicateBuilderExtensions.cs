using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace S3.CoreServices.Linq
{
    public static class PredicateBuilderExtensions
    {
        public static Expression<Func<TModel, TResult>> Or<TModel, TResult>(
            this Expression<Func<TModel, TResult>> expr1,
            Expression<Func<TModel, TResult>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<TModel, TResult>>
                (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }


        public static Expression<Func<TModel, TResult>> And<TModel, TResult>(
            this Expression<Func<TModel, TResult>> expr1,
            Expression<Func<TModel, TResult>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<TModel, TResult>>
                (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
        public static Expression<Func<TModel, bool>> Like<TModel>(this string propertyName, string value)
        {
            var propertyInfo = typeof(TModel).GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

            var param = Expression.Parameter(typeof(TModel), "t");
            var member = Expression.Property(param, propertyInfo.Name);

            var startWith = value.StartsWith("*");
            var endsWith = value.EndsWith("*");

            if (startWith)
                value = value.Remove(0, 1);

            if (endsWith)
                value = value.Remove(value.Length - 1, 1);

            var constant = Expression.Constant(value);
            Expression exp;

            if (endsWith && startWith)
            {
                exp = Expression.Call(member, ContainsMethod, constant);
            }
            else if (startWith)
            {
                exp = Expression.Call(member, EndsWithMethod, constant);
            }
            else if (endsWith)
            {
                exp = Expression.Call(member, StartsWithMethod, constant);
            }
            else
            {
                exp = Expression.Equal(member, constant);
            }

            return Expression.Lambda<Func<TModel, bool>>(exp, param);
        }
      
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

    }
}
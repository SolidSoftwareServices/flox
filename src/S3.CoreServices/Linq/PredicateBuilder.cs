using System;
using System.Linq.Expressions;

namespace S3.CoreServices.Linq
{
    public class PredicateBuilder
    {
        public static Expression<Func<T, bool>> ConfigurePredicateFor<T>()
        {
            return f => true;
        }
    }
}

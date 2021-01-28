using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

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

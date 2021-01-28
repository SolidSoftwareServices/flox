using System;
using System.Linq;
using System.Linq.Expressions;

namespace EI.RP.CoreServices.Ports.OData
{
	public static class ODataFunctionExtensions
	{
		public static TFunction Expand<TFunction, TFunctionResultDto>(this TFunction function,
			Expression<Func<TFunctionResultDto, object>> expandExpression)
			where TFunction : ODataFunction<TFunctionResultDto>
		{
			if (expandExpression == null) throw new ArgumentNullException(nameof(expandExpression));

			var lst=function.Expands.ToList();
			lst.Add(expandExpression);
			function.Expands = lst;

			return function;
		}
	}
}
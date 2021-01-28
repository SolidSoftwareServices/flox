using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EI.RP.CoreServices.System;

namespace EI.RP.CoreServices.Ports.OData
{
	public abstract class ODataFunction<TResult>: EntityContainerItem
	{
		protected ODataFunction(string functionName, params Expression<Func<TResult, object>>[] expands)
		{
			if (string.IsNullOrWhiteSpace(functionName))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(functionName));
			FunctionName = functionName;
			Expands = expands ?? new Expression<Func<TResult, object>>[0];
		}

		public abstract object QueryAsObject { get; }
		public string FunctionName { get; }
		public IEnumerable<Expression<Func<TResult, object>>> Expands { get; internal set; }
		public int? Top { get; protected set; }
		public abstract bool ReturnsComplexType();
		public abstract bool ReturnsCollection();

	}



	public abstract class ODataFunction<TQuery, TResult>: ODataFunction<TResult>
		where TQuery : new()
	{
		protected ODataFunction(string functionName, params Expression<Func<TResult, object>>[] expands) :base(functionName, expands)
		{
			if (string.IsNullOrWhiteSpace(functionName))
				throw new ArgumentException("Value cannot be null or whitespace.", nameof(functionName));
		}

		

		public TQuery Query { get; private set; } = new TQuery();

		public override object QueryAsObject => Query;


	}
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using EI.RP.CoreServices.System.FastReflection;

namespace EI.RP.UiFlows.Core.Flows.Screens.Models
{
	public static class ScreenModelExtensions
	{
		/// <summary>
		/// </summary>
		/// <typeparam name="TStepData"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="stepData"></param>
		/// <param name="customizations"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static bool SetFlowCustomizableValue<TStepData, TValue>(this TStepData stepData,
			IDictionary<string, object> customizations, Expression<Func<TStepData, TValue>> expression)
			where TStepData : UiFlowScreenModel
		{
			var result = false;

			var key = expression.GetPropertyPath().Split('.').LastOrDefault()?.ToLower();

            if (key != null && customizations.ContainsKey(key))
			{
				var requestedValue =
					(TValue) Convert.ChangeType(customizations[key], typeof(TValue), CultureInfo.InvariantCulture);

				var value = stepData.GetPropertyValueFast(expression);

				if (value != null && requestedValue == null || value == null || !value.Equals(requestedValue))
				{
					stepData.SetPropertyValueFast(expression, requestedValue);
					result = true;
				}
			}

			return result;
		}
	}
}
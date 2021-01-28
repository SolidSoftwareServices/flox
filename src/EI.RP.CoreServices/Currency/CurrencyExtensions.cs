using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EI.RP.CoreServices.Currency
{
	public static class CurrencyExtensions
	{
		public static string ToCurrencyString(this decimal amount)
		{
			return amount.ToString("C",CultureInfo.GetCultureInfo("en-IE"));
		}
	}
}

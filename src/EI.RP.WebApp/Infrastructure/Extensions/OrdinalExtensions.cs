﻿using System;

namespace EI.RP.WebApp.Infrastructure.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToOccurrenceSuffix(this int integer)
        {
            switch (integer % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }
            switch (integer % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }

        public static string ToOccurrenceSuffix(this int? integer)
        {
	        if (integer == null) return string.Empty;

	        switch (integer % 100)
	        {
		        case 11:
		        case 12:
		        case 13:
			        return "th";
	        }
	        switch (integer % 10)
	        {
		        case 1:
			        return "st";
		        case 2:
			        return "nd";
		        case 3:
			        return "rd";
		        default:
			        return "th";
	        }
        }

        public static string ToOrdinal(this int integer)
        {
	        return $"{ integer }{ integer.ToOccurrenceSuffix() }";
        }

        public static string ToOrdinal(this int? integer)
        {
            return $"{ (integer.HasValue ? integer.ToString() : string.Empty) }{ integer.ToOccurrenceSuffix() }";
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime dateTime, string format, bool useExtendedSpecifiers)
        {
            return useExtendedSpecifiers
                ? dateTime.ToString(format)
                    .Replace("nn", dateTime.Day.ToOccurrenceSuffix().ToLower())
                    .Replace("NN", dateTime.Day.ToOccurrenceSuffix().ToUpper())
                : dateTime.ToString(format);
        }
    }
}

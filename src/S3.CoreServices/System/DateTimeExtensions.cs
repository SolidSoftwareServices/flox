using System;
using System.Collections.Generic;
using System.Linq;

namespace S3.CoreServices.System
{
	public static class DateTimeExtensions
	{
		//TODO: review these parameters....
		public static string ToDisplayDate(this DateTime dt, bool fullMonthFormat = true, bool includeDateYear = false)
		{
			var prefix = dt.Day.ToString().Trim().Length == 1 ? '0' : dt.Day.ToString().TrimStart('0').First();
			var suffix = dt.Day.ToString().TrimStart('0').Last();
			var day = dt.Day.ToString().TrimStart('0');

			switch (suffix)
			{
				case '1':
					day = day + (prefix.Equals('1') ? "th" : "st");
					break;
				case '2':
					day = day + (prefix.Equals('1') ? "th" : "nd");
					break;
				case '3':
					day = day + (prefix.Equals('1') ? "th" : "rd");
					break;
				default:
					day = day + "th";
					break;
			}
			string month = null;
			month = fullMonthFormat ? $"{dt:MMMM}" : $"{dt:MMM}";
			var year = dt.Year.ToString();
			var date = default(string);
			if (includeDateYear == true)
				date = $"{day} {month}";

			else
				date = $"{day} {month} {year}";
			return date;
		}

		public static DateTime FirstDayOfNextMonth(this DateTime dt, int? onlyIfDayBiggerThan = null)
		{
			if (onlyIfDayBiggerThan == null || dt.Day > onlyIfDayBiggerThan.Value)
			{
				var oneMoreMonth = dt.AddMonths(1);
				dt = new DateTime(oneMoreMonth.Year, oneMoreMonth.Month, 1);
			}

			return dt;
		}

		public static DateTime LastDayOfTheMonth(this DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
		}
		public static DateTime FirstDayOfTheMonth(this DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, 1);
		}
		public static DateTime LastDayOfTheYear(this DateTime dt)
		{
			return new DateTime(dt.Year, 12, 31);
		}
		public static DateTime FirstDayOfTheYear(this DateTime dt)
		{
			return new DateTime(dt.Year, 1, 1);
		}
		public static DateTime LastTimeOfTheYear(this DateTime dt)
		{
			return dt.FirstDayOfTheYear().AddYears(1).AddTicks(-1);
		}

		public static DateTime LastTimeOfTheDay(this DateTime dt)
		{
			return dt.Date.AddDays(1).AddTicks(-1);
		}
		public static DateTime ToUnspecified(this DateTime dt)
		{
			return DateTime.SpecifyKind(dt.ToLocalTime(), DateTimeKind.Unspecified);
		}
		public static  DateTime FirstDayOfWeek(this DateTime date,DayOfWeek firstDayOfWeek=DayOfWeek.Sunday)
		{
			
			var offset = firstDayOfWeek - date.DayOfWeek;
			return date.AddDays(offset);
		}

		public static DateTime LastDayOfWeek(this DateTime date, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
		{
			var lastDayOfWeek = FirstDayOfWeek(date,firstDayOfWeek).AddDays(6);
			return new DateTime(lastDayOfWeek.Year, lastDayOfWeek.Month, lastDayOfWeek.Day);
		}

		public static int GetBimonthlyGroupIndex(this DateTime date)
		{
			var group = Math.Ceiling(date.Month / (double)2);
			return (int)group;
		}

		public static DateTimeRange GetYearRange(this DateTime date)
		{
			return new DateTimeRange(date.FirstDayOfTheYear(),date.LastDayOfTheYear());
		}

		public static IEnumerable<DateTime> GetDatesGreaterThanDay(this DateTimeRange dtr, int day) 
		{
			var result = new List<DateTime>();

			for (var date = dtr.Start; date <= dtr.End; date = date.AddDays(1)) 
			{
				if (date.Date.Day > day) result.Add(date);
			}

			return result;
		}
	}
}
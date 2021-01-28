using System;
using System.Collections.Generic;
using System.Linq;

namespace EI.RP.CoreServices.System
{
	public static class DateTimeRangeExtensions
	{
		public static IEnumerable<DateTimeRange> MergeConsecutivePeriods(this IEnumerable<DateTimeRange> src,
			DateTimeRangeUnitPrecision precision = DateTimeRangeUnitPrecision.Day)
		{
			if (precision != DateTimeRangeUnitPrecision.Day) throw new NotSupportedException();
			if (src == null) return null;
			var sortedSrc = src.MergeOverlappedPeriods().OrderBy(x => x.Start).ToArray();
			if (!sortedSrc.Any()) return sortedSrc;

			var stack = new Stack<DateTimeRange>();
			stack.Push(sortedSrc[0]);
			for (int i = 1; i < sortedSrc.Length; i++)
			{
				var top = stack.Peek();

				var current = sortedSrc[i];
				if (top.End.AddDays(1) != current.Start)
				{
					stack.Push(current);
				}
				else 
				{
					top = stack.Pop();
					top = new DateTimeRange(top.Start, current.End);
					stack.Push(top);
				}
			}

			return stack.OrderBy(x=>x.Start).ToArray();
		}

		public static IEnumerable<DateTimeRange> MergeOverlappedPeriods(this IEnumerable<DateTimeRange> src)
		{
			
			if (src == null || !src.Any()) return src;

			var sortedSrc = src.OrderBy(x => x.Start).ToArray();
			
			var stack =new Stack<DateTimeRange>();
			stack.Push(sortedSrc[0]);

			for (int i = 1; i < sortedSrc.Length; i++)
			{
				var top = stack.Peek();

				var current = sortedSrc[i];
				if (top.End < current.Start)
				{
					stack.Push(current);
				}
				else if (top.End < current.End)
				{
					top = stack.Pop();
					top=new DateTimeRange(top.Start,current.End);
					stack.Push(top);
				}
			}

			return stack.OrderBy(x => x.Start).ToArray();
		}
	}

	public enum DateTimeRangeUnitPrecision
	{
		Day,
		
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace EI.RP.CoreServices.System
{
	public class DateTimeRange : IEquatable<DateTimeRange>
	{
		[JsonProperty]
		public DateTime Start { get; private set; }
		[JsonProperty]
		public DateTime End { get; private set; }

		[JsonConstructor]
		private DateTimeRange(){}

		public static DateTimeRange YearRangeOf(DateTime date)
		{
			return new DateTimeRange(date.FirstDayOfTheYear(), date.LastTimeOfTheYear());
		}

		public DateTimeRange(DateTime start, TimeSpan duration) : this(start, start.Add(duration)) { }

		public DateTimeRange(DateTime start, DateTime end)
		{
			if (start > end) throw new ArgumentOutOfRangeException(nameof(start), "Range boundaries are invalid");
			if (start == end) throw new ArgumentException("Not a range");
			Start = start;
			End = end;
		}

		public static DateTimeRange CurrentYear =>
			new DateTimeRange(DateTime.Today.FirstDayOfTheYear(), DateTime.Today.LastDayOfTheYear());

		public static DateTimeRange CurrentMonth=>new DateTimeRange(DateTime.Today.FirstDayOfTheMonth(), DateTime.Today.LastDayOfTheMonth());

		public bool Intersects(DateTimeRange other)
		{
			if (this.Start > this.End || other.Start > other.End)
				throw new ArgumentOutOfRangeException();

			
			if (this.Start == other.Start || this.End == other.End)
				return true; 

			if (this.Start < other.Start)
			{
				if (this.End > other.Start && this.End < other.End)
					return true; 

				if (this.End > other.End)
					return true; 
			}
			else
			{
				if (other.End > this.Start && other.End < this.End)
					return true; 

				if (other.End > this.End)
					return true; 
			}

			return false;
		}

		public IEnumerable<DateTime> SplitInBiMonthlyDataPoints()
		{
			return new[]
			{
			new DateTime(Start.Year,2,1).LastDayOfTheMonth(),
			new DateTime(Start.Year,4,1).LastDayOfTheMonth(),
			new DateTime(Start.Year,6,1).LastDayOfTheMonth(),
			new DateTime(Start.Year,8,1).LastDayOfTheMonth(),
			new DateTime(Start.Year,10,1).LastDayOfTheMonth(),
			new DateTime(Start.Year,12,1).LastDayOfTheMonth(),
			};
		}
		public IEnumerable<DateTime> SplitInMonthlyDataPoints(bool useLastDayOfMonth= false,int? useDay=null)
		{
			var result = new HashSet<DateTime>();
			var current = Start;

			current = CorrectToLast();

			while (current <= End)
			{
				result.Add(current);
				current = current.AddMonths(1);
				CorrectToLast();
			}

			return result;

			DateTime CorrectToLast()
			{
				if (useLastDayOfMonth)
				{
					current = current.LastDayOfTheMonth();
				}
				else if (useDay.HasValue)
				{
					current=new DateTime(current.Year,current.Month,Math.Min(useDay.Value,current.LastDayOfTheMonth().Day));
				}

				return current;
			}
		}

		public IEnumerable<DateTime> SplitInDailyDataPoints()
		{
			return SplitInDataPointsByTimespan(TimeSpan.FromDays(1));
		}

		public IEnumerable<DateTime> SplitInHourlyDataPoints()
		{
			return SplitInDataPointsByTimespan(TimeSpan.FromMinutes(60));
		}

		public IEnumerable<DateTime> SplitInHalfHourlyDataPoints()
		{
			return SplitInDataPointsByTimespan(TimeSpan.FromMinutes(30));
		}

		public IEnumerable<DateTime> SplitInDataPointsByTimespan(TimeSpan ts)
		{
			var current = Start;

			while (current <= End)
			{
				var result = current;
				current = current.Add(ts);
				yield return result;
				
			}
		}

		public override string ToString()
		{
			return $"{nameof(Start)}: {Start}, {nameof(End)}: {End}";
		}

		public bool Equals(DateTimeRange other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Start.Equals(other.Start) && End.Equals(other.End);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DateTimeRange) obj);
		}

		public IEnumerable<DateTimeRange> DifferenceTo(IEnumerable<DateTimeRange> others)
		{
			throw new NotSupportedException();
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Start.GetHashCode() * 397) ^ End.GetHashCode();
			}
		}

		public static bool operator ==(DateTimeRange left, DateTimeRange right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(DateTimeRange left, DateTimeRange right)
		{
			return !Equals(left, right);
		}


		public bool Contains(DateTime dateTime)
		{
			return Start <= dateTime && End >= dateTime;
		}


	}
}
using System;
using System.Collections.Generic;
using System.Text;
using EI.RP.CoreServices.System;

namespace EI.RP.DataServices
{
	public static class SapDateTimes
	{
		public static readonly DateTime SapDateTimeMax = new DateTime(9999, 12, 31);

		public static DateTime ToSapFilterDateTime(this DateTime src)
		{
			return new DateTime(src.Year, src.Month, src.Day, src.Hour, src.Minute, src.Second, DateTimeKind.Unspecified);
		}

		public static DateTimeRange ToSapFilterDateTime(this DateTimeRange src)
		{
			return new DateTimeRange(src.Start.ToSapFilterDateTime(),src.End.ToSapFilterDateTime());
		}


	}
}

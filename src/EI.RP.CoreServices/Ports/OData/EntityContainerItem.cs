using System;

namespace EI.RP.CoreServices.Ports.OData
{
	public abstract class EntityContainerItem{
		public abstract string GetEntityContainerName() ;

		protected static DateTime CompensateSapDateTimeBug(DateTime src)
		{
			return new DateTime(src.Year, src.Month, src.Day, src.Hour, src.Minute, src.Second, DateTimeKind.Unspecified);
		}
		protected static DateTime? CompensateSapDateTimeBug(DateTime? src)
		{
			if (!src.HasValue) return null;
			return CompensateSapDateTimeBug(src.Value);
		}
	}
}
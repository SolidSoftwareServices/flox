using Ei.Rp.DomainModels.MappingValues;
using System.Collections.Generic;

namespace EI.RP.WebApp.DomainModelExtensions
{
    public static class MeterUnitExtensions
    {
        public static string ToDisplayFormat(this MeterUnit m)
        {
            var displayFormats = new Dictionary<string, string>
            {
                { MeterUnit.KWH, "kWh" },
                { MeterUnit.M3, "m³" }
            };

            return displayFormats.ContainsKey(m) ? displayFormats.GetValueOrDefault(m) : m.ToString();
        }
    }
}

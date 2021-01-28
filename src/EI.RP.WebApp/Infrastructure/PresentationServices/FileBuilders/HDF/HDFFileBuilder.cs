using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Metering.Consumption;

namespace EI.RP.WebApp.Infrastructure.PresentationServices.FileBuilders.HDF
{
    internal class HDFFileBuilder : IHDFFileBuilder
    {
        private const int HoursInADay = 24;
        private const int HalfHourInMinutes = 30;

        public async Task<byte[]> BuildFileData(AccountConsumption consumption)
        {
            if (consumption == null)
            {
                throw new ArgumentNullException(nameof(consumption));
            }

            return await GenerateHdfFile(consumption.UsageEntries.OrderBy(u => u.Date));
        }

        private async Task<byte[]> GenerateHdfFile(IEnumerable<AccountConsumption.UsageEntry> consumptionUsageEntries)
        {
            var consumptionUsageEntriesByDate = consumptionUsageEntries.GroupBy(ce => ce.Date.Date)
                .Select(gp => new { Key = gp.Key, Value = gp.OrderBy(d => d.Date).ToList() });
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.AutoFlush = true;
                    await WriteHeaderAsync(writer);
                    foreach (var consumptionUsageEntry in consumptionUsageEntriesByDate)
                    {
                        var consumptionEntry = consumptionUsageEntry.Value.First();
                        if (consumptionEntry.Date.Date == DateTime.Today.Date)
                        {
	                        continue;
                        }
                        var date = consumptionUsageEntry.Key.ToString("dd/MM/yyyy");
                        var mprn = consumptionEntry.Prn;
                        var meterSerialNumber = consumptionEntry.SerialNumber;
                        await writer.WriteAsync($"{date},{mprn},{meterSerialNumber}");
                        await WriteConsumptionData(writer, consumptionUsageEntry.Key, consumptionUsageEntry.Value);
                    }

                    return stream.ToArray();
                }
            }

            async Task WriteConsumptionData(StreamWriter writer, DateTime usageEntryDate, List<AccountConsumption.UsageEntry> usageEntries)
            {
                if (!usageEntries.Any())
                {
                    await writer.WriteLineAsync(string.Join("", Enumerable.Repeat(",", HoursInADay * 2)));
                    return;
                }

                int usageIndex = 0;
                var dayFirstEntryValue = string.Empty;
                if (usageEntries[0].Date == usageEntryDate)
                {
                    dayFirstEntryValue = usageEntries[0].Value.ToString("F2");
                    usageIndex = 1;
                }
                
                foreach (var timeEntry in Enumerable.Range(1, HoursInADay * 2 - 1).Select(r => usageEntryDate.Date.AddMinutes(r * HalfHourInMinutes)))
                {
                    if (usageIndex >= usageEntries.Count)
                    {
                        await writer.WriteAsync(",");
                        continue;
                    }

                    var usageEntry = usageEntries[usageIndex];
                    if (usageEntry.Date == timeEntry)
                    {
                        await writer.WriteAsync($",{usageEntry.Value:F2}");
                        usageIndex++;
                    }
                    else
                    {
                        await writer.WriteAsync(",");
                    }
                }

                await writer.WriteLineAsync($",{dayFirstEntryValue}");
            }

            async Task WriteHeaderAsync(StreamWriter writer)
            {
                var header = GetHeaderValues();
                await writer.WriteLineAsync(string.Join(",", header));
            }
        }

        private List<string> GetHeaderValues()
        {
            return new List<string>
            {
                "Date",
                "MPRN",
                "Meter Serial Number"
            }
            .Union(Enumerable.Range(1, HoursInADay * 2 - 1)
                .Select(r => DateTime.Today.Date.AddMinutes(r * HalfHourInMinutes).ToString("HH:mm")))
            .Union(Enumerable.Repeat("00:00", 1))
            .ToList();
        }
    }
}
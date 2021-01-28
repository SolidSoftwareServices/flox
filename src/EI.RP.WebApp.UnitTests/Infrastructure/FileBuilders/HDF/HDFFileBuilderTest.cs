using AutoFixture;
using Ei.Rp.DomainModels.Metering.Consumption;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.WebApp.Infrastructure.PresentationServices.FileBuilders.HDF;

namespace EI.RP.WebApp.UnitTests.Infrastructure.FileBuilders.HDF
{
	[TestFixture]
	internal class HDFFileBuilderTest
	{
		private const int HoursInADay = 24;
		private const int HalfHourInMinutes = 30;

		IFixture Fixture = new Fixture().CustomizeDomainTypeBuilders();

		[Test]
		public async Task NullArgumentException()
		{
			var builder = new HDFFileBuilder();
			var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await builder.BuildFileData(null));
			Assert.That(exception.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: consumption"));
		}

		[TestCase(0, TestName = "WithoutData")]
		[TestCase(1, TestName = "DataWith-1TimeEntry")]
		[TestCase(2, TestName = "DataWith-2TimeEntries")]
		[TestCase(3, TestName = "DataWith-3TimeEntries")]
		[TestCase(5, TestName = "DataWith-5TimeEntries")]
		[TestCase(10, TestName = "DataWith-10TimeEntries")]
		[TestCase(48, TestName = "DataWith-48TimeEntries")]
		public async Task BuildHdfFile(int numberTimeValueEntries)
		{
			var domainFacade = new DomainFacade();
			var mprn = Fixture.Create<ElectricityPointReferenceNumber>().ToString();
			var serialNumber = domainFacade.ModelsBuilder.Create<string>();
			int dataDays = 1;
			var usageEntries = GetUsageEntries(domainFacade, mprn, serialNumber, dataDays);
			usageEntries = usageEntries.Take(numberTimeValueEntries).ToList();
			var fileData = await GenerateDataFile(domainFacade, usageEntries);
			Assert.NotNull(fileData);
			if (numberTimeValueEntries > 0)
			{
				Assert.True(fileData.Length > 319);
			}
			else
			{
				Assert.True(fileData.Length == 319);
			}
			await AssertFileData(fileData, dataDays, mprn, serialNumber, usageEntries);
		}

		[TestCase(true, TestName = "MultipleDaysUsageEntriesWithTodayUsage")]
		[TestCase(false, TestName = "MultipleDaysUsageEntriesWithOutTodayUsage")]
		public async Task BuildHdfFileWithManyDaysData(bool usageEntriesWithTodayUsage)
		{
			var domainFacade = new DomainFacade();
			var mprn = Fixture.Create<ElectricityPointReferenceNumber>().ToString();
			var serialNumber = domainFacade.ModelsBuilder.Create<string>();
			int dataDays = 5;
			var usageEntries = GetUsageEntries(domainFacade, mprn, serialNumber, dataDays);
			if (usageEntriesWithTodayUsage)
			{
				usageEntries = usageEntries.Union(Enumerable.Range(0, HoursInADay * 2).Select(e =>
					new AccountConsumption.UsageEntry
					{
						Date = DateTime.Today.Date.AddMinutes(e * HalfHourInMinutes),
						SerialNumber = serialNumber,
						Prn = mprn,
						Value = domainFacade.ModelsBuilder.Create<decimal>()
					})).ToList();
			}

			var fileData = await GenerateDataFile(domainFacade, usageEntries);
			Assert.NotNull(fileData);
			Assert.True(fileData.Length > 319);
			await AssertFileData(fileData, dataDays, mprn, serialNumber, usageEntries);
		}

		[Test]
		public async Task BuildHdfFileWithSomeTimeSlotMissings()
		{
			var domainFacade = new DomainFacade();
			var mprn = Fixture.Create<ElectricityPointReferenceNumber>().ToString();
			var serialNumber = domainFacade.ModelsBuilder.Create<string>();
			int dataDays = 5;
			var usageEntries = GetUsageEntries(domainFacade, mprn, serialNumber, dataDays);
			int removeEntries = 3;
			usageEntries = RandomlyEntries(usageEntries, removeEntries);
			var fileData = await GenerateDataFile(domainFacade, usageEntries);
			Assert.NotNull(fileData);
			Assert.True(fileData.Length > 319);
			await AssertFileData(fileData, dataDays, mprn, serialNumber, usageEntries);
		}

		private async Task AssertFileData(byte[] fileData, int dataDays, string mprn, string serialNumber, List<AccountConsumption.UsageEntry> usageEntries)
		{
			using (var stream = new MemoryStream(fileData))
			{
				using (var reader = new StreamReader(stream))
				{
					string line;
					var lineNumber = -1;
					while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
					{
						if (lineNumber == -1) //Header row
						{
							AssertHeaderValues(line);
						}
						else
						{
							var date = DateTime.Today.Date.AddDays((-1 * dataDays) + lineNumber);
							AssertRowValues(line, date.ToString("dd/MM/yyyy"), mprn, serialNumber,
								GetUsageValuesAgainstDate(date, usageEntries));
						}

						lineNumber++;
					}
				}
			}
		}

		private List<AccountConsumption.UsageEntry> RandomlyEntries(List<AccountConsumption.UsageEntry> usageEntries, int removeEntriesCount)
		{
			var rand = new Random();
			for (int i = 0; i < removeEntriesCount; i++)
			{
				int totalCount = usageEntries.Count;
				var randIndex = rand.Next(0, totalCount);
				usageEntries.RemoveAt(randIndex);
			}

			return usageEntries;
		}

		private IEnumerable<string> GetUsageValuesAgainstDate(DateTime date, List<AccountConsumption.UsageEntry> usageEntries)
		{
			usageEntries = usageEntries
				.Where(d => d.Date.Date == date)
				.OrderBy(d => d.Date).ToList();
			var usageEntryValues = Enumerable.Repeat("", HoursInADay * 2).ToList();

			int i = 0;
			if (usageEntries[i].Date == date)
			{
				usageEntryValues[usageEntryValues.Count - 1] = usageEntries[i].Value.ToString("F2"); //IF usageEntries contains value for 00:00 put it on the last index
				i++;
			}

			int usageValueIndex = 0;
			foreach (var timeEntry in Enumerable.Range(1, HoursInADay * 2 - 1).Select(r => date.Date.AddMinutes(r * HalfHourInMinutes)))
			{
				if (i >= usageEntries.Count)
				{
					break;
				}
				var usageEntry = usageEntries[i];
				if (usageEntry.Date == timeEntry)
				{
					usageEntryValues[usageValueIndex] = usageEntry.Value.ToString("F2");
					i++;
				}

				usageValueIndex++;
			}

			return usageEntryValues;
		}

		private async Task<byte[]> GenerateDataFile(DomainFacade domainFacade, List<AccountConsumption.UsageEntry> usageEntries)
		{
			var consumptionData = domainFacade.ModelsBuilder.Build<AccountConsumption>()
				.With(x => x.UsageEntries, usageEntries)
				.With(x => x.CostEntries, new List<AccountConsumption.CostEntry>())
				.Create();
			var builder = new HDFFileBuilder();
			return await builder.BuildFileData(consumptionData);
		}

		private void AssertHeaderValues(string line)
		{
			AssertRowValues(line, "Date", "MPRN", "Meter Serial Number", Enumerable.Range(1, HoursInADay * 2 - 1)
				.Select(r => DateTime.Today.Date.AddMinutes(r * HalfHourInMinutes).ToString("HH:mm"))
				.Union(Enumerable.Range(0, 1).Select(r => DateTime.Today.Date.AddMinutes(r * HalfHourInMinutes).ToString("HH:mm"))));
		}

		private void AssertRowValues(string line, string date, string mprn, string serialNumber, IEnumerable<string> halfHourlyValues)
		{
			var headerColumns = line.Split(",");
			Assert.AreEqual(date, headerColumns[0]);
			Assert.AreEqual(mprn, headerColumns[1]);
			Assert.AreEqual(serialNumber, headerColumns[2]);
			var halfHourStartIndex = 3;
			foreach (var halfHourValue in halfHourlyValues)
			{
				Assert.AreEqual(halfHourValue, headerColumns[halfHourStartIndex]);
				halfHourStartIndex++;
			}
		}

		private List<AccountConsumption.UsageEntry> GetUsageEntries(DomainFacade domainFacade, string mprn, string serialNumber, int dataDays)
		{
			var usageEntries = new List<AccountConsumption.UsageEntry>();

			for (int i = 1; i <= dataDays; i++)
			{
				usageEntries = usageEntries.Union(Enumerable.Range(0, HoursInADay * 2).Select(e =>
					 new AccountConsumption.UsageEntry
					 {
						 Date = DateTime.Today.Date.AddDays(-1 * i).AddMinutes(e * HalfHourInMinutes),
						 SerialNumber = serialNumber,
						 Prn = mprn,
						 Value = domainFacade.ModelsBuilder.Create<decimal>()
					 })).ToList();
			}

			return usageEntries;
		}
	}
}

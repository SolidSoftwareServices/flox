using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Commands.Metering.SubmitMeterReading;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Metering.Devices;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.MeterReading
{
	[Explicit]
	[TestFixture]
	public class MeterReadingSubmitCommandTests: DomainTests
	{
		[Test]
		public async Task CanSubmit2DevicesResult()
		{
			await LoginUser("MCC02_DN@test.ie", "Test3333");

			var accountInfos = await DomainQueryProvider.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery());
			Assert.IsNotNull(accountInfos);

			var account = accountInfos.First();

			var devices = await DomainQueryProvider.FetchAsync<DevicesQuery, DeviceInfo>(new DevicesQuery
			{
				AccountNumber = account.AccountNumber,
			});

			Assert.IsNotNull(devices);

			var Input1Value = "95956";
			var Input2Value = "73480";

			var device = devices.First();

			var meterReadingDataResults = new List<MeterReadingData>();
			var meterReadingData = new MeterReadingData
			{
				DeviceId = device.DeviceId,
				MeterReading = Input1Value.ToString(),
				RegisterId = device.Registers.First().RegisterId,
				MeterNumber = device.Registers.First().MeterNumber,
				MeterTypeName = device.Registers.First().MeterType.ToString()
			};
			meterReadingDataResults.Add(meterReadingData);

			meterReadingData = new MeterReadingData
			{
				DeviceId = device.DeviceId,
				MeterReading = Input2Value.ToString(),
				RegisterId = device.Registers.Last().RegisterId,
				MeterNumber = device.Registers.Last().MeterNumber,
				MeterTypeName = device.Registers.Last().MeterType.ToString()
			};
			meterReadingDataResults.Add(meterReadingData);

			var cmd = new SubmitMeterReadingCommand(account.AccountNumber, meterReadingDataResults);
			await DomainCommandDispatcher.ExecuteAsync(cmd);
		}
	}
}

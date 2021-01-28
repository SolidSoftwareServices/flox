using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Serialization;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Metering
{
    [TestFixture]
    [Explicit("TODO")]
	public class MeterReadingQueryTest: DomainTests
    {
        public static IEnumerable QueryDoesNotFailCases()
        {
            yield return new TestCaseData("hop.mark@test.com", "Test1234").Returns(1);
        }

        [Test, TestCaseSource(nameof(QueryDoesNotFailCases))]
        public async Task<int> QueryReturnsMeterReadingDevices(string userName, string password)
        {
            await LoginUser(userName, password);
            var result = await DomainQueryProvider.FetchAsync<MeterReadingsQuery, MeterReadingInfo>(new MeterReadingsQuery());


            Assert.IsNotNull(result);


            Console.WriteLine(result.ToJson());

            return result.Count();
        }

        public static IEnumerable QueryDoesReturnsHistoryDataCases()
        {
           // yield return new TestCaseData("NSHspecial1@esb.ie", "Test3333", "950141662");
            yield return new TestCaseData("NSH24Hr1@esb.ie", "Test3333", "903764173");
        }

        [Test, TestCaseSource(nameof(QueryDoesReturnsHistoryDataCases))]
        public async Task QueryReturnsMeterReadingHistoryData(string userName, string password, string accountNumber)
        {
            await LoginUser(userName, password);
            var result = await DomainQueryProvider.FetchAsync<MeterReadingsQuery, MeterReadingInfo>(
                new MeterReadingsQuery
                {
                    AccountNumber = accountNumber
                }
             );

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(x => x.MeterType == MeterType.Electricity24h.ToString()));
            Assert.IsTrue(result.Any(x => x.MeterType == MeterType.ElectricityNightStorageHeater.ToString()));
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Metering.Devices;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Metering
{
    [Explicit("TODO:TEST")]
    [TestFixture]
    public class DevicesQueryTests : DomainTests
    {
        [Test]
        public async Task CanQueryByAccountNumber()
        {
            await LoginUser("Elecdd32@esb.ie", "Test3333");

            var result = await DomainQueryProvider.GetDevicesByAccount("900294276");
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(MeterType.Electricity24h, result.First().MeterType);
            Assert.AreEqual(MeterType.ElectricityNightStorageHeater, result.Last().MeterType);
        }

        [Test]
        public async Task CanQueryByDeviceId()
        {
            await LoginUser("Elecdd32@esb.ie", "Test3333");

            var resultE24 = await DomainQueryProvider.GetDeviceById("11385875");
            Assert.NotNull(resultE24);
            Assert.AreEqual(MeterType.Electricity24h, resultE24.MeterType);

            var resultNhs = await DomainQueryProvider.GetDeviceById("11915469");
            Assert.NotNull(resultNhs);
            Assert.AreEqual(MeterType.ElectricityNightStorageHeater, resultNhs.MeterType);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountAndMeterDetails;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountAndMeterDetails
{
    [TestFixture]
    abstract class WhenAccountAndMeterDetailsOnTests : MyAccountCommonTests<AccountAndMeterDetailsPage>
    {
        [Test]
        public virtual async Task TheViewShowsTheExpectedInformation()
        {
            var businessAgreement = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.BusinessAgreement;
            var device = UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.Single();

            var billingAddress = businessAgreement.BillToAccountAddress == null
                ? string.Empty
                : businessAgreement.BillToAccountAddress.AsDescriptionText();
            var contractItem = businessAgreement.Contracts.Single();
            var siteAddress = contractItem.Premise.Address == null
                ? string.Empty
                : contractItem.Premise.Address.AsDescriptionText();

            var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;

	        var meterPointRefNumber = contractItem.Premise.PointOfDeliveries.FirstOrDefault()?.Prn ?? " ";
            var networkMeterNumber = device.SerialNum.Mask('*', device.SerialNum.Length - 4);
            var meterDesc = device?.DeviceDescription;
            var meterLocation = device?.DeviceLocation;
            var meterIsSmart = device?.IsSmart;
            var meterCommsLevel = (device.CTF ?? CommsTechnicallyFeasibleValue.NotAvailableYet) == CommsTechnicallyFeasibleValue.NotAvailableYet
                ? "Pending"
                : $"Level { device.CTF.ToString().Replace("0", string.Empty) }";

            var meterNumberFieldName =
                businessAgreement.Contracts.Single().Division ==
                DivisionType.Gas
                    ? "GPRN"
                    : "MPRN";

            Assert.AreEqual(accountInfo.Name, Sut.DetailsInfo.AccountName?.TextContent);
            Assert.AreEqual(accountInfo.AccountNumber, Sut.DetailsInfo.AccountNumber?.TextContent);
            Assert.AreEqual(siteAddress, Sut.DetailsInfo.SiteAddress?.TextContent);
            Assert.AreEqual(billingAddress, Sut.DetailsInfo.BillingAddress?.TextContent);

            Assert.AreEqual(meterNumberFieldName, Sut.DetailsInfo.MetersDetails.First().MeterPointRefNumberLabel?.TextContent);
            Assert.AreEqual(meterPointRefNumber, Sut.DetailsInfo.MetersDetails.First().MeterPointRefNumber?.TextContent);
            Assert.AreEqual(networkMeterNumber, Sut.DetailsInfo.MetersDetails.First().NetworksMeterNumber?.TextContent);
            Assert.AreEqual(meterLocation, Sut.DetailsInfo.MetersDetails.First().Location?.TextContent);
            Assert.AreEqual(meterDesc, Sut.DetailsInfo.MetersDetails.First().Description?.TextContent);

            if (meterIsSmart.Value)
            {
                Assert.AreEqual(meterCommsLevel, Sut.DetailsInfo.MetersDetails.First().CommsLevel?.TextContent);
            }

            var last2Years = new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today);
            if (accountInfo.SmartPeriods.Any(sp => sp.Intersects(last2Years)) || UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.Any(x => x.SmartActivationStatus == SmartActivationStatus.SmartActive && x.CTF != null && x.CTF.IsOneOf(CommsTechnicallyFeasibleValue.CTF3, CommsTechnicallyFeasibleValue.CTF4)))
			{
                Assert.NotNull(Sut.DetailsInfo.MeterConsumptionData);
                Assert.IsTrue(Sut.DetailsInfo.MeterConsumptionDataDownloadLink?.Href.EndsWith($"Files/GetConsumptionDataFile?AccountNumber={accountInfo.AccountNumber}"));
            }
        }
    }
}
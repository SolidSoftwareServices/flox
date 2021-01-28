using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountAndMeterDetails;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountAndMeterDetails
{
	[TestFixture]
	internal class WhenInElectricityAccountAndManyMetersTests : WhenAccountAndMeterDetailsOnTests
	{
		internal class TestCaseDevice
		{
			public string MeterPointRefNumber { get; set; }
			public string NetworkMeterNumber { get; set; }
			public string MeterDescription { get; set; }
			public string MeterLocation { get; set; }
			public bool MeterIsSmart { get; set; }
			public string MeterCommsLevel { get; set; }
		}

		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount();

			UserConfig.ElectricityAccount().WithElectricityDayAndNightDevices();
			UserConfig.Execute();

			await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			Sut = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToAccountAndMeterDetails())
				.CurrentPageAs<AccountAndMeterDetailsPage>();
		}

		[Test]
		public override async Task TheViewShowsTheExpectedInformation()
		{
			var businessAgreement = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.BusinessAgreement;

			var billingAddress = businessAgreement.BillToAccountAddress == null
				? string.Empty
				: businessAgreement.BillToAccountAddress.AsDescriptionText();
			var contractItem = businessAgreement.Contracts.Single();
			var siteAddress = contractItem.Premise.Address == null
				? string.Empty
				: contractItem.Premise.Address.AsDescriptionText();

			var accountInfo = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model;

			var meterNumberFieldName =
				businessAgreement.Contracts.Single().Division ==
				DivisionType.Gas
					? "GPRN"
					: "MPRN";
			Assert.AreEqual(meterNumberFieldName, Sut.DetailsInfo.MetersDetails.First().MeterPointRefNumberLabel?.TextContent);
			Assert.AreEqual(accountInfo.Name, Sut.DetailsInfo.AccountName?.TextContent);
			Assert.AreEqual(accountInfo.AccountNumber, Sut.DetailsInfo.AccountNumber?.TextContent);
			Assert.AreEqual(siteAddress, Sut.DetailsInfo.SiteAddress?.TextContent);
			Assert.AreEqual(billingAddress, Sut.DetailsInfo.BillingAddress?.TextContent);

			var metersDetails = Sut.DetailsInfo.MetersDetails.ToArray();
			var devices = UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.ToArray();

			for (var i = 0; i < devices.Length; i++)
			{
				AssertDevice(new TestCaseDevice
				{
					MeterPointRefNumber = contractItem.Premise.PointOfDeliveries.FirstOrDefault().Prn,
					MeterDescription = devices[i].DeviceDescription,
					MeterLocation = devices[i].DeviceLocation,
					NetworkMeterNumber = devices[i].SerialNum.Mask('*', devices[i].SerialNum.Length - 4),
                    MeterIsSmart = devices[i].IsSmart,
                    MeterCommsLevel = (devices[i].CTF ?? CommsTechnicallyFeasibleValue.NotAvailableYet) == CommsTechnicallyFeasibleValue.NotAvailableYet
                        ? "Pending"
                        : $"Level { devices[i].CTF.ToString().Replace("0", string.Empty) }"
				}, metersDetails[i]);
			}
			var last2Years = new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today);
			if (accountInfo.SmartPeriods.Any(sp => sp.Intersects(last2Years))||
				UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.Any(x => x.SmartActivationStatus == SmartActivationStatus.SmartActive && x.CTF != null && x.CTF.IsOneOf(CommsTechnicallyFeasibleValue.CTF3, CommsTechnicallyFeasibleValue.CTF4)))
			{
                Assert.NotNull(Sut.DetailsInfo.MeterConsumptionData);
                Assert.IsTrue(Sut.DetailsInfo.MeterConsumptionDataDownloadLink?.Href.EndsWith($"Files/GetConsumptionDataFile?AccountNumber={accountInfo.AccountNumber}"));
            }
		}

		private void AssertDevice(TestCaseDevice device, AccountAndMeterDetailsPage.MeterDetails meterDetails)
		{
			Assert.AreEqual(
				device.MeterPointRefNumber,
				meterDetails.MeterPointRefNumber.TextContent);
			Assert.AreEqual(
				device.NetworkMeterNumber,
				meterDetails.NetworksMeterNumber.TextContent);
			Assert.AreEqual(
				device.MeterLocation,
				meterDetails.Location.TextContent);
			Assert.AreEqual(
				device.MeterDescription,
				meterDetails.Description.TextContent);

            if (device.MeterIsSmart)
            {
                Assert.AreEqual(device.MeterCommsLevel, meterDetails.CommsLevel.TextContent);
            }
		}
	}
}
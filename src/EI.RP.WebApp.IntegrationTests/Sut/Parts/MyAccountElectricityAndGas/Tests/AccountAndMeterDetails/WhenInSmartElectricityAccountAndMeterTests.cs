using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.WebApp.DomainModelExtensions;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountAndMeterDetails;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountAndMeterDetails
{
	[TestFixture]
	internal class WhenInSmartElectricityAccountAndMeterTests : MyAccountCommonTests<AccountAndMeterDetailsPage>
	{
		internal class CaseModel
		{
			public CommsTechnicallyFeasibleValue CTF { get; set; }
			public RegisterConfigType MCCConfiguration { get; set; }
		}
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount();
			UserConfig.Execute();
        }

		public static IEnumerable<TestCaseData> GetTestCases()
		{
			foreach (RegisterConfigType mccConfig in RegisterConfigType.AllValues)
			{
				foreach (CommsTechnicallyFeasibleValue ctf in CommsTechnicallyFeasibleValue.AllValues)
				{
					yield return new TestCaseData(new CaseModel { MCCConfiguration = mccConfig, CTF = ctf }).SetName($"MCC: {mccConfig}-CommsLevel: {ctf}");
				}
			}
		}
	    
        [TestCaseSource(nameof(GetTestCases))]
        public async Task TheViewShowsTheExpectedInformationPerCommsLevel(CaseModel config)
        {
            var device = UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.Single();
            await NavigateToSut(device, config.CTF, config.MCCConfiguration);

            var businessAgreement = UserConfig.ElectricityAndGasAccountConfigurators.Single().Model.BusinessAgreement;

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
			var meterDesc = device.DeviceDescription;
			var meterLocation = device.DeviceLocation;
			var meterIsSmart = device.IsSmart;
			var meterCommsLevel = device.CTF == CommsTechnicallyFeasibleValue.NotAvailableYet
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

			var firstMeterDetails = Sut.DetailsInfo.MetersDetails.First();
			Assert.AreEqual(meterNumberFieldName, firstMeterDetails.MeterPointRefNumberLabel?.TextContent);
			Assert.AreEqual(meterPointRefNumber, firstMeterDetails.MeterPointRefNumber?.TextContent);
			Assert.AreEqual(networkMeterNumber, firstMeterDetails.NetworksMeterNumber?.TextContent);
			Assert.AreEqual(meterLocation, firstMeterDetails.Location?.TextContent);
			Assert.AreEqual(meterDesc, firstMeterDetails.Description?.TextContent);

			if (config.MCCConfiguration != RegisterConfigType.MCC16)
			{
				Assert.AreEqual(meterCommsLevel, firstMeterDetails.CommsLevel.TextContent);
			}
			else
			{
				Assert.IsNull(firstMeterDetails.CommsLevel);
			}

			var last2Years = new DateTimeRange(DateTime.Today.AddYears(-2), DateTime.Today);
			if (accountInfo.SmartPeriods.Any(sp => sp.Intersects(last2Years)) || UserConfig.ElectricityAndGasAccountConfigurators.Single().Premise.Devices.Any(x => x.SmartActivationStatus == SmartActivationStatus.SmartActive && x.CTF != null && x.CTF.IsOneOf(CommsTechnicallyFeasibleValue.CTF3, CommsTechnicallyFeasibleValue.CTF4)))
            {
                Assert.NotNull(Sut.DetailsInfo.MeterConsumptionData);
                Assert.IsTrue(Sut.DetailsInfo.MeterConsumptionDataDownloadLink?.Href.EndsWith($"Files/GetConsumptionDataFile?AccountNumber={accountInfo.AccountNumber}"));
            }
		}

        protected async Task NavigateToSut(DeviceInfo device, CommsTechnicallyFeasibleValue commsTechnicallyFeasibleValue, RegisterConfigType mccCongfig)
        {
            device.IsSmart = true;
            device.CTF = commsTechnicallyFeasibleValue;
            device.MCCConfiguration = mccCongfig;

            await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            Sut = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToAccountAndMeterDetails())
                .CurrentPageAs<AccountAndMeterDetailsPage>();
        }
	}
}
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.MakeAPayment
{
    [TestFixture]
    class WhenInMakeAPayment_FromEstimateCostFlow: WhenInMakeAPaymentPage_AndPaymentTypeIsDirectDebit_ShowLastPaymentDetails_Test
    {
	    public override bool CanEstimateLatestBill { get; } = true;
	    public override PaymentMethodType PaymentType { get; } = PaymentMethodType.DirectDebit;
        [Test]
        public  async Task ItShowsTheEstimatedAmount()
        {
            var configurator = UserConfig.ElectricityAndGasAccountConfigurators.Single();
            configurator.NextBillEstimation.CostCanBeEstimated = true;
            var estimateCost = configurator.NextBillEstimation.EstimatedAmount;
            Assert.AreNotEqual(estimateCost, (EuroMoney)Sut.ShowPaymentDetails.AmountDue);
        }
    }
}

using Ei.Rp.DomainModels.Contracts;
using EI.RP.CoreServices.System;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.DomainServices.Queries.Metering.Consumption.Services;
using EI.RP.TestServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Moq;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainServices.Queries.Metering.Devices;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Billing.Activity;
using static Ei.Rp.DomainModels.Billing.AccountBillingActivity;

namespace EI.RP.DomainServices.UnitTests.Queries.Metering.Consumption.Services
{
	[TestFixture]
	internal class NonSmartConsumptionResolverTest : UnitTestFixture<NonSmartConsumptionResolverTest.TestContext, NonSmartConsumptionResolver>
	{
		internal class TestContext : UnitTestContext<NonSmartConsumptionResolver>
		{
		}

		[Test]
		public async Task ItFiltersSecurityDeposit()
		{
			var queryModel = new AccountConsumptionQuery()
			{
				AccountNumber = "904751413",
				RetrievalType = ConsumptionDataRetrievalType.NonSmart,
			};

			var accountInfo = new AccountInfo
			{
				AccountNumber = "123",
				Description = "bill",
				IsOpen = true,
				ContractId = "2012402935",
			};

			var deviceInfo = new DeviceInfo()
			{
				MeterReadingResults = new List<MeterReadingInfo>()
				{
					new MeterReadingInfo { ReadingDateTime = DateTime.Now, Reading = 12345, },
					new MeterReadingInfo { ReadingDateTime = DateTime.Now, Reading = 13345, },
					new MeterReadingInfo { ReadingDateTime = DateTime.Now, Reading = 14345, }
				},
				ContractId = "1234556",
				MeterReading = "5",
			};

			EuroMoney securityDepositAmount = 999.00;
			var accountBillingActivitySecurity = new AccountBillingActivity(ActivitySource.Invoice)
			{
				AccountNumber = "1233",
				Amount = securityDepositAmount,
				DueAmount = "999",
				Description = "security deposit",
				OriginalDate = new DateTime(2019, 1, 1),
			};

			EuroMoney invoiceAmount = 434.00;
			var accountBillingActivityBill = new AccountBillingActivity(ActivitySource.Invoice)
			{
				AccountNumber = "123",
				Amount = invoiceAmount,
				DueAmount = "434",
				Description = "bill",
				OriginalDate = new DateTime(2019, 2, 1),
			};

			var accountBilliingActivitiesList = new List<AccountBillingActivity>();
			accountBilliingActivitiesList.Add(accountBillingActivitySecurity);
			accountBilliingActivitiesList.Add(accountBillingActivityBill);

			var domainQueryResolver = Context.AutoMocker.GetMock<IDomainQueryResolver>();

			domainQueryResolver.Setup(x =>
					x.FetchAsync<DevicesQuery, DeviceInfo>(It.IsAny<DevicesQuery>(), It.IsAny<bool>()))
				.Returns(
					Task.FromResult(deviceInfo.ToOneItemArray().AsEnumerable()));


			domainQueryResolver.Setup(x =>
					x.FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(It.IsAny<AccountBillingActivityQuery>(), It.IsAny<bool>()))
				.Returns(
					Task.FromResult(accountBilliingActivitiesList.AsEnumerable()));


			var result = await Context.Sut.ResolveCostsAndUsage(queryModel, accountInfo);

			foreach (var r in result)
			{
				Assert.AreNotEqual(securityDepositAmount, r.cost.Value);
				Assert.AreEqual(invoiceAmount, r.cost.Value);
			}
		}
	}
}

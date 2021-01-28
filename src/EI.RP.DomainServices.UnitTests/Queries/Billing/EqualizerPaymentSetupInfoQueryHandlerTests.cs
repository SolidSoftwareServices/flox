using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.System;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.Billing
{
	[Ignore("Needs to be reworked")]
	[TestFixture]
	internal class EqualizerPaymentSetupInfoQueryHandlerTests : QueryHandlerTest<
		EqualizerPaymentSetupInfoQueryHandler, EqualizerPaymentSetupInfoQuery>
	{
	

		public static IEnumerable CanQueryCases()
		{
			var fixture = new Fixture().CustomizeDomainTypeBuilders();
			var values = new[] {false, true};
			foreach (var isPaye in values)
			foreach (var allowsEqualizer in values)
			foreach (var contractIsOpened in values)
			{

				var accountInfo = fixture.Build<AccountInfo>()
					.With(x => x.IsPAYGCustomer, isPaye)
					.With(x => x.ContractEndDate,
						contractIsOpened ? SapDateTimes.SapDateTimeMax : DateTime.Today.AddHours(-1))
					.Create();
				var businessAgreement = fixture.Build<BusinessAgreement>()
					.With(x => x.BusinessAgreementId, accountInfo.AccountNumber)
					.With(x => x.IncomingPaymentMethodType,
						allowsEqualizer ? PaymentMethodType.Manual : PaymentMethodType.Equalizer)
					.With(x => x.AccountCategory, DivisionType.Electricity)
					.With(x => x.AlternativePayerId, (string) null)
					.With(x => x.CollectiveParentId, (string) null)
					.With(x => x.AccountDeterminationID, AccountDeterminationType.ResidentialCustomer)
					.Create();

				var canSetup = !isPaye && allowsEqualizer && contractIsOpened;
				EqualizerPaymentSetupInfo equalizerPaymentSetupInfo;
				if (!canSetup)
				{
					equalizerPaymentSetupInfo = new EqualizerPaymentSetupInfo
					{
						AccountNumber = accountInfo.AccountNumber,
						CanSetUpEqualizer = canSetup
					};
					yield return new TestCaseData(accountInfo, businessAgreement)
						.Returns(equalizerPaymentSetupInfo)
						.SetName(
							$"CanQuery - isPaye:{isPaye} allowsEqualizer:{allowsEqualizer} contractClosed:{!contractIsOpened}");
						}


				//yield return new TestCaseData(accountInfo, businessAgreement)
				//	.Returns(equalizerPaymentSetupInfo)
				//	.SetName(
				//		$"CanQuery - isPaye:{isPaye} allowsEqualizer:{allowsEqualizer} contractClosed:{!contractIsOpened}");
			}
		}

		[Test,TestCaseSource(nameof(CanQueryCases))]
		public async Task<EqualizerPaymentSetupInfo> CanQuery(AccountInfo accountInfo,BusinessAgreement businessAgreement)
		{
			var domainCacheProviderMock = Context.AutoMocker.GetMock<ICacheProvider>();
			
			domainCacheProviderMock
				.Setup(x => x.GetOrAddAsync(It.Is<AccountInfoQuery>(q =>
					q.AccountNumber.Equals(accountInfo.AccountNumber)),
					It.IsAny<Func<Task<IEnumerable<AccountInfo>>>>(),
					It.IsAny<string>(),It.IsAny<TimeSpan?>(),It.IsAny<CancellationToken>()))
				.ReturnsAsync(accountInfo.ToOneItemArray());

			var query = new EqualizerPaymentSetupInfoQuery
			{
				AccountNumber = accountInfo.AccountNumber
			};
			var actual=(await Context.Sut.ExecuteQueryAsync<EqualizerPaymentSetupInfo>(query)).Single();

			return actual;
		}
	}
}
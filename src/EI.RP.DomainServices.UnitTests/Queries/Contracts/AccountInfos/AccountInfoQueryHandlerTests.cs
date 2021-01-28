using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Http.Session;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Dtos.Extensions;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.Contracts.AccountInfos
{
	[TestFixture]
	internal class AccountInfoQueryHandlerTests : QueryHandlerTest<
		AccountInfoQueryHandler, AccountInfoQuery>
	{
	
		public static IEnumerable CanExecuteQueryAsyncTestCases()
		{
			AccountInfo MapExpectedAccountInfo(ContractItemDto x)
			{
				var accountInfo = new AccountInfo
				{
					AccountNumber = x.BusinessAgreementID,
					Partner = x.AccountID,
					IsOpen = !x.ContractEndDate.HasValue || x.ContractEndDate.Value > DateTime.Now,
					ClientAccountType = ((DivisionType) x.DivisionID).ToAccountType()
				};
				if (x.BusinessAgreement != null)
				{
					accountInfo.Name = x.BusinessAgreement.Account == null
						? string.Empty
						: x.BusinessAgreement.Account.FullName;

					accountInfo.PaymentMethod = PaymentMethodType.From(x.BusinessAgreement.IncomingPaymentMethodID);
				}

				if (x.Premise != null)
					accountInfo.Description = x.Premise.AddressInfo == null
						? string.Empty
						: x.Premise.AddressInfo.AsDescriptionText();

				//TODO: real values
				accountInfo.IsPAYGCustomer = false;
				
				return accountInfo;
			}


			yield return new TestCaseData(new object[] {new ContractItemDto[0]})
				.Returns(new AccountInfo[0])
				.SetName("When no contracts");

			var fixture = new Fixture().CustomizeDomainTypeBuilders();

			////one contract
			//var contracts = fixture.Build<ContractItemDto>()
			//	//.With(x=>x.DivisionID)
			//	.CreateMany(1).ToArray();
			//yield return new TestCaseData(new object[] {contracts})
			//	.Returns(
			//		contracts
			//			.Select(MapExpectedAccountInfo).ToArray()
			//	)
			//	.SetName("When one contract").Ignore("FIX"); ;

			////more than one contract
			//contracts = fixture.CreateMany<ContractItemDto>(3).ToArray();
			//yield return new TestCaseData(new object[] {contracts})
			//	.Returns(
			//		contracts
			//			.Select(MapExpectedAccountInfo).ToArray()
			//	)
			//	.SetName("When more than one contract").Ignore("FIX");
		}
		[Ignore("TODO")]
		[Test]
		[TestCaseSource(nameof(CanExecuteQueryAsyncTestCases))]
		public async Task<IEnumerable<AccountInfo>> CanExecuteQueryAsync(ContractItemDto[] apiContractItemsDto)
		{
			Context.AutoMocker.GetMock<IUserSessionProvider>().SetupGet(x => x.CurrentUserClaimsPrincipal)
				.Returns(new ClaimsPrincipal());
			Context.CrmUmcRepoMock.Value.MockQuery(apiContractItemsDto)
				.WithNavigation<BusinessAgreementDto, ContractItemDto>();
			Context.CrmUmcRepoMock.Value.MockQuery(apiContractItemsDto.Select(x => x.BusinessAgreement).ToArray())
				.WithNavigation<AccountDto, BusinessAgreementDto>();


			var actual =
				await Context.Sut.ExecuteQueryAsync<AccountInfo>(Context.Fixture.Build<AccountInfoQuery>()
					.With(x => x.Prn, (PointReferenceNumber) null).Without(x => x.BusinessPartner).Create());
			return actual;
		}
	}
}
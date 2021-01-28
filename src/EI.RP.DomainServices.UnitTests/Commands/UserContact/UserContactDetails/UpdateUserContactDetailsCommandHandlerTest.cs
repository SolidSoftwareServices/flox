using AutoFixture;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.User;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Commands.Users.UserContact;
using EI.RP.DomainServices.Queries.User.PhoneMetaData;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.UnitTests.Commands.UserContact.UserContactDetails
{
	internal class UpdateUserContactDetailsCommandHandlerTest : CommandHandlerTest<
		UpdateUserContactDetailsCommandHandler,
		UpdateUserContactDetailsCommand>
	{

		public static IEnumerable<TestCaseData> CanResolveCases()
		{
			var testCases = new[]
			{
				new CaseModel(),
				new CaseModel
				{
					Phone = new PhoneInfo {PhoneNumer = "08712345678", Type = PhoneMetadataType.Mobile},
					Email = "test@esb.ie"
				},
				new CaseModel
				{
					Phone = new PhoneInfo {PhoneNumer = "0871278", Type = PhoneMetadataType.Invalid},
					Email = "test@esb.ie",
				},
				new CaseModel
				{
					Phone = new PhoneInfo {PhoneNumer = "08712345678", Type = PhoneMetadataType.Mobile},
					AlternativePhone = new PhoneInfo {PhoneNumer = "08912345678", Type = PhoneMetadataType.Mobile},
					Email = "test@esb.ie",
					PreviousEmail = "prev@esb.ie"
				},
				new CaseModel
				{
					Phone = new PhoneInfo {PhoneNumer = "08712345678", Type = PhoneMetadataType.Mobile},
					AlternativePhone = new PhoneInfo {PhoneNumer = "088", Type = PhoneMetadataType.Invalid},
					Email = "test@esb.ie",
					PreviousEmail = "prev@esb.ie"
				},
				new CaseModel
				{
					Phone = new PhoneInfo {PhoneNumer = "08712345678", Type = PhoneMetadataType.Mobile},
					AlternativePhone = new PhoneInfo {PhoneNumer = "0219105044", Type = PhoneMetadataType.LandLine},
					Email = "test@esb.ie",
					PreviousEmail = "prev@esb.ie"
				},
				new CaseModel
				{
					Phone = new PhoneInfo {PhoneNumer = "0219105044", Type = PhoneMetadataType.LandLine},
					Email = "test@esb.ie",
					PreviousEmail = "prev@esb.ie"
				},
				new CaseModel
				{
					Phone = new PhoneInfo {PhoneNumer = "0219105044", Type = PhoneMetadataType.LandLine},
					AlternativePhone = new PhoneInfo {PhoneNumer = "0619105041", Type = PhoneMetadataType.LandLine},
					Email = "test@esb.ie",
				}
			};
			foreach (var caseItem in testCases)
			{
				yield return new TestCaseData(caseItem).SetName(caseItem.ToString());
			}
		}


		[TestCaseSource(nameof(CanResolveCases))]
		public async Task UpdateUserContactDetails(CaseModel caseData)
		{
			var accountNumber = Context.Fixture.Create<string>();
			var accountId = Context.Fixture.Create<string>();
			AccountInfo accountInfo = Context.Fixture.Build<AccountInfo>()
				.With(x => x.AccountNumber, accountNumber)
				.With(x => x.Partner, accountId)
				.With(x=>x.PointReferenceNumber,Context.Fixture.Create<ElectricityPointReferenceNumber>())
				//removed to improve the test perfomance since its not needed
				.Without(x=>x.BankAccounts)
				.Without(x=>x.BusinessAgreement)
				.Without(x=>x.NonSmartPeriods)
				.Without(x=>x.SmartPeriods)
				.Create();

			AccountDto accountDto = Context.Fixture
				.Build<AccountDto>()
				.With(x => x.AccountID, accountId)
				//removed to improve the test perfomance since its not needed
				.Without(x=>x.AccountAddressIndependentEmails)
				.Without(x=>x.AccountAddressIndependentFaxes)
				.Without(x=>x.AccountAddressIndependentMobilePhones)
				.Without(x=>x.AccountAddressIndependentPhones)
				.Without(x=>x.AccountAddresses)
				.Without(x=>x.QuotationItems)
				.Without(x=>x.PaymentCards)
				.Create();

			var cmd = ArrangeAndGetCommand();

			try
			{
				await Context.Sut.ExecuteAsync(cmd);
			}
			catch (DomainException de)
			{
				if (caseData.Phone == null ||
					caseData.Phone.Type == PhoneMetadataType.Invalid ||
					caseData.AlternativePhone?.Type == PhoneMetadataType.Invalid)
				{
					Assert.AreEqual(de.DomainError, ResidentialDomainError.ContactNumberInvalid);
				}
				else
				{
					throw;
				}
			}


			var repositoryOfCrmUmc = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();
			if (caseData.Phone == null ||
				caseData.Phone.Type == PhoneMetadataType.Invalid ||
				caseData.AlternativePhone?.Type == PhoneMetadataType.Invalid)
			{
				AssertPhoneDeletion(Times.Never());
				return;
			}

			AssertPhoneUpdate();
			AssertEmailUpdate();

			void AssertPhoneUpdate()
			{
				AssertPhoneDeletion(Times.Once());
				if (caseData.Phone.Type == PhoneMetadataType.Mobile &&
					(caseData.AlternativePhone == null ||
					 caseData.AlternativePhone?.Type == PhoneMetadataType.Mobile))
				{
					AssertMobilePhoneUpdate(cmd.PrimaryPhoneNumber, PhoneType.DefaultMobilePhone, true, true);

					if (!string.IsNullOrEmpty(cmd.AlternativePhoneNumber))
					{
						AssertMobilePhoneUpdate(cmd.AlternativePhoneNumber, PhoneType.NotDefaultMobilePhone, false, false);
					}
				}
				else if (caseData.Phone.Type == PhoneMetadataType.Mobile &&
						 caseData.AlternativePhone?.Type == PhoneMetadataType.LandLine)
				{
					AssertMobilePhoneUpdate(cmd.PrimaryPhoneNumber, PhoneType.DefaultMobilePhone, true, true);

					if (!string.IsNullOrEmpty(cmd.AlternativePhoneNumber))
					{
						AssertLandlinePhoneUpdate(cmd.AlternativePhoneNumber, PhoneType.DefaultLandlinePhone, false);
					}
				}
				else if (caseData.Phone.Type == PhoneMetadataType.LandLine &&
						 (caseData.AlternativePhone == null ||
						  caseData.AlternativePhone?.Type == PhoneMetadataType.Mobile))
				{
					AssertLandlinePhoneUpdate(cmd.PrimaryPhoneNumber, PhoneType.DefaultLandlinePhone, true);
					if (!string.IsNullOrEmpty(cmd.AlternativePhoneNumber))
					{
						AssertMobilePhoneUpdate(cmd.AlternativePhoneNumber, PhoneType.DefaultMobilePhone, false, true);
					}
				}
				else
				{
					AssertLandlinePhoneUpdate(cmd.PrimaryPhoneNumber,
							PhoneType.DefaultLandlinePhone, true);

					if (!string.IsNullOrEmpty(cmd.AlternativePhoneNumber))
					{
						AssertLandlinePhoneUpdate(cmd.AlternativePhoneNumber,
							string.Empty, false);
					}
				}

				void AssertMobilePhoneUpdate(string phoneNumber, PhoneType phoneType, bool standardFlag, bool defaultFlag)
				{
					var phoneCountry = (phoneNumber.StartsWith("0044") || phoneNumber.StartsWith("+44")) ? CountryIdType.GB : CountryIdType.IE;
					repositoryOfCrmUmc.Verify(_ => _.Add(It.Is<AccountAddressDependentMobilePhoneDto>(m =>
						m.AccountID == accountInfo.Partner &&
						m.AddressID == accountInfo.BillToAccountAddressId &&
						m.CountryID == phoneCountry &&
						string.IsNullOrEmpty(m.Extension) &&
						m.HomeFlag &&
						m.PhoneNo == phoneNumber &&
						m.PhoneType == phoneType &&
						string.IsNullOrEmpty(m.SequenceNo) &&
						m.StandardFlag == standardFlag &&
						m.DefaultFlag == defaultFlag), true), Times.Once);
				}

				void AssertLandlinePhoneUpdate(string phoneNumber, string phoneType, bool standardFlag)
				{
					var phoneCountry = (phoneNumber.StartsWith("0044") || phoneNumber.StartsWith("+44")) ? CountryIdType.GB : CountryIdType.IE;
					repositoryOfCrmUmc.Verify(_ => _.Add(It.Is<AccountAddressDependentPhoneDto>(m =>
						m.AccountID == accountInfo.Partner &&
						m.AddressID == accountInfo.BillToAccountAddressId &&
						m.CountryID == phoneCountry &&
						string.IsNullOrEmpty(m.Extension) &&
						m.HomeFlag &&
						m.PhoneNo == phoneNumber &&
						m.PhoneType == phoneType &&
						string.IsNullOrEmpty(m.SequenceNo) &&
						m.StandardFlag == standardFlag), true), Times.Once);
				}
			}

			void AssertEmailUpdate()
			{
				var times = cmd.ContactEMail != cmd.PreviousContactEMail ? Times.Once() : Times.Never();

				repositoryOfCrmUmc.Verify(_ => _.Add(It.Is<AccountAddressDependentEmailDto>(m =>
					  m.AccountID == accountInfo.Partner &&
					  m.Email == cmd.ContactEMail &&
					  m.HomeFlag &&
					  string.IsNullOrEmpty(m.SequenceNo) &&
					  m.AddressID == accountInfo.BillToAccountAddressId
					), true), times);

			}

			void AssertPhoneDeletion(Times times)
			{
				foreach (var existingPhone in accountDto.StandardAccountAddress
					.AccountAddressDependentPhones)
				{
					repositoryOfCrmUmc.Verify(_ => _.Delete(existingPhone, true), times);
				}
				foreach (var mobilePhone in accountDto.StandardAccountAddress
					.AccountAddressDependentMobilePhones)
				{
					repositoryOfCrmUmc.Verify(_ => _.Delete(mobilePhone, true), times);
				}
			}

			UpdateUserContactDetailsCommand ArrangeAndGetCommand()
			{
				var domainFacade = new DomainFacade();
				domainFacade.SetUpMocker(Context.AutoMocker);
				domainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
				{
					AccountNumber = accountNumber
				},
					accountInfo.ToOneItemArray());

				Context.CrmUmcRepoMock.Value.MockQuerySingle(accountDto);
				var accountQueryMock = Context.AutoMocker.GetMock<IFluentODataModelQuery<AccountDto>>();
				accountQueryMock.Setup(_ => _.GetOne()).ReturnsAsync(accountDto);

				SetupMockForPhoneNumber(caseData.Phone);
				SetupMockForPhoneNumber(caseData.AlternativePhone);

				return new UpdateUserContactDetailsCommand(accountNumber,
					caseData.Phone?.PhoneNumer,
					caseData.AlternativePhone?.PhoneNumer,
					caseData.Email,
					caseData.PreviousEmail);

				void SetupMockForPhoneNumber(PhoneInfo phone)
				{
					domainFacade.QueryResolver.ExpectQuery(new PhoneMetadataResolverQuery
					{
						PhoneNumber = phone?.PhoneNumer
					},
						new PhoneMetaData
						{
							ContactNumberType = phone?.Type ?? PhoneMetadataType.Invalid
						}.ToOneItemArray());
				}
			}
		}

		internal class CaseModel
		{
			public string Email { get; set; }
			public string PreviousEmail { get; set; }
			public PhoneInfo Phone { get; set; }
			public PhoneInfo AlternativePhone { get; set; }
			public override string ToString()
			{
				return
					$"{Email ?? "NoEmail"}-{Phone?.ToString() ?? "NoPhone"}-{AlternativePhone?.ToString() ?? "NoAlternatePhone"}-{PreviousEmail ?? "NoPreviousEmail"}";
			}
		}

		internal class PhoneInfo
		{
			public string PhoneNumer { get; set; }
			public PhoneMetadataType Type { get; set; }
			public override string ToString()
			{
				return $"{PhoneNumer}-{Type}";
			}
		}
	}
}

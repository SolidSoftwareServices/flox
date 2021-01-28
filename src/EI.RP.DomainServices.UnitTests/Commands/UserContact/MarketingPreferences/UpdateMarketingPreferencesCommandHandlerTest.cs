using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Commands.Users.MarketingPreferences;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.UnitTests.Commands.UserContact.MarketingPreferences
{
	internal class UpdateMarketingPreferencesCommandHandlerTest : CommandHandlerTest<UpdateMarketingPreferencesCommandHandler,
		UpdateMarketingPreferencesCommand>
	{
		[Test]
		[Theory]
		public async Task UpdateMarketingPreference(bool isEmailMarketingActive,
			bool isMobileMarketingActive,
			bool isSmsMarketingActive,
			bool isPostMarketingActive,
			bool isDoorToDoorMarketingActive,
			bool isLandlineMarketingActive)
		{
			AccountDto accountDto;
			var cmd = ArrangeAndGetCommand();

			await Context.Sut.ExecuteAsync(cmd);

			Assert();

			void Assert()
			{
				AssertUpdateMarketingPreference(CommunicationPreferenceType.SMS, cmd.SmsMarketingActive);
				AssertUpdateMarketingPreference(CommunicationPreferenceType.Mobile, cmd.MobileMarketingActive);
				AssertUpdateMarketingPreference(CommunicationPreferenceType.Post, cmd.PostMarketingActive);
				AssertUpdateMarketingPreference(CommunicationPreferenceType.DoorToDoor, cmd.DoorToDoorMarketingActive);
				AssertUpdateMarketingPreference(CommunicationPreferenceType.Email, cmd.EmailMarketingActive);
				AssertUpdateMarketingPreference(CommunicationPreferenceType.LandLine, cmd.LandLineMarketingActive);


				void AssertUpdateMarketingPreference(CommunicationPreferenceType preferenceType, bool isActive)
				{
					var repositoryOfCrmUmc = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();

					foreach (var permission in accountDto.CommunicationPermissions.Where(c => c.CommunicationChannelID == preferenceType))
					{
						repositoryOfCrmUmc.Verify(x => x.Delete(permission, true), Times.Once);
					}

					var acceptedStatus = isActive
						? CommunicationPermissionStatusType.Accepted
						: CommunicationPermissionStatusType.NotAccepted;


					repositoryOfCrmUmc.Verify(x => x.Add(It.Is<CommunicationPermissionDto>(m => 
											m.AccountID == accountDto.AccountID &&
											m.CommunicationChannelID == preferenceType &&
											m.CommunicationPermissionStatusID == acceptedStatus),
							true), Times.Once);
				}
			}

			UpdateMarketingPreferencesCommand ArrangeAndGetCommand()
			{
				var accountNumber = Context.Fixture.Create<string>();
				var accountId = Context.Fixture.Create<string>();
				var account = Context.Fixture.Build<AccountInfo>()
					.With(x => x.AccountNumber, accountNumber)
					.With(x => x.Partner, accountId)
					.With(x=>x.PointReferenceNumber,Context.Fixture.Create<ElectricityPointReferenceNumber>())
					//removed to improve the test perfomance since its not needed
					.Without(x=>x.BankAccounts)
					.Without(x=>x.BusinessAgreement)
					.Without(x=>x.NonSmartPeriods)
					.Without(x=>x.SmartPeriods)
					.Create();

				var domainFacade = new DomainFacade();
				domainFacade.SetUpMocker(Context.AutoMocker);
				domainFacade.QueryResolver.ExpectQuery(new AccountInfoQuery
				{
					AccountNumber = accountNumber
				},
					account.ToOneItemArray());

				accountDto = Context.Fixture
					.Build<AccountDto>()
					.With(x => x.AccountID, accountId)
					.With(x=>x.CommunicationPermissions, GetCommunicationPermissions())
					//removed to improve the test perfomance since its not needed
					.Without(x=>x.AccountAddressIndependentEmails)
					.Without(x=>x.AccountAddressIndependentFaxes)
					.Without(x=>x.AccountAddressIndependentMobilePhones)
					.Without(x=>x.AccountAddressIndependentPhones)
					.Without(x=>x.AccountAddresses)
					.Without(x=>x.QuotationItems)
					.Without(x=>x.PaymentCards)
					.Create();
				
				Context.CrmUmcRepoMock.Value.MockQuerySingle(accountDto);
				var accountQueryMock = Context.AutoMocker.GetMock<IFluentODataModelQuery<AccountDto>>();
				accountQueryMock.Setup(_ => _.GetOne()).ReturnsAsync(accountDto);

				return new UpdateMarketingPreferencesCommand(accountNumber,
					isSmsMarketingActive, 
					isLandlineMarketingActive, 
					isMobileMarketingActive, 
					isPostMarketingActive,
					isDoorToDoorMarketingActive, 
					isEmailMarketingActive);
			}

			List<CommunicationPermissionDto> GetCommunicationPermissions()
			{
				var communicationPermissions = new List<CommunicationPermissionDto>();
				foreach (CommunicationPreferenceType communicationPreferenceType in CommunicationPreferenceType.AllValues)
				{
					communicationPermissions.Add(Context
						.Fixture
						.Build<CommunicationPermissionDto>()
						.With(x => x.CommunicationChannelID, communicationPreferenceType)
						.With(x => x.CommunicationPermissionStatusID, CommunicationPermissionStatusType.Accepted)
						.Create());
				}

				return communicationPermissions;
			}
		}
	}
}

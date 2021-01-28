using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainServices.Commands.Platform.PublishBusinessActivity;
using EI.RP.DomainServices.Queries.User.PhoneMetaData;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Commands.Users.UserContact
{
	internal class UpdateUserContactDetailsCommandHandler : ICommandHandler<UpdateUserContactDetailsCommand>
	{
		private readonly ISapRepositoryOfCrmUmc _repositoryOfCrmUmc;
		private readonly IDomainQueryResolver _queryResolver;


		private readonly ICommandHandler<PublishBusinessActivityDomainCommand> _businessActivityPublisher;

		public UpdateUserContactDetailsCommandHandler(ISapRepositoryOfCrmUmc repositoryOfCrmUmc,
			IDomainQueryResolver domainQueryResolver, 
			ICommandHandler<PublishBusinessActivityDomainCommand> businessActivityPublisher)
		{
			_repositoryOfCrmUmc = repositoryOfCrmUmc;
			_queryResolver = domainQueryResolver;
			_businessActivityPublisher = businessActivityPublisher;
		}

		public async Task ExecuteAsync(UpdateUserContactDetailsCommand command)
		{
			var userContactInfoTask = GetContactInfo();


			var primaryContactNumberType =
				(PhoneMetadataType)(await _queryResolver.GetPhoneMetaDataType(command.PrimaryPhoneNumber))
				.ContactNumberType;
			var alternativeContactNumberType = PhoneMetadataType.Mobile;
			if (!string.IsNullOrEmpty(command.AlternativePhoneNumber))
			{
				alternativeContactNumberType =
					(PhoneMetadataType)(await _queryResolver.GetPhoneMetaDataType(command
						.AlternativePhoneNumber))
					.ContactNumberType;
			}

			if (alternativeContactNumberType == "Invalid" || primaryContactNumberType == "Invalid")
			{
				throw new DomainException(ResidentialDomainError.ContactNumberInvalid);
			}

			var userContactInfo = await userContactInfoTask;

			await UpdatePhoneDetails();
			await UpdateEmail();


			await SubmitBusinessActivity(command.AccountNumber);

			#region LocalFunctions
			
			async Task DeleteAccountAddressDependentPhones()
			{
				var lstAccountAddressDependentPhonesDtos = userContactInfo.StandardAccountAddress
					.AccountAddressDependentPhones.ToArray();

				foreach (var item in lstAccountAddressDependentPhonesDtos)
				{
					await _repositoryOfCrmUmc.Delete(item);
				}
			}


			async Task DeleteAccountAddressDependentMobiles()
			{
				var lstAccountAddressDependentMobileDtos = userContactInfo.StandardAccountAddress
					.AccountAddressDependentMobilePhones.ToArray();

				foreach (var item in lstAccountAddressDependentMobileDtos)
				{
					await _repositoryOfCrmUmc.Delete(item);
				}
			}

			async Task DeleteDuplicatePhones()
			{
				if (userContactInfo != null)
				{
					await DeleteAccountAddressDependentPhones();
					await DeleteAccountAddressDependentMobiles();
				}
			}

			async Task UpdatePhoneDetails()
			{
				await DeleteDuplicatePhones();

				var accountInfo =
					await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);
				if (primaryContactNumberType == PhoneMetadataType.Mobile &&
					alternativeContactNumberType == PhoneMetadataType.Mobile)
				{
					AccountAddressDependentMobilePhoneDto accountAddressPrimaryPhone =
						ResolveAccountAddressDependentMobilePhoneDto(command.PrimaryPhoneNumber, accountInfo,
							PhoneType.DefaultMobilePhone, true, true);

					await _repositoryOfCrmUmc.Add(accountAddressPrimaryPhone);

					if (!string.IsNullOrEmpty(command.AlternativePhoneNumber))
					{
						accountAddressPrimaryPhone =
							ResolveAccountAddressDependentMobilePhoneDto(command.AlternativePhoneNumber,
								accountInfo,
								PhoneType.NotDefaultMobilePhone, false, false);

						await _repositoryOfCrmUmc.Add(accountAddressPrimaryPhone);
					}
				}
				else if (primaryContactNumberType == PhoneMetadataType.Mobile &&
						 alternativeContactNumberType == PhoneMetadataType.LandLine)
				{
					AccountAddressDependentMobilePhoneDto accountAddressPrimaryPhone =
						ResolveAccountAddressDependentMobilePhoneDto(command.PrimaryPhoneNumber, accountInfo,
							PhoneType.DefaultMobilePhone, true, true);

					await _repositoryOfCrmUmc.Add(accountAddressPrimaryPhone);
					if (!string.IsNullOrEmpty(command.AlternativePhoneNumber))
					{
						AccountAddressDependentPhoneDto accountAddressAlternativePhone =
							ResolveAccountAddressDependentLandLinePhone(command.AlternativePhoneNumber,
								accountInfo,
								PhoneType.DefaultLandlinePhone, false);

						await _repositoryOfCrmUmc.Add(accountAddressAlternativePhone);
					}
				}
				else if (primaryContactNumberType == PhoneMetadataType.LandLine &&
						 alternativeContactNumberType == PhoneMetadataType.Mobile)
				{
					AccountAddressDependentPhoneDto accountAddressPrimaryPhone =
						ResolveAccountAddressDependentLandLinePhone(command.PrimaryPhoneNumber, accountInfo,
							PhoneType.DefaultLandlinePhone, true);

					await _repositoryOfCrmUmc.Add(accountAddressPrimaryPhone);
					if (!string.IsNullOrEmpty(command.AlternativePhoneNumber))
					{
						AccountAddressDependentMobilePhoneDto accountAddressAlternativePhone =
							ResolveAccountAddressDependentMobilePhoneDto(command.AlternativePhoneNumber,
								accountInfo,
								PhoneType.DefaultMobilePhone, false, true);

						await _repositoryOfCrmUmc.Add(accountAddressAlternativePhone);
					}
				}
				else
				{
					AccountAddressDependentPhoneDto accountAddressPrimaryPhone =
						ResolveAccountAddressDependentLandLinePhone(command.PrimaryPhoneNumber, accountInfo,
							PhoneType.DefaultLandlinePhone, true);
					await _repositoryOfCrmUmc.Add(accountAddressPrimaryPhone);

					if (!string.IsNullOrEmpty(command.AlternativePhoneNumber))
					{
						AccountAddressDependentPhoneDto accountAddressAlternativePhone =
							ResolveAccountAddressDependentLandLinePhone(command.AlternativePhoneNumber,
								accountInfo,
								string.Empty, false);

						await _repositoryOfCrmUmc.Add(accountAddressAlternativePhone);
					}
				}
			}

			async Task UpdateEmail()
			{
				//email update
				if (command.ContactEMail != command.PreviousContactEMail)
				{
					var accountInfo =
						await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber,
							byPassPipeline: true);

					AccountAddressDependentEmailDto accountAdressDependantEmail =
						new AccountAddressDependentEmailDto();
					accountAdressDependantEmail.AccountID = accountInfo.Partner;
					accountAdressDependantEmail.Email = command.ContactEMail;
					accountAdressDependantEmail.StandardFlag = true;
					accountAdressDependantEmail.HomeFlag = true;
					accountAdressDependantEmail.SequenceNo = string.Empty;
					accountAdressDependantEmail.AddressID = accountInfo.BillToAccountAddressId;
					await _repositoryOfCrmUmc.Add(accountAdressDependantEmail);
				}
			}

			#endregion LocalFunctions

			async Task<AccountDto> GetContactInfo()
			{
				var accountInfo =
					await _queryResolver.GetAccountInfoByAccountNumber(command.AccountNumber, true);

				return await _repositoryOfCrmUmc
					.NewQuery<AccountDto>()
					.Key(accountInfo.Partner)
					.Expand(x => x.StandardAccountAddress.AccountAddressDependentEmails)
					.Expand(x => x.StandardAccountAddress.AccountAddressDependentMobilePhones)
					.Expand(x => x.StandardAccountAddress.AccountAddressDependentPhones)
					.GetOne();

			}
		}

		private static AccountAddressDependentPhoneDto ResolveAccountAddressDependentLandLinePhone(string phoneNumber,
			AccountInfo accountInfo, string phoneType, bool standardFlag)
		{
			AccountAddressDependentPhoneDto accountAddressLandLinePhone = new AccountAddressDependentPhoneDto();
			accountAddressLandLinePhone.AccountID = accountInfo.Partner;
			accountAddressLandLinePhone.AddressID = accountInfo.BillToAccountAddressId;
			accountAddressLandLinePhone.CompletePhoneNo = string.Empty;
			if (phoneNumber.StartsWith("0044") || phoneNumber.StartsWith("+44"))
			{
				accountAddressLandLinePhone.CountryID = CountryIdType.GB;
			}
			else
			{
				accountAddressLandLinePhone.CountryID = CountryIdType.IE;
			}

			accountAddressLandLinePhone.Extension = string.Empty;
			accountAddressLandLinePhone.HomeFlag = true;
			accountAddressLandLinePhone.PhoneNo = phoneNumber;
			accountAddressLandLinePhone.PhoneType = phoneType;
			accountAddressLandLinePhone.SequenceNo = string.Empty;
			accountAddressLandLinePhone.StandardFlag = standardFlag;
			return accountAddressLandLinePhone;
		}


		private static AccountAddressDependentMobilePhoneDto ResolveAccountAddressDependentMobilePhoneDto(
			string phoneNumber,
			AccountInfo accountInfo, string phoneType, bool standardFlag, bool defaultFlag)
		{
			AccountAddressDependentMobilePhoneDto accountAddressPrimaryPhone =
				new AccountAddressDependentMobilePhoneDto();
			accountAddressPrimaryPhone.AccountID = accountInfo.Partner;
			accountAddressPrimaryPhone.AddressID = accountInfo.BillToAccountAddressId;
			accountAddressPrimaryPhone.CompletePhoneNo = string.Empty;
			if (phoneNumber.StartsWith("0044") || phoneNumber.StartsWith("+44"))
			{
				accountAddressPrimaryPhone.CountryID = CountryIdType.GB;
			}
			else
			{
				accountAddressPrimaryPhone.CountryID = CountryIdType.IE;
			}

			accountAddressPrimaryPhone.Extension = string.Empty;
			accountAddressPrimaryPhone.HomeFlag = true;
			accountAddressPrimaryPhone.PhoneNo = phoneNumber;
			accountAddressPrimaryPhone.PhoneType = phoneType;
			accountAddressPrimaryPhone.SequenceNo = string.Empty;
			accountAddressPrimaryPhone.StandardFlag = standardFlag;
			accountAddressPrimaryPhone.DefaultFlag = defaultFlag;
			return accountAddressPrimaryPhone;
		}

		private async Task SubmitBusinessActivity(string accountNumber)
		{
			var accountInfo =
				await _queryResolver.GetAccountInfoByAccountNumber(accountNumber, true);
			var businessActivityType =
				PublishBusinessActivityDomainCommand.BusinessActivityType.UpdateUserContactDetails;

			await _businessActivityPublisher.ExecuteAsync(new PublishBusinessActivityDomainCommand(businessActivityType,
				accountInfo.Partner, accountNumber));
		}
	}
}
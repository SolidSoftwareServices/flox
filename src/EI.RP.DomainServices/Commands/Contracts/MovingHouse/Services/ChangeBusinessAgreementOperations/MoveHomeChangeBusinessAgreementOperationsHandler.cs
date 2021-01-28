using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ActivityOperations;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context;
using EI.RP.DomainServices.Queries.Metering.Premises;
using AccountAddressDependentEmailDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AccountAddressDependentEmailDto;
using AccountAddressDto = EI.RP.DataModels.Sap.CrmUmc.Dtos.AccountAddressDto;

namespace EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.ChangeBusinessAgreementOperations
{
	class MoveHomeChangeBusinessAgreementOperationsHandler : IMoveHomeChangeBusinessAgreementOperationsHandler
	{
		private readonly ISapRepositoryOfErpUmc _erpUmc;
		private readonly ISapRepositoryOfCrmUmc _crmUmc;
		private readonly IMoveHomeActivityPublisher _activityPublisher;
		private readonly IDomainQueryResolver _queryResolver;

		public MoveHomeChangeBusinessAgreementOperationsHandler(ISapRepositoryOfErpUmc erpUmc, ISapRepositoryOfCrmUmc crmUmc,IMoveHomeActivityPublisher activityPublisher, IDomainQueryResolver queryResolver)
		{
			_erpUmc = erpUmc;
			_crmUmc = crmUmc;
			_activityPublisher = activityPublisher;
			_queryResolver = queryResolver;
		}

		public async Task SetNewAddressAndBusinessAgreementChanges(MoveHouse commandData)
		{
			var context = commandData.Context;
			try
			{
				var accountEx = commandData.ContractAccountType == ClientAccountType.Gas
					? context.Gas
					: context.Electricity;
				var businessAgreement = (await _crmUmc.NewQuery<BusinessAgreementDto>()
					.Key(accountEx.Account.AccountNumber)
					.Expand(x => x.Account.AccountAddresses)
					.Expand(x => x.Account.StandardAccountAddress.AccountAddressDependentEmails)
					.GetMany()).FirstOrDefault();

				var currentEmail = businessAgreement?.Account.StandardAccountAddress
					                   .AccountAddressDependentEmails.FirstOrDefault(x => x.StandardFlag)?.Email ??
				                   string.Empty;
				await SetAccountAddressesToBlank(accountEx);
				await SubmitNewAddress(accountEx);
				await SubmitAccountAddressDependentEmail(accountEx, currentEmail);
			}
			catch (DomainException ex)
			{
				//second attempt
				if (!ex.DomainError.Equals(ResidentialDomainError.DataAlreadyReleased))
				{
					await _activityPublisher.SubmitActivityError(commandData, ex);
					throw;
				}
			}

			return;
			async Task SubmitNewAddress(CompleteMoveHouseContext.ExpandedAccount accountEx)
			{

				var dto = await MapAddress();
				await _crmUmc.AddThenGet(dto);

				async Task<AccountAddressDto> MapAddress()
				{
					var prn = accountEx.Account.ClientAccountType == ClientAccountType.Electricity
						? (PointReferenceNumber)context.NewPrns.NewMprn
						: (PointReferenceNumber)context.NewPrns.NewGprn;
					var premise = await _queryResolver.GetPremiseByPrn(prn, true);

					var premiseAddress = premise.Address;
					var newAddress = new AccountAddressDto(){AddressInfo= new EI.RP.DataModels.Sap.CrmUmc.Dtos.AddressInfoDto()};
					var addressInfo = newAddress.AddressInfo;
					addressInfo.StandardFlag = AddressFlag.StandardFlag;
					

					addressInfo.Street =
						premiseAddress.Street == null ? string.Empty : premiseAddress.Street.ToUpper();
					addressInfo.City = premiseAddress.City == null ? string.Empty : premiseAddress.City.ToUpper();
					addressInfo.CountryID = string.IsNullOrWhiteSpace(premiseAddress.Country)? CountryIdType.IE: premiseAddress.Country.ToUpper();

					addressInfo.PostalCode = premiseAddress.PostalCode == null
								? string.Empty
								: premiseAddress.PostalCode.ToUpper();


					addressInfo.AddressLine1 =
						premiseAddress.AddressLine1 == null ? string.Empty : premiseAddress.AddressLine1.ToUpper();
					addressInfo.AddressLine2 =
						premiseAddress.AddressLine2 == null ? string.Empty : premiseAddress.AddressLine2.ToUpper();
					addressInfo.AddressLine4 =
						premiseAddress.AddressLine4 == null ? string.Empty : premiseAddress.AddressLine4.ToUpper();
					addressInfo.AddressLine5 =
						premiseAddress.AddressLine5 == null ? string.Empty : premiseAddress.AddressLine5.ToUpper();
					addressInfo.POBoxPostalCode = premiseAddress.POBoxPostalCode == null
						? string.Empty
						: premiseAddress.POBoxPostalCode.ToUpper();
					addressInfo.POBox = premiseAddress.POBox == null ? string.Empty : premiseAddress.POBox.ToUpper();

					addressInfo.HouseNo =
						premiseAddress.HouseNo == null ? string.Empty : premiseAddress.HouseNo.ToUpper();
					addressInfo.Region =
						premiseAddress.Region == null ? string.Empty : premiseAddress.Region.ToUpper();
					addressInfo.District =
						premiseAddress.District == null ? string.Empty : premiseAddress.District.ToUpper();


					newAddress.AccountID = accountEx.Account.Partner;

					return newAddress;
				}
			}

			async Task SubmitAccountAddressDependentEmail(CompleteMoveHouseContext.ExpandedAccount accountEx,
				string currentEmail)
			{
				var businessAgreement = (await _crmUmc.NewQuery<BusinessAgreementDto>()
					.Key(accountEx.Account.AccountNumber)
					.Expand(x => x.Account.AccountAddresses)
					.Expand(x => x.Account.StandardAccountAddress.AccountAddressDependentEmails)
					.GetMany()).FirstOrDefault();


				var email = new AccountAddressDependentEmailDto();
				email.AccountID = accountEx.Account.Partner;
				email.AddressID = businessAgreement?.Account.AccountAddresses.FirstOrDefault()?.AddressID ??
								  string.Empty;
				email.HomeFlag = true;
				email.StandardFlag = true;
				email.Email = currentEmail;


				await _crmUmc.Add(email);
			}

			async Task SetAccountAddressesToBlank(CompleteMoveHouseContext.ExpandedAccount accountEx)
			{
				var contract = await _erpUmc.NewQuery<ContractAccountDto>()
						.Key(accountEx.Account.AccountNumber)
						.GetOne();
				await _erpUmc.Update(contract);
			}
		}

	}
}
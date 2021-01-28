using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.User;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.User.UserContact
{
    internal class UserContactDetailsQueryHandler : QueryHandler<UserContactDetailsQuery>
    {
        private readonly IDomainQueryResolver _domainQueryResolver;
        private readonly ISapRepositoryOfCrmUmc _repositoryOfCrmUmc;

        public UserContactDetailsQueryHandler(ISapRepositoryOfCrmUmc repositoryOfCrmUmc, IDomainQueryResolver domainQueryResolver)
        {
            _repositoryOfCrmUmc = repositoryOfCrmUmc;
            _domainQueryResolver = domainQueryResolver;
        }

        protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(UserContactDetailsQuery query)
        {
            var accountInfo = await _domainQueryResolver.GetAccountInfoByAccountNumber(query.AccountNumber);

            var userContactInfoTask = _repositoryOfCrmUmc.NewQuery<AccountDto>().Key(accountInfo.Partner)
                .Expand(x => x.StandardAccountAddress.AccountAddressDependentEmails)
                .Expand(x => x.StandardAccountAddress.AccountAddressDependentMobilePhones)
                .Expand(x => x.StandardAccountAddress.AccountAddressDependentPhones)
                .Expand(x => x.CommunicationPermissions[0].CommunicationChannel)
                .Expand(x => x.CommunicationPermissions[0].CommunicationPermissionStatus)
                .GetOne();

            var result = await MapToUserDetails(await userContactInfoTask);

            return result.ToOneItemArray().Cast<TQueryResult>();

        }

        private async Task<UserContactDetails> MapToUserDetails(AccountDto source)
        {
            var userAccountInfo = new UserContactDetails();
            userAccountInfo.LoginEMail = source.Username;
            userAccountInfo.ContactEMail = source.StandardAccountAddress?.AccountAddressDependentEmails
                ?.LastOrDefault(x => x.StandardFlag)?.Email;

            var mobilePhone = GetAccountAddressDependentMobilePhones(true);
            var phone = GetAccountAddressDependentPhone(true);

            userAccountInfo.PrimaryPhoneNumber = ResolvePrimaryPhoneNumber();

            var alternateMobile = GetAccountAddressDependentMobilePhones(false);
            var alternatePhone = GetAccountAddressDependentPhone(false);

            userAccountInfo.AlternativePhoneNumber = ResolveAlternatePhoneNumber();

            var communicationPreferencesDto = source.CommunicationPermissions.ToArray();
            var communicationPreferences = new List<CommunicationPreference>();
            var communicationPermissionStatusTypes = CommunicationPreferenceType.AllValues.Cast<CommunicationPreferenceType>().ToArray();
            foreach (var item in communicationPermissionStatusTypes)
            {
                var communicationPreference = new CommunicationPreference();
                var communicationPreferenceExistInDto = communicationPreferencesDto.FirstOrDefault(x =>
                    string.Equals(x.CommunicationChannelID, item.ToString(),
                        StringComparison.InvariantCultureIgnoreCase));

                communicationPreference.Accepted = communicationPreferenceExistInDto != null && communicationPreferenceExistInDto.CommunicationPermissionStatusID == CommunicationPermissionStatusType.Accepted;
                communicationPreference.PreferenceType = item;
                communicationPreferences.Add(communicationPreference);
            }

            userAccountInfo.CommunicationPreference = communicationPreferences;

            AccountAddressDependentPhoneDto GetAccountAddressDependentPhone(bool isStandardFlag)
            {
                return
                     source.StandardAccountAddress?.AccountAddressDependentPhones.LastOrDefault(x => x.StandardFlag == isStandardFlag);
            }

            AccountAddressDependentMobilePhoneDto GetAccountAddressDependentMobilePhones(bool isStandardFlag)
            {
                return
                    source.StandardAccountAddress?.AccountAddressDependentMobilePhones.LastOrDefault(x => x.StandardFlag == isStandardFlag);
            }

            string ResolvePrimaryPhoneNumber()
            {
                if (mobilePhone != null && phone != null)
                {
                    return userAccountInfo.PrimaryPhoneNumber =
                        Convert.ToInt16(mobilePhone.SequenceNo) > Convert.ToInt16(phone.SequenceNo)
                            ? mobilePhone.PhoneNo
                            : phone.PhoneNo;
                }

                if (mobilePhone != null)
                {
	                return userAccountInfo.PrimaryPhoneNumber = mobilePhone.PhoneNo;
                }
                if (phone != null)
                {
	                return userAccountInfo.PrimaryPhoneNumber = phone.PhoneNo;
                }

                return string.Empty;
            }

            string ResolveAlternatePhoneNumber()
            {
                if (alternateMobile != null && alternatePhone != null)
                {
                    return userAccountInfo.AlternativePhoneNumber =
                        Convert.ToInt16(alternateMobile.SequenceNo) > Convert.ToInt16(alternatePhone.SequenceNo)
                            ? alternateMobile.PhoneNo
                            : alternatePhone.PhoneNo;
                }

                if (alternateMobile != null)
                {
	                return userAccountInfo.AlternativePhoneNumber = alternateMobile.PhoneNo;
                }
                if (alternatePhone != null)
                {
	                return userAccountInfo.AlternativePhoneNumber = alternatePhone.PhoneNo;
                }

                return string.Empty;
            }

            return userAccountInfo;
        }


        protected override Type[] ValidQueryResultTypes { get; } = { typeof(UserContactDetails) };
    }
}

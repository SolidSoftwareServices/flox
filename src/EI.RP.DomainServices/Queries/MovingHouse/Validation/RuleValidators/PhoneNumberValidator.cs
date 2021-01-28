using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.User.PhoneMetaData;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
    public class PhoneNumberValidator : IMovingHouseValidator
    {
        private readonly IDomainQueryResolver _queryResolver;

        public PhoneNumberValidator(IDomainQueryResolver queryResolver)
        {
            _queryResolver = queryResolver;
        }
        public string ResolveCacheKey(MovingHouseValidationQuery query)
        {
	        return $"{query.ValidatePhoneNumber}-{query.PhoneNumber}";
        }
        public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
        {
            var movingHouseValidationType = MovingHouseValidationType.PhoneNumberIsValid;
            if (!query.ValidatePhoneNumber)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.NotExecuted,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            if (!query.IsValidQuery(out var validationFailedReason) ||
                !await IsPhoneNumberValid(query.PhoneNumber))
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.Failed,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            return new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = movingHouseValidationType
            };
        }

        private async Task<bool> IsPhoneNumberValid(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            
            var phoneNumberType = await _queryResolver.GetPhoneMetaDataType(phoneNumber);
            if (phoneNumberType.ContactNumberType.Equals(PhoneMetadataType.Invalid, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}

using System;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse;
using EI.RP.DomainServices.Commands.Contracts.MovingHouse.Services.Context;
using EI.RP.DomainServices.InternalShared.ContractSales;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using EI.RP.DomainServices.Queries.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.MovingHouse.Validation.RuleValidators
{
	internal class ContractSaleChecksValidator : IMovingHouseValidator
	{
        private readonly IDomainQueryResolver _queryResolver;
        private readonly IContractSaleCommand _contractSaleBuilder;
        private readonly ICompleteMovingHouseContextFactory _context;

        public ContractSaleChecksValidator(
            IDomainQueryResolver queryResolver,
            IContractSaleCommand contractSaleBuilder,
            ICompleteMovingHouseContextFactory context)
        {
            _queryResolver = queryResolver;
            _contractSaleBuilder = contractSaleBuilder;
            _context = context;
        }
        public string ResolveCacheKey(MovingHouseValidationQuery query)
        {
	        return $"{query.ElectricityAccountNumber}-{query.GasAccountNumber}-{query.ValidateContractSaleChecks}-{query.InitiatedFromAccountNumber}-{query.MovingHouseType}";
        }
		public async Task<MovingHouseRulesValidationResult> Resolve(MovingHouseValidationQuery query)
		{
            var movingHouseValidationType = MovingHouseValidationType.IsContractSaleChecksOk;
            if (!query.ValidateContractSaleChecks)
            {
                return new MovingHouseRulesValidationResult
                {
                    Output = OutputType.NotExecuted,
                    MovingHouseValidationType = movingHouseValidationType
                };
            }

            var isContractSaleChecksOk = await SaleChecks(query.InitiatedFromAccountNumber, query.ElectricityAccountNumber, query.GasAccountNumber, query.MovingHouseType);

            return new MovingHouseRulesValidationResult
			{
				Output = isContractSaleChecksOk,
				MovingHouseValidationType = movingHouseValidationType
            };
		}

        private async Task<OutputType> SaleChecks(string initiatedFromAccountNumber, string electricityAccountNumber, 
                                                  string gasAccountNumber, MovingHouseType movingHouseType)
        {
            if (string.IsNullOrEmpty(initiatedFromAccountNumber)) return OutputType.Failed;

            var clientAccountType = (await _queryResolver.GetAccountInfoByAccountNumber(initiatedFromAccountNumber)).ClientAccountType;

            var moveHouse = new MoveHouse(
                electricityAccountNumber,
                gasAccountNumber,
                movingHouseType,
                null,
                clientAccountType);

            moveHouse.Context = await _context.Resolve(moveHouse);

            try
            {
                var contractSaleInfo = await _contractSaleBuilder.ResolveContractSaleChecks(moveHouse, false);
            }
            catch (DomainException ex)
            {
                if (ex.DomainError.Equals(ResidentialDomainError.ContractErrorPreventTheContactFromBeingSubmitted))
                {
                    return OutputType.Failed;
                }
                throw;
            }

            return OutputType.Passed;
        }
    }
}
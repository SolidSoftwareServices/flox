using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.CanCloseAccount
{
	internal class AccountsCloseableInfoQueryHandler : QueryHandler<CanCloseAccountQuery>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IDomainQueryResolver _queryResolver;
		private readonly ISapRepositoryOfCrmUmc _crmUmc;
		public AccountsCloseableInfoQueryHandler(IDomainQueryResolver queryResolver, ISapRepositoryOfCrmUmc crmUmc)
		{
			_queryResolver = queryResolver;
			_crmUmc = crmUmc;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(AccountsCloseableInfo)};



		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(CanCloseAccountQuery query)
		{
			var electricityAccountTask = query.HasElectricityAccount()?_queryResolver.GetAccountInfoByAccountNumber(query.ElectricityAccountNumber, true):Task.FromResult((AccountInfo)null);
			var gasAccountTask = query.HasGasAccount()? _queryResolver.GetAccountInfoByAccountNumber(query.GasAccountNumber, true):Task.FromResult((AccountInfo)null);
			var moveOutCheck = new MOUPFChecksFunction
			{
				Query =
				{
					ContractElecID = (await electricityAccountTask)?.ContractId??string.Empty ,
					ContractGasID = (await gasAccountTask)?.ContractId??string.Empty,
					MoveOutDate = query.ClosingDate
				}
			};

            try
            {
                var checkResult = await _crmUmc.ExecuteFunctionWithSingleResult(moveOutCheck, false);
                var accountsCloseableInfo = new AccountsCloseableInfo
                {
                    ElectricityAccountNumber = query.ElectricityAccountNumber,
                    GasAccountNumber = query.GasAccountNumber,

                    ElectricityEffectiveClosingDate = checkResult.MODate_Elec,
                    GasEffectiveClosingDate = checkResult.MODate_Gas,

                    CanClose = true,
                    UpfLcpe = checkResult.UpfLcpe
                };
                return (accountsCloseableInfo as TQueryResult).ToOneItemArray();
            }
            catch (DomainException ex)
            {
                if (ex.DomainError.Equals(ResidentialDomainError.CantProcessMoveInMoveOut))
                {
                    var accountsCloseableInfo = new AccountsCloseableInfo
                    {
                        ElectricityAccountNumber = query.ElectricityAccountNumber,
                        GasAccountNumber = query.GasAccountNumber,

                        CanClose = false,
                        ReasonCannotClose = ResidentialDomainError.CantProcessMoveInMoveOut.ErrorMessage,
                    };
                    return (accountsCloseableInfo as TQueryResult).ToOneItemArray();
                }
                throw;
            }
        }
    }
}
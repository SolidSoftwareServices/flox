using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataModels.Sap.ErpUmc.Functions;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.InternalShared.Accounts;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using DateTime = System.DateTime;

namespace EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo
{
	internal class EqualizerPaymentSetupInfoQueryHandler : QueryHandler<EqualizerPaymentSetupInfoQuery>
	{
		private readonly ISapRepositoryOfErpUmc _erpRepository;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IAccountExtraInfoResolver _accountExtraInfoResolver;

		public EqualizerPaymentSetupInfoQueryHandler(
			ISapRepositoryOfErpUmc erpRepository,
			IDomainQueryResolver queryResolver,
			IAccountExtraInfoResolver accountExtraInfoResolver)
		{
			_erpRepository = erpRepository;
			_queryResolver = queryResolver;
			_accountExtraInfoResolver = accountExtraInfoResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } = {typeof(EqualizerPaymentSetupInfo)};

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
			EqualizerPaymentSetupInfoQuery query)
		{
			var accountInfo = await _queryResolver.GetAccountInfoByAccountNumber(query.AccountNumber, true);

			var result = new EqualizerPaymentSetupInfo
			{
				AccountNumber = query.AccountNumber,
				CanSetUpEqualizer = await _accountExtraInfoResolver.CanEqualizePayments(accountInfo)
			};
            if (result.CanSetUpEqualizer)
            {
                var maxContractEndDate = SapDateTimes.SapDateTimeMax;
                if (accountInfo != null && accountInfo.ContractEndDate == maxContractEndDate)
                {
                    var equaliserScheme = await ResolveEqualiserScheme();

                    result.Amount = equaliserScheme.Amount;
                    result.StartDate = equaliserScheme.StartDate;
                    result.ContractId = equaliserScheme.ContractID;
                }
            }

            return result.ToOneItemArray().Cast<TQueryResult>();

			async Task<PaymentSchemeDto> ResolveEqualiserScheme()
			{
				var spQuery = new SimulatePaymentSchemeFunction
				{
					Query =
					{
						ContractID = accountInfo.ContractId,
						Frequency = PaymentSchemeFrequency.Monthly,
						FirstDueDate = query.FirstPaymentDateTime,
                        Category = PaymentSchemeCategoryType.MEQCategoryType
					}
				};

                if (spQuery.Query.StartDate==default(DateTime))
                {
                    spQuery.Query.StartDate = DateTime.Today.Date;
                }

				var paymentScheme = await _erpRepository.ExecuteFunctionWithSingleResult(spQuery);
				return paymentScheme;
			}
		}
	}
}
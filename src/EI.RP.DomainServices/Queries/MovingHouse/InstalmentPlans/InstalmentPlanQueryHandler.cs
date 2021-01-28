using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.DomainServices.Queries.Billing.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.DomainServices.Queries.MovingHouse.InstalmentPlans
{
	internal class InstalmentPlanQueryHandler : QueryHandler<InstalmentPlanQuery>
	{
        private readonly IDomainQueryResolver _queryResolver;

        public InstalmentPlanQueryHandler(IDomainQueryResolver queryResolver)
		{
            _queryResolver = queryResolver;
		}

		protected override Type[] ValidQueryResultTypes { get; } = { typeof(InstalmentPlanRequestResult) };

		protected override async Task<IEnumerable<TQueryResult>> _ExecuteQueryAsync<TQueryResult>(
            InstalmentPlanQuery queryModel)
		{
            var getInvoicesTask = _queryResolver.GetInvoicesByAccountNumber(queryModel.AccountNumber);
            var hasNonPaidInvoicesWithSubstitueDocuments = (await getInvoicesTask).Any(x=>x.InvoiceStatus != InvoiceStatus.Paid && x.SubstituteDocument != string.Empty);

            var requestResult = new InstalmentPlanRequestResult()
            {
                HasInstalmentPlan = hasNonPaidInvoicesWithSubstitueDocuments
            };

            return new[]
			{
                requestResult
            }.Cast<TQueryResult>().ToArray();
		}
	}
}
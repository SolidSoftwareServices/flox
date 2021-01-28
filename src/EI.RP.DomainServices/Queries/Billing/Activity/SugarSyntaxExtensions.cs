using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Billing;

namespace EI.RP.DomainServices.Queries.Billing.Activity
{
    /// <summary>
    ///     simplify Domain queries here
    /// </summary>
    public static class SugarSyntaxExtensions
    {
        public static async Task<IEnumerable<AccountBillingActivity>> GetInvoicesByAccountNumber(this IDomainQueryResolver provider, string accountNumber, DateTime? minDate = null,DateTime? maxDate = null, bool byPassPipeline = false)
        {
            var query = new AccountBillingActivityQuery
            {
                AccountNumber = accountNumber,
                Source = AccountBillingActivity.ActivitySource.Invoice
            };


            query.MinDate = minDate ?? query.MinDate;
            query.MaxDate = maxDate ?? query.MaxDate;

            var items = await provider
                .FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(query,byPassPipeline);

            return items;
        }

   
        public static async Task<IEnumerable<AccountBillingActivity>> GetAccountBillingsByAccountNumber(
            this IDomainQueryResolver provider, string accountNumber, bool byPassPipeline = false)
        {
            var query = new AccountBillingActivityQuery
            {
                AccountNumber = accountNumber
            };
            var accountBillingActivities = await provider
                .FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(query,byPassPipeline);
            return accountBillingActivities;
        }
     

    }
}
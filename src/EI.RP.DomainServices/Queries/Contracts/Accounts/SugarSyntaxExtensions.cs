using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static async Task<AccountInfo> GetAccountInfoByAccountNumber(this IDomainQueryResolver provider,
			string accountNumber, bool byPassPipeline = false)
		{
			if (accountNumber == null) throw new ArgumentNullException(nameof(accountNumber));

			var accountInfos = await provider
				.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
				{
					AccountNumber = accountNumber
				}, byPassPipeline);
			return accountInfos.SingleOrDefault();
		}
		public static async Task<IEnumerable<AccountInfo>> GetAccountInfoByBusinessPartner(this IDomainQueryResolver provider,
			string businessPartner, bool byPassPipeline = false)
		{
			return await GetByBusinessPartner<AccountInfo>(provider, businessPartner, byPassPipeline);
		}

	
		public static async Task<IEnumerable<AccountOverview>> GetAccountOverViewByBusinessPartner(this IDomainQueryResolver provider,
			string businessPartner, bool byPassPipeline = false)
		{
			return await GetByBusinessPartner<AccountOverview>(provider, businessPartner, byPassPipeline);
		}
		private static async Task<IEnumerable<TResult>> GetByBusinessPartner<TResult>(IDomainQueryResolver provider, string businessPartner,
			bool byPassPipeline) where TResult:AccountBase
		{
			if (businessPartner == null) throw new ArgumentNullException(nameof(businessPartner));

			var accountInfos = await provider
				.FetchAsync<AccountInfoQuery, TResult>(new AccountInfoQuery
				{
					BusinessPartner = businessPartner
				}, byPassPipeline);
			return accountInfos;
		}

		public static async Task<AccountInfo> GetAccountInfoByPrn(this IDomainQueryResolver provider,
			PointReferenceNumber prn, bool byPassPipeline = false)
		{
			if (prn == null) throw new ArgumentNullException(nameof(prn));

			var accountInfos = await provider
				.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
				{
					Prn = prn
				}, byPassPipeline);
			return accountInfos.SingleOrDefault();
		}


		public static async Task<IEnumerable<AccountInfo>> GetAccounts(this IDomainQueryResolver provider,
			bool onlyOpened = false, bool byPassPipeline = false)
		{
			var accountInfos = await provider
				.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
				{
					Opened = onlyOpened ? true : (bool?) null
				}, byPassPipeline);
			return accountInfos.ToArray();
		}

		public static async Task<IEnumerable<AccountOverview>> GetAccountsOverview(this IDomainQueryResolver provider,
			bool onlyOpened = false, bool byPassPipeline = false)
		{
			var accountInfos = await provider
				.FetchAsync<AccountInfoQuery, AccountOverview>(new AccountInfoQuery
				{
					Opened = onlyOpened ? true : (bool?) null
				}, byPassPipeline);
			return accountInfos.ToArray();
		}

		/// <summary>
		/// It retrieves account info data for dual fuel accounts.
		/// </summary>
		/// <param name="ignoreAddressMismatch">It enables or disables  address match validation in DualFual boundled contracts.</param>
		/// <returns></returns>
		public static async Task<IEnumerable<AccountInfo>> GetDuelFuelAccountsByAccountNumber(
			this IDomainQueryResolver provider, string accountNumber, bool ignoreDuelFuelAddressMismatch = true, bool byPassPipeline = false)
		{
			if (accountNumber == null) throw new ArgumentNullException(nameof(accountNumber));
			var accountInfos = await provider
				.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
				{
					AccountNumber = accountNumber,
					RetrieveDuelFuelSisterAccounts = true,
					IgnoreDuelFuelAddressMismatch = ignoreDuelFuelAddressMismatch
				}, byPassPipeline);
			return accountInfos.ToArray();
		}		

		public static async Task<IEnumerable<AccountInfo>> GetDuelFuelAccountsByPrn(this IDomainQueryResolver provider,
			PointReferenceNumber prn, bool byPassPipeline = false)
		{
			if (prn == null) throw new ArgumentNullException(nameof(prn));
			var accountInfos = await provider
				.FetchAsync<AccountInfoQuery, AccountInfo>(new AccountInfoQuery
				{
					Prn = prn,
					RetrieveDuelFuelSisterAccounts = true
				}, byPassPipeline);
			return accountInfos.ToArray();
		}
	}
}
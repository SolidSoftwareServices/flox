using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.DomainServices.Queries.Contracts.BusinessPartners
{
	/// <summary>
	///     simplify Domain queries here
	/// </summary>
	public static class SugarSyntaxExtensions
	{
		public static async Task<IEnumerable<BusinessPartner>> GetBusinessPartners(this IDomainQueryResolver provider,
			int maxRecords, string userName=null, string lastName = null,
			string city = null, string houseNumber = null, string numPartner = null, string street = null,
			bool byPassPipeline = false)
		{
			var query = new BusinessPartnerQuery
			{
				City = city,
				HouseNum = houseNumber,
				LastName = lastName ?? string.Empty,
				PartnerNum =numPartner,
				Street = street,
				UserName =userName,
				NumberOfRows = maxRecords
			};
			return await provider.FetchAsync<BusinessPartnerQuery, BusinessPartner>(query, byPassPipeline);
			
		}
		public static async Task<BusinessPartner> GetBusinessPartner(this IDomainQueryResolver provider, string numPartner, bool byPassPipeline = false)
		{
			if (numPartner == null) throw new ArgumentNullException(nameof(numPartner));
			return (await provider.GetBusinessPartners(maxRecords: 1, numPartner: numPartner,
				byPassPipeline: byPassPipeline)).SingleOrDefault();

		}
		public static async Task<IEnumerable<TAccountInfo>> GetAccountsByUserName<TAccountInfo>(this IDomainQueryResolver provider, string userName, bool byPassPipeline = false) where TAccountInfo:AccountBase
		{
			var query = new BusinessPartnerQuery
			{
				UserName = userName,
				NumberOfRows = 200
			};
			return await provider.FetchAsync<BusinessPartnerQuery, TAccountInfo>(query, byPassPipeline);

		}
	}
}
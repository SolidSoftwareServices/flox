using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;

namespace EI.RP.DomainServices.Queries.User.PhoneMetaData
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<Ei.Rp.DomainModels.User.PhoneMetaData> GetPhoneMetaDataType(this IDomainQueryResolver provider,
			string phoneNumber, bool byPassPipeline = false)
		{
			var phoneMetaDataInfo = await provider
				.FetchAsync<PhoneMetadataResolverQuery, Ei.Rp.DomainModels.User.PhoneMetaData>(new PhoneMetadataResolverQuery
				{
					PhoneNumber = phoneNumber
				}, byPassPipeline);
			return phoneMetaDataInfo.Single();
		}
	}
}
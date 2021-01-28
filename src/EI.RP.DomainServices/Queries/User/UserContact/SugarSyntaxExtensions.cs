using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using Ei.Rp.DomainModels.User;

namespace EI.RP.DomainServices.Queries.User.UserContact
{
	public static class SugarSyntaxExtensions
	{
		public static async Task<UserContactDetails> GetUserContactInfoByAccountNumber(
			this IDomainQueryResolver provider,
			string accountNumber, bool byPassPipeline = false)
		{
			var userAccountInfo = await provider
				.FetchAsync<UserContactDetailsQuery, UserContactDetails>(new UserContactDetailsQuery
				{
					AccountNumber = accountNumber
				}, byPassPipeline);
			return userAccountInfo.Single();
		}
	}
}
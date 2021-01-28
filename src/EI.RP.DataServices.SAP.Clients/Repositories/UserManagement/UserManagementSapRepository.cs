using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap;
using EI.RP.DataModels.Sap.UserManagement.Dtos;

namespace EI.RP.DataServices.SAP.Clients.Repositories.UserManagement
{
	internal class UserManagementSapRepository : SapRepository, ISapRepositoryOfUserManagement
	{
		public UserManagementSapRepository(UserManagementSapRepositoryOptions options,IHttpClientBuilder httpClientBuilder) : base(options,httpClientBuilder) { }
		public override Task<string> GetName( )=>Task.FromResult("UserManagement");
		public async Task<SapSessionData> LoginUser(string userName, string password, bool clearExistingSessionIfAnyOnError = true)
		{
			using (Profiler.Profile(ProfileCategoryId, nameof(LoginUser)))
			{
				return await SapExecute(async (client, handler) =>
				{
					await SetNewSapSession(client, handler);
					var role = await ResolveUserRole();

					var info = BuildSessionInfo(role);

					return info;
				});
			}

			async Task<string> ResolveUserRole()
			{

				var sapUser = await GetOne(NewQuery<UserDto>().Key(userName));
				return sapUser.UserProfile.UserGroup;
			}

			SapSessionData BuildSessionInfo(string role)
			{
				var info = new SapSessionData();
				info.JsonCookie = SapSessionData.SapJsonCookie;
				info.Csrf = SapSessionData.SapCsrf;

				info.SapUserRole = role;
				return info;
			}

			async Task SetNewSapSession(HttpClient client, HttpClientHandler handler)
			{
				client.BaseAddress = new Uri(EndpointUrl, UriKind.Absolute);

				var basicAuth = $"{userName}:{password}".ToBase64String();

				client.DefaultRequestHeaders.AddOrAppendAuthorizationValue($"Basic {basicAuth}");

				var result =
					await client.GetAsync($"UserCollection('{userName}')");
				await ApiResultHandler.EnsureSuccessfulResponse(result,
					clearSessionOnAuthenticationError: clearExistingSessionIfAnyOnError);
				var csrf = "";
				if (result.Headers.TryGetValues("x-csrf-token", out var csrfCollection))
					csrf = csrfCollection.First();

				SapSessionData.SapCsrf = csrf;
				SapSessionData.SapJsonCookie =
					handler.CookieContainer.GetCookies(client.BaseAddress).ToJson();
			}
		}
		

	}
}
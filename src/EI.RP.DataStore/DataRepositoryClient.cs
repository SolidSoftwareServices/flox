using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Resiliency;
using EI.RP.CoreServices.Serialization;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using EI.RP.DataStore.SmartPlans;

namespace EI.RP.DataStore
{
	class DataRepositoryClient: IResidentialPortalDataRepository
	{
		private readonly IResidentialPortalDataSourceSettings _settings;
		private readonly IProfiler _profiler;
		private readonly IBearerTokenProvider _bearerTokenProvider;
		private readonly ISmartPlansFileSource _smartPlansFileSource;
		private readonly IHttpClientBuilder _httpClientBuilder;
		private readonly IUserSessionProvider _userSessionProvider;
		
		public DataRepositoryClient(IResidentialPortalDataSourceSettings settings,
			IProfiler profiler,
			IBearerTokenProvider bearerTokenProvider,
			ISmartPlansFileSource smartPlansFileSource,
			IHttpClientBuilder httpClientBuilder,
			IUserSessionProvider userSessionProvider)
		{
			_settings = settings;
			_profiler = profiler;
			_bearerTokenProvider = bearerTokenProvider;
			_smartPlansFileSource = smartPlansFileSource;
			_httpClientBuilder = httpClientBuilder;
			_userSessionProvider = userSessionProvider;
		}

		private async Task<HttpClient> BuildClient()
		{
			var getToken = _settings.ResidentialPortalDataSourceBearerTokenProviderUrlAsync();

			return await _httpClientBuilder.BuildAsync(customizeHttpClientFunc: async c =>
			{
				c.BaseAddress = new Uri(_settings.ResidentialPortalDataSourceBaseUrl);
				c.DefaultRequestHeaders.Accept.Clear();
				c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				await _bearerTokenProvider.AppendHeaders(c.DefaultRequestHeaders,
					await getToken);
			});
		}


	

		public async Task<CompetitionEntryDto> GetCompetitionEntry(string userName)
		{
			var uri = $"api/competitionentries/getcompetitionentrydetails?username={userName}";

			return await _Get<CompetitionEntryDto>(uri);
		}

		public async Task<CompetitionEntryDto> AddCompetitionEntry(CompetitionEntryDto newItem)
		{
			var uri = "api/competitionentries/addcompetitionentriesdetails";

			return await _Post(uri, newItem);
		}

	
		public async Task<MovingHouseProcessStatusDataModel> SetMovingHouseProcessStatus(MovingHouseProcessStatusDataModel data)
		{
			var uri = data.ID==0? "api/movehouse/addmovehousedetails": "api/movehouse/updatemovehousedetails";

			return await _Post(uri, data);
		}

		public async Task<MovingHouseProcessStatusDataModel> GetMovingHouseProcessStatus(long identity)
		{
			var uri = $"api/movehouse/getmovehousedetails?id={identity}";

			return await _Get<MovingHouseProcessStatusDataModel>(uri);
		}

		public SmartActivationNotificationDto GetSmartActivationNotificationInfo(string userName,string accountNumber)
		{
			return _userSessionProvider.Get<SmartActivationNotificationDto>(
				BuildNotificationKey(userName, accountNumber));

		}

		public SmartActivationNotificationDto Save(SmartActivationNotificationDto newItem)
		{
			if (newItem == null) throw new ArgumentNullException(nameof(newItem));
			newItem.Validate();
			_userSessionProvider.Set(BuildNotificationKey(newItem.UserName, newItem.AccountNumber), newItem);
			
			return newItem;
		}

		private string BuildNotificationKey(string userName,string accountNumber)
		{
			return $"{userName}-{accountNumber}";
		}

		public async Task<IEnumerable<SmartActivationPlanDataModel>> GetSmartActivationPlans(string groupName)
		{
			if (groupName == null) throw new ArgumentNullException(nameof(groupName));
			return (await _smartPlansFileSource.ReadFileData())
				.Where(x => x.GroupName == groupName);
		}

		private async Task<TDto> _Get<TDto>(string uri)
		{
			return await _Execute<TDto>(c => c.GetAsync(uri));
		}

		private async Task<TDto> _Post<TDto>(string uri, TDto dto)
		{
			return await _Execute<TDto>(c => c.PostAsync(uri,
				new StringContent(dto.ToJson(), Encoding.UTF8, "application/json")));
		}

		private async Task<TDto> _Execute<TDto>(Func<HttpClient, Task<HttpResponseMessage>> operationPayload)
		{
			return await ResilientOperations.Default.RetryIfNeeded(async () =>
			{
				using (var client = await BuildClient())
				{
					using (var response =
						await operationPayload(client))
					{
						var jsonResult = await response.Content.ReadAsStringAsync();

						response.EnsureSuccessStatusCode();

						var result = jsonResult.JsonToObject<TDto>();
						return result;
					}
				}
			},maxAttempts:3);
		}
	}
}

using System;
using Ei.Rp.Mvc.Core;
using Microsoft.Extensions.Configuration;

namespace EI.HybridAdapter.WebApi.Infrastructure.Settings
{
	class AppSettings : IStreamServeSettings,
		IAzureSettings,
		IRequestPipelineCoreSettings
	{
		private readonly IConfiguration _configuration;

		public AppSettings(IConfiguration configuration, IServiceProvider serviceProvider)
		{
			_configuration = configuration;
		}

		public string StreamServeLive => _configuration["DataServicesSettings:StreamServe:BaseUrl"];
		public string StreamServeArchive => _configuration["DataServicesSettings:StreamServe:ArchiveBaseUrl"];
		public string StreamServeUserName => _configuration["DataServicesSettings:StreamServe:UserName"];
		public string StreamServePassword => _configuration["DataServicesSettings:StreamServe:Password"];

		public string JwtAudience => _configuration["Azure:Audience"];

		public string AzureAdInstance => _configuration["Azure:ADInstance"];

		public string AzureAdTenant => _configuration["Azure:ADTenant"];

		public bool IsRequestVerboseLoggingEnabled => false;
	}
}
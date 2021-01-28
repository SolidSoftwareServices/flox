using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap;
using EI.RP.DataServices;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.Stubs.CoreServices.Http.Session;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.OData;
using NLog;
using ResidentialPortalCLI.OData.ODataProxy;
using IEdmModel = Microsoft.OData.Edm.IEdmModel;

namespace ResidentialPortalCLI.OData.OpenApi
{
	partial class OpenApiGenerator:IOpenApiGenerator
	{
		private static ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly IEnumerable<IODataRepository> _repositories;
		private readonly IEdmxResolver _edmxResolver;
		private readonly ISapRepositoryOfUserManagement _userManagementRepo;
		public OpenApiGenerator(IEnumerable<IODataRepository> repositories,IEdmxResolver edmxResolver)
		{
			_repositories = repositories;
			_edmxResolver = edmxResolver;
			_userManagementRepo = (ISapRepositoryOfUserManagement)_repositories.Single(x => x is ISapRepositoryOfUserManagement);
		}

		public async Task Execute(ODataSettingsFile input, ODataOpenApiGenerationOptions options)
		{

			await LoginUser(input.UserName, input.Password);
			var tasks = new ConcurrentBag<Task>();
			foreach (var repository in _repositories)
			{

				var outputFolder = await ResolveOutputFolder(repository);
				await EnsureDirectoriesExistAndRemovePreviousFiles(outputFolder);

				var metadata = (await repository.ResolveMetadata()).EdmModel;
				var document = metadata.ConvertToOpenApi(new OpenApiConvertSettings());

				var openApi = document.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);
				var path = Path.Combine(outputFolder, $"{await repository.GetName()}.openapiv2.json");
				await File.WriteAllTextAsync(path, openApi);

				openApi = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
				path = Path.Combine(outputFolder, $"{await repository.GetName()}.openapiv3.json");
				await File.WriteAllTextAsync(path, openApi);

				openApi = document.SerializeAsYaml(OpenApiSpecVersion.OpenApi2_0);
				path = Path.Combine(outputFolder, $"{await repository.GetName()}.openapiv2.yml");
				await File.WriteAllTextAsync(path, openApi);

				openApi = document.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
				path = Path.Combine(outputFolder, $"{await repository.GetName()}.openapiv3.yml");
				await File.WriteAllTextAsync(path, openApi);
			}

			//wait for all the files to be generated
			await Task.WhenAll(tasks);

			string Fix(string src)
			{
				return src
					.Replace("Type=\"Edm.DateTime\" Mode=\"In\" Precision=\"7\"", "Type=\"Edm.DateTime\" Mode=\"In\" ")
					.Replace("Type=\"Edm.DateTime\" Mode=\"In\" Precision=\"0\"", "Type=\"Edm.DateTime\" Mode=\"In\" ");
			}

			async Task<string> ResolveOutputFolder(IODataRepository repository)
			{
				var repositorySubFolder = await repository.GetName();
				var outputFolder = Path.GetFullPath(Path.Combine(options.OutputFolder, repositorySubFolder));
				return outputFolder;
			}

			async Task EnsureDirectoriesExistAndRemovePreviousFiles(string outputFolder)
			{

				EnsureDirectoryExists(outputFolder);
				
				await RemovePreviouslyGeneratedFilesIfNeeded(outputFolder);
				
				void EnsureDirectoryExists(string dirPath)
				{
					if (!Directory.Exists(dirPath))
					{
						Directory.CreateDirectory(dirPath);
						Logger.Info(() => $"Created folder {dirPath}");
					}
				}
				async Task RemovePreviouslyGeneratedFilesIfNeeded(params string[] dirs)
				{
					foreach (var dir in dirs)
					{
						var files = new DirectoryInfo(dir).GetFiles("*", SearchOption.TopDirectoryOnly);
						foreach (var file in files)
						{
							file.Delete();
							Logger.Info(() => $"Deleted {file.FullName}");
						}
					}
				}
			}

		
		}

	


		protected async Task LoginUser(string userName, string password)
		{
			SapSessionData session = await _userManagementRepo.LoginUser(userName.AdaptToSapUserNameFormat(),
				password.AdaptToSapPasswordFormat());
			SessionDataHolder.Instance.SapCsrf = session.Csrf;
			SessionDataHolder.Instance.SapJsonCookie = session.JsonCookie;
		}
	}
};
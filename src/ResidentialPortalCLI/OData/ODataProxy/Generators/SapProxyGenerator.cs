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
using Microsoft.Data.Edm;
using NLog;

using ResidentialPortalCLI.OData.ODataProxy.Generators.Dto;
using ResidentialPortalCLI.OData.ODataProxy.Generators.Function;
using ResidentialPortalCLI.Shared;

namespace ResidentialPortalCLI.OData.ODataProxy.Generators
{
	partial class SapProxyGenerator:IRpCodeGenerator
	{
		private static ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly IEnumerable<IODataRepository> _repositories;
		private readonly IEdmxResolver _edmxResolver;
		private readonly ISapRepositoryOfUserManagement _userManagementRepo;
		private readonly TemplateGenerationFiles _templateGenerationFiles=new TemplateGenerationFiles();
		public SapProxyGenerator(IEnumerable<IODataRepository> repositories,IEdmxResolver edmxResolver)
		{
			_repositories = repositories;
			_edmxResolver = edmxResolver;
			_userManagementRepo = (ISapRepositoryOfUserManagement)_repositories.Single(x => x is ISapRepositoryOfUserManagement);
		}

		public async Task Execute(ODataSettingsFile input)
		{
			const string dtoSubFolderName = "Dtos";
			const string functionsSubFolderName = "Functions";
			const string actionsSubFolderName = "Functions";
			

			await LoginUser(input.UserName, input.Password);
			var tasks = new ConcurrentBag<Task>();
			foreach (var repository in _repositories)
			{
				
				var outputFolder = await ResolveOutputFolder(repository);
				var edmx =await repository.ResolveMetadata();
				var metadata =  _edmxResolver.Parse(edmx.RawEdm);
				foreach (var task in metadata.SchemaElements
					.OrderBy(x=>x.Name)
					.Select(x=>GenerateFile(metadata,repository,x, outputFolder)))
				{
					tasks.Add(task);
				}
			}
			//wait for all the files to be generated
			await Task.WhenAll(tasks);
			
			//remove previous generations
			foreach (var repository in _repositories)
			{
				await EnsureDirectoriesExistAndRemovePreviousFiles(await ResolveOutputFolder(repository));
			}
			//write files
			await _templateGenerationFiles.FlushFiles();

			async Task<string> ResolveOutputFolder(IODataRepository repository)
			{
				var repositorySubFolder = await repository.GetName();

				var outputFolder = Path.GetFullPath(Path.Combine(input.OutputFolder, repositorySubFolder));
				return outputFolder;
			}

			async Task EnsureDirectoriesExistAndRemovePreviousFiles(string outputFolder)
			{

				EnsureDirectoryExists(outputFolder);
				var dtosDir = Path.Combine(outputFolder, dtoSubFolderName);
				EnsureDirectoryExists(dtosDir);
				var functionsDir = Path.Combine(outputFolder, functionsSubFolderName);
				EnsureDirectoryExists(functionsDir);
				await RemovePreviouslyGeneratedFilesIfNeeded(dtosDir, functionsDir);
				
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
						var files = new DirectoryInfo(dir).GetFiles("*.generated.cs", SearchOption.TopDirectoryOnly);
						foreach (var file in files)
						{
							file.Delete();
							Logger.Info(() => $"Deleted {file.FullName}");
						}
					}
				}
			}

			async Task GenerateFile(IEdmModel metadata, IODataRepository repository, IEdmSchemaElement edmSchemaElement,
				string outputFolder)
			{
			
				ICodeGenerationTemplate ttDesigner;
				switch (edmSchemaElement.SchemaElementKind)
				{

					case EdmSchemaElementKind.TypeDefinition:
					{
						var subDir = Path.Combine(outputFolder, dtoSubFolderName);

						var className = $"{edmSchemaElement.Name}Dto";
						ttDesigner = new DtoTemplate(input,metadata,edmSchemaElement, className, await repository.GetName(), await repository.GetEntityContainerName());
						_templateGenerationFiles.GenerateFile(ttDesigner, subDir, className);
					}
						break;

					case EdmSchemaElementKind.EntityContainer:
					{
						var container = (IEdmEntityContainer) edmSchemaElement;

						await Task.WhenAll(container.Elements.Where(x =>
								x.ContainerElementKind == EdmContainerElementKind.FunctionImport)
							.Cast<IEdmFunctionImport>()
							.Select(x => Task.Run(async () =>
								_templateGenerationFiles.GenerateFile(
									new FunctionTemplate(metadata, x, x.Name, await repository.GetName(),
										await repository.GetEntityContainerName()), Path.Combine(outputFolder, functionsSubFolderName), x.Name))).ToArray());

					}
						break;

					case EdmSchemaElementKind.None:
					case EdmSchemaElementKind.ValueTerm:
						//TODO:
						return;
					default:
						throw new ArgumentOutOfRangeException();
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
using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using CommandLine;
using EI.RP.CoreServices.Serialization;
using EI.RP.Stubs.IoC;
using Microsoft.Extensions.Configuration;
using ResidentialPortalCLI.OData.Infrastructure;
using ResidentialPortalCLI.OData.ODataProxy;

namespace ResidentialPortalCLI.OData.OpenApi
{
	[Verb("oDataOpenApi", HelpText = "Generates the odata openapi definitions")]
	public class ODataOpenApiGenerationOptions
	{
		[Option('s', "settingsFile", Required = true, HelpText = "location of the odata generation settings file")]
		public string SettingsFile { get; set; }
		[Option('o', "output", Required = true, HelpText = "location of the output")]
		public string OutputFolder { get; set; }

		public static int Execute(ODataOpenApiGenerationOptions input)
		{
			try
			{
				var settings = File.ReadAllText(input.SettingsFile).JsonToObject<ODataSettingsFile>();
				var configuration = BuildConfigurationRoot(settings);

				using (var container = IoCContainerBuilder.From(new MainModule(configuration)))
				{
					foreach (var codeGenerator in container.Resolve<IEnumerable<IOpenApiGenerator>>())
					{
						codeGenerator.Execute(settings,input).GetAwaiter().GetResult();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return -2;
			}

			return 0;

			IConfigurationRoot BuildConfigurationRoot(ODataSettingsFile settings)
			{
				var objAppSettingsFileFolder = Path.GetFullPath(settings.AppSettingsFileFolder);
				var configuration = new ConfigurationBuilder()
					.SetBasePath(objAppSettingsFileFolder)
					.AddJsonFile($"{settings.AppSettingsFileName}.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"{settings.AppSettingsFileName}.{settings.EnvironmentName}.json", optional: false,
						reloadOnChange: true)
					.Build();
				return configuration;
			}
		}


	}


}
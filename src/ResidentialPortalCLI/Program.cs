using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using EI.RP.TestServices.Logging;
using NLog;
using ResidentialPortalCLI.Flows.CreateFlowComponent;
using ResidentialPortalCLI.Flows.CreateFlowInput;
using ResidentialPortalCLI.Flows.CreateNewFlow;
using ResidentialPortalCLI.OData.ODataProxy;
using ResidentialPortalCLI.OData.OpenApi;

namespace ResidentialPortalCLI
{
	class Program
	{
		/// <summary>
		/// Generates Dtos for an api
		/// </summary>
		public static async Task Main(string[] args)
		{

			TestLogging.Default.ConfigureLogging(minLogLevel: LogLevel.Debug);

			Parser.Default.ParseArguments<ODataProxyGenerationOptions
					, ODataOpenApiGenerationOptions
					, CreateNewFlowOptions
					, CreateFlowInputOptions
					, CreateFlowComponentOptions>(args)
				.MapResult(
					(ODataProxyGenerationOptions o) => ODataProxyGenerationOptions.Execute(o),
					(ODataOpenApiGenerationOptions o) => ODataOpenApiGenerationOptions.Execute(o),
					(CreateNewFlowOptions o) => CreateNewFlowOptions.Execute(o).GetAwaiter().GetResult(),
					(CreateFlowInputOptions o) => CreateFlowInputOptions.Execute(o).GetAwaiter().GetResult(),
					(CreateFlowComponentOptions o) => CreateFlowComponentOptions.Execute(o).GetAwaiter().GetResult(),
					HandleParseErrors
				);

			Console.WriteLine("Press key to continue....");
			Console.ReadKey();


			int HandleParseErrors(IEnumerable<Error> errs)
			{

				var result = -1;
				Console.WriteLine(String.Join(Environment.NewLine, errs.Select(x =>
				{
					switch (x)
					{
						case NamedError namedError:
							return $"{x.GetType().Name}, {namedError.NameInfo}";
						case TokenError tokenError:
							return $"{x.GetType().Name}, {tokenError.Token}";
						case HelpVerbRequestedError helpError:
							return
								$"{x.GetType().Name}, verb:{helpError.Verb} - type:{helpError.Type} - matched:{helpError.Matched} ";
						case InvalidAttributeConfigurationError invalidAttribute:
							return $"{x.GetType().Name}, {InvalidAttributeConfigurationError.ErrorMessage}";
						case MissingGroupOptionError missingGroupOption:
							return
								$"{x.GetType().Name}, {InvalidAttributeConfigurationError.ErrorMessage} group:{missingGroupOption.Group} names:{string.Join(Environment.NewLine, missingGroupOption.Names.Select(y => $"{y.ShortName} {y.LongName}"))}";
						case MultipleDefaultVerbsError error:
							return $"{x.GetType().Name}, {MultipleDefaultVerbsError.ErrorMessage}";
						default:
							return $"{x.GetType().Name}, ";
					}
				})));


				return result;
			}
		}


	}
}
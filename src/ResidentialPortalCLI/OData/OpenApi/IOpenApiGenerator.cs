using System.Threading.Tasks;
using ResidentialPortalCLI.OData.ODataProxy;

namespace ResidentialPortalCLI.OData.OpenApi
{
	internal interface IOpenApiGenerator
	{
		Task Execute(ODataSettingsFile settings, ODataOpenApiGenerationOptions options);
	}
}
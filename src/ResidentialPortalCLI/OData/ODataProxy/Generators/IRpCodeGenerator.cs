using System.Threading.Tasks;

namespace ResidentialPortalCLI.OData.ODataProxy.Generators
{
	internal interface IRpCodeGenerator
	{
		Task Execute(ODataSettingsFile input);
	}
}
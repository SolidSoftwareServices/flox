using System.Linq;
using EI.RP.CoreServices.OData.Client;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;


namespace EI.RP.DataServices.SAP.Clients.Config
{
	internal class ODataClientSettings : IODataClientSettings
	{
		public ODataClientSettings()
		{
			var strings = typeof(AccountDto).Namespace.Split('.');
			strings = strings.Take(strings.Length - 1).ToArray();
			ODataDtosNamespaceRoot = string.Join(".", strings);
		}

		public string ODataDtosNamespaceRoot { get; }
	}
}
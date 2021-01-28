using Microsoft.OData.Edm;

namespace EI.RP.CoreServices.Ports.OData
{
	public class ODataRepositoryMetadata{
		public ODataRepositoryMetadata(string edmxString, IEdmModel model)
		{
			EdmModel = model;
			RawEdm = edmxString;
		}

		public string RawEdm { get;  }

		public IEdmModel EdmModel { get;}
	}
}
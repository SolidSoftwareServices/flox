using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EI.RP.CoreServices.System;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Csdl;

namespace EI.RP.CoreServices.OData.Client.Infrastructure.Edmx
{
	partial class LegacySapEdmxResolver
	{
		public IEdmModel Parse(string edmxString)
		{
			IEdmModel result;
			var edmx = edmxString
				//.Replace("edmx:Edmx","Edmx")
				.Replace("http://schemas.microsoft.com/ado/2007/06/edmx",
					"http://schemas.microsoft.com/ado/2008/09/edm")
				.Replace("<edmx:DataServices m:DataServiceVersion=\"2.0\">", string.Empty)
				.Replace("</edmx:DataServices>", string.Empty)
				.Replace(
					"<edmx:Edmx Version=\"1.0\" xmlns:edmx=\"http://schemas.microsoft.com/ado/2008/09/edm\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\" xmlns:sap=\"http://www.sap.com/Protocols/SAPData\">",
					string.Empty)
				.Replace("</edmx:Edmx>", string.Empty)
				.Replace("xmlns=\"http://schemas.microsoft.com/ado/2008/09/edm\"",
					"xmlns=\"http://schemas.microsoft.com/ado/2008/09/edm\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\" xmlns:sap=\"http://www.sap.com/Protocols/SAPData\"");

			using (var xmlReader = XElement.Parse(edmx).CreateReader())
			{
				if (!CsdlReader.TryParse(xmlReader.ToOneItemArray(), out result, out var errors))
				{
					var sb = new StringBuilder();
					foreach (var error in errors)
					{
						sb.Append(error.ErrorMessage).Append("\n");
					}

					throw new InvalidOperationException(sb.ToString());
				}
			}

			return result;
		}
	}
}